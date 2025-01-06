using System.Collections.Generic;
using SansyHuman.TWHG.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    public class HierarchyObject : MonoBehaviour
    {
        // The depth of the object node.
        private int _depth = 0;

        // Whether the children are shown.
        private bool _hierarchyExpanded;

        [Tooltip("Button that expands the hierarchy and shows children.")]
        [SerializeField] private Button expandButton;
        
        [Tooltip("Text that shows the name of the object.")]
        [SerializeField] private TextMeshProUGUI objectName;

        // UI elements of children.
        private List<HierarchyObject> _children;
        
        // UI element of parent.
        private HierarchyObject _parent;
        
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
                offset.left = _depth == 0 ? 0 : Mathf.FloorToInt(_rectTransform.rect.height);
                _layoutGroup.padding = offset;

                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].Depth = _depth + 1;
                }
            }
        }
        
        /// <summary>
        /// Gets the object connected to this UI.
        /// </summary>
        public ObjectEditorData ConnectedObject { get; private set; }

        void Awake()
        {
            _children = new List<HierarchyObject>();
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
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].gameObject.SetActive(_hierarchyExpanded);
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
        public void Initialize(ObjectEditorData obj, HierarchyObject parent)
        {
            ConnectedObject = obj;
            objectName.text = obj.gameObject.name;
            parent.AddChild(this);
        }

        /// <summary>
        /// Adds child to this object. Note that it changes hierarchy in both UI and real object.
        /// </summary>
        /// <param name="child">Child UI element.</param>
        public void AddChild(HierarchyObject child)
        {
            if (_children.Contains(child))
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
            _children.Add(child);
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
            int index = _children.IndexOf(child);
            if (index < 0)
            {
                UnityEngine.Debug.LogWarning("Child " + child.name + " is not in the hierarchy.");
                return;
            }
            
            child._rectTransform.SetParent(null);
            if (updateDepth)
            {
                child.Depth = 0;
            }
            _children.RemoveAt(index);
            if (_children.Count == 0)
            {
                expandButton.interactable = false;
                ExpandButtonUpdate();
            }
            child._parent = null;
            
            child.ConnectedObject.transform.SetParent(null);
        }
    }
}
