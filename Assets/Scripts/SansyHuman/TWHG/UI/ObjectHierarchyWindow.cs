using System.Collections;
using System.Collections.Generic;
using SansyHuman.TWHG.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of hierarchy window.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ObjectHierarchyWindow : MonoBehaviour
    {
        [Tooltip("Object that shows the hierarchy.")]
        [SerializeField] private RectTransform hierarchyWindow;

        [Tooltip("Prefab for UI element of object in hierarchy window.")]
        [SerializeField] private HierarchyObject objectPrefab;

        // Objects in the hierarchy window in the order of UI elements.
        private LinkedList<HierarchyObject> _objects;
        // Dictionary for quick search for HierarchyObject connected to the object.
        private Dictionary<ObjectEditorData, LinkedListNode<HierarchyObject>> _objNodePairs;

        void Awake()
        {
            _objects = new LinkedList<HierarchyObject>();
            _objNodePairs = new Dictionary<ObjectEditorData, LinkedListNode<HierarchyObject>>();
        }

        /// <summary>
        /// Adds an object to hierarchy window(for internal use).
        /// </summary>
        /// <param name="obj">Object to add.</param>
        public void AddObject(ObjectEditorData obj)
        {
            if (_objNodePairs.ContainsKey(obj))
            {
                UnityEngine.Debug.LogWarning("Object already added to hierarchy window.");
                return;
            }

            ObjectEditorData parent = null;
            if (obj.transform.parent)
            {
                parent = obj.transform.parent.GetComponent<ObjectEditorData>();
            }

            if (!parent) // obj is a root object.
            {
                HierarchyObject hobj = Instantiate(objectPrefab, hierarchyWindow);
                hobj.Initialize(obj, null, hierarchyWindow);
                hobj.AddExpandButtonClickListener(Refresh);
                LinkedListNode<HierarchyObject> newNode = _objects.AddLast(hobj); // Most recent root object is always at the last.
                _objNodePairs.Add(obj, newNode);
            }
            else
            {
                if (!_objNodePairs.ContainsKey(parent))
                {
                    AddObject(parent);
                }
                
                LinkedListNode<HierarchyObject> parentNode = _objNodePairs[parent];
                ObjectEditorData lastChild = parentNode.Value.LastChild;
                
                HierarchyObject hobj = Instantiate(objectPrefab, hierarchyWindow);
                hobj.Initialize(obj, parentNode.Value, hierarchyWindow);
                hobj.AddExpandButtonClickListener(Refresh);
                
                LinkedListNode<HierarchyObject> newNode = null;
                if (!lastChild)
                {
                    newNode = _objects.AddAfter(parentNode, hobj);
                }
                else
                {
                    LinkedListNode<HierarchyObject> lastChildNode = _objNodePairs[lastChild];
                    newNode = _objects.AddAfter(lastChildNode, hobj);
                }
                _objNodePairs.Add(obj, newNode);
            }

            Refresh();
        }

        /// <summary>
        /// Updates the layout of the window.
        /// </summary>
        public void Refresh()
        {
            StartCoroutine(RebuildLayout(hierarchyWindow));
        }
        
        private readonly WaitForEndOfFrame _wait = new WaitForEndOfFrame();
        private IEnumerator RebuildLayout(RectTransform tr)
        {
            yield return _wait;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
            Canvas.ForceUpdateCanvases();
            
            yield return _wait;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
            Canvas.ForceUpdateCanvases();
        }
    }
}
