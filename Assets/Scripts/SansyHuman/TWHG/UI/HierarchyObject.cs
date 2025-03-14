using System.Collections.Generic;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of UI element which indicates an object in the editor.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class HierarchyObject : MonoBehaviour
    {
        // The depth of the object node.
        private int _depth = 0;

        // Whether the children are shown.
        private bool _hierarchyExpanded;

        [Tooltip("Button that expands the hierarchy and shows children.")]
        [SerializeField] private Button expandButton;
        
        [Tooltip("Text that shows the name of the object.")]
        [SerializeField] private ObjectNameField objectName;

        // UI elements of children.
        private IndexedLinkedList<HierarchyObject, ObjectEditorData> _children;
        
        // UI element of parent.
        private HierarchyObject _parent;

        // RectTransform of the hierarchy window.
        private RectTransform _hierarchyWindow;
        
        private RectTransform _rectTransform;
        private VerticalLayoutGroup _layoutGroup;

        /// <summary>
        /// Gets the depth of the node. If the object has no parent, the depth is 0.
        /// </summary>
        public int Depth
        {
            get => _depth;
            private set
            {
                _depth = value;
                RectOffset offset = _layoutGroup.padding;
                offset.left = _depth == 0 ? 0 : Mathf.FloorToInt(expandButton.GetComponent<RectTransform>().rect.height);
                _layoutGroup.padding = offset;

                foreach (var child in _children)
                {
                    child.Depth = _depth + 1;
                }
            }
        }
        
        /// <summary>
        /// Gets the object connected to this UI.
        /// </summary>
        public ObjectEditorData ConnectedObject { get; private set; }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public ObjectEditorData Parent
        {
            get
            {
                if (!_parent)
                {
                    return null;
                }
                
                return _parent.ConnectedObject;
            }
        }

        /// <summary>
        /// Gets the last child object in the UI order. If child does not exist, returns null.
        /// Note that it searches for last child of all subtree.
        /// </summary>
        public ObjectEditorData LastChild
        {
            get
            {
                if (_children.Count == 0)
                {
                    return null;
                }

                HierarchyObject lastChild = _children.Last.Value;
                while (true)
                {
                    if (lastChild._children.Count == 0)
                    {
                        break;
                    }

                    lastChild = lastChild._children.Last.Value;
                }
                
                return lastChild.ConnectedObject;
            }
        }

        /// <summary>
        /// Gets the last direct child object in the UI order. If child does not exist, returns null.
        /// </summary>
        public ObjectEditorData LastDirectChild
        {
            get
            {
                if (_children.Count == 0)
                {
                    return null;
                }
                
                return _children.Last.Value.ConnectedObject;
            }
        }

        /// <summary>
        /// Gets and sets whether the object is selected.
        /// </summary>
        public bool Selected
        {
            get => objectName.Selected;
            set => objectName.Selected = value;
        }
        
        /// <summary>
        /// Gets whether the children of the objects are shown.
        /// </summary>
        public bool Expanded => _hierarchyExpanded;

        void Awake()
        {
            _children = new IndexedLinkedList<HierarchyObject, ObjectEditorData>();
            _parent = null;
            _rectTransform = GetComponent<RectTransform>();
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            expandButton.interactable = false;
            _hierarchyExpanded = false;
        }

        public void OnExpandButtonClick()
        {
            _hierarchyExpanded = !_hierarchyExpanded;
            ExpandButtonUpdate();
            foreach (var child in _children)
            {
                child.gameObject.SetActive(_hierarchyExpanded);
            }
        }

        private void ExpandButtonUpdate()
        {
            RectTransform buttonTr = expandButton.GetComponent<RectTransform>();
            Quaternion rotation = buttonTr.rotation;
            rotation.eulerAngles = new Vector3(0, 0, _hierarchyExpanded ? -180 : -90);
            buttonTr.rotation = rotation;
        }

        /// <summary>
        /// Connects an object to this UI(only for internal usage).
        /// </summary>
        /// <param name="obj">Object to connect.</param>
        /// <param name="parent">Parent of this object.</param>
        /// <param name="hierarchyWindow">Hierarchy window ui.</param>
        public void Initialize(ObjectEditorData obj, HierarchyObject parent, RectTransform hierarchyWindow)
        {
            ConnectedObject = obj;
            objectName.Text = obj.gameObject.name;
            _hierarchyWindow = hierarchyWindow;
            if (!parent)
            {
                _rectTransform.SetParent(_hierarchyWindow);
            }
            else
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Adds child to this object. Note that it changes hierarchy in both UI and real object.
        /// </summary>
        /// <param name="child">Child UI element.</param>
        public void AddChild(HierarchyObject child)
        {
            if (_children.ContainsKey(child.ConnectedObject))
            {
                UnityEngine.Debug.LogWarning("Child " + child.name + " is already in the hierarchy.");
                return;
            }

            if (child._parent)
            {
                child._parent.RemoveChild(child, updateDepth:false);
            }
            
            child._rectTransform.SetParent(_rectTransform);
            child.Depth = _depth + 1;
            child.gameObject.SetActive(_hierarchyExpanded);
            if (_children.Count == 0)
            {
                expandButton.interactable = true;
                ExpandButtonUpdate();
            }
            _children.AddLast(child.ConnectedObject, child);
            child._parent = this;
            
            child.ConnectedObject.transform.SetParent(ConnectedObject.transform);
        }

        /// <summary>
        /// Removes child from this object and make it as a root object. Note that it changes hierarchy
        /// in both UI and real object.
        /// </summary>
        /// <param name="child">Chile UI element.</param>
        /// <param name="updateDepth">If true update the depth of the child and its children. Else,
        /// do not update the depth.</param>
        public void RemoveChild(HierarchyObject child, bool updateDepth = true)
        {
            if (child._parent != this)
            {
                UnityEngine.Debug.LogWarning("Child " + child.name + " is not in the hierarchy.");
                return;
            }
            
            child._rectTransform.SetParent(_hierarchyWindow);
            if (updateDepth)
            {
                child.Depth = 0;
            }

            _children.RemoveByKey(child.ConnectedObject);
            if (_children.Count == 0)
            {
                expandButton.interactable = false;
                ExpandButtonUpdate();
            }
            child._parent = null;
            
            child.ConnectedObject.transform.SetParent(null);
        }

        /// <summary>
        /// Adds a listener to the expand button click event.
        /// </summary>
        /// <param name="callback">Callback function.</param>
        public void AddExpandButtonClickListener(UnityAction callback)
        {
            expandButton.onClick.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener from the expand button click event.
        /// </summary>
        /// <param name="callback">Callback function.</param>
        public void RemoveExpandButtonClickListener(UnityAction callback)
        {
            expandButton.onClick.RemoveListener(callback);
        }

        /// <summary>
        /// Adds a listener to the event called when the pointer down event occured on the name field.
        /// </summary>
        /// <param name="callback">Callback function</param>
        public void AddObjectPointerDownListener(UnityAction<ObjectNameField> callback)
        {
            objectName.onObjectPointerDown.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener from the event called when the pointer down event occured on the name field.
        /// </summary>
        /// <param name="callback">Callback function</param>
        public void RemoveObjectPointerDownListener(UnityAction<ObjectNameField> callback)
        {
            objectName.onObjectPointerDown.RemoveListener(callback);
        }
        
        /// <summary>
        /// Adds a listener to the event called when the pointer up event occured on the name field.
        /// </summary>
        /// <param name="callback">Callback function</param>
        public void AddObjectPointerUpListener(UnityAction<ObjectNameField> callback)
        {
            objectName.onObjectPointerUp.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener from the event called when the pointer up event occured on the name field.
        /// </summary>
        /// <param name="callback">Callback function</param>
        public void RemoveObjectPointerUpListener(UnityAction<ObjectNameField> callback)
        {
            objectName.onObjectPointerUp.RemoveListener(callback);
        }
    }
}
