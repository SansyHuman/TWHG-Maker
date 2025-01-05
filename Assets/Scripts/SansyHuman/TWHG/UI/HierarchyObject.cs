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
                offset.left = Mathf.FloorToInt(_depth * _rectTransform.rect.height);
                _layoutGroup.padding = offset;
            }
        }
        
        /// <summary>
        /// Gets the object connected to this UI.
        /// </summary>
        public ObjectEditorData ConnectedObject { get; private set; }

        void Awake()
        {
            _children = new List<HierarchyObject>();
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
        /// Adds child UI element to this UI element.
        /// </summary>
        /// <param name="child">Child UI element.</param>
        public void AddChild(HierarchyObject child)
        {
            if (_children.Contains(child))
            {
                UnityEngine.Debug.LogWarning("Child " + child.name + " is already in the hierarchy.");
                return;
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
        }
    }
}
