using System.Collections;
using System.Collections.Generic;
using SansyHuman.TWHG.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of Hierarchy window.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ObjectHierarchyWindow : MonoBehaviour
    {
        [Tooltip("Object that shows the hierarchy.")]
        [SerializeField] private RectTransform hierarchyWindow;

        [Tooltip("Prefab for UI element of object in hierarchy window.")]
        [SerializeField] private HierarchyObject objectPrefab;

        private Dictionary<ObjectEditorData, HierarchyObject> _objects;

        void Awake()
        {
            _objects = new Dictionary<ObjectEditorData, HierarchyObject>();
        }

        /// <summary>
        /// Adds an object to hierarchy window(for internal use).
        /// </summary>
        /// <param name="obj">Object to add.</param>
        public void AddObject(ObjectEditorData obj)
        {
            if (_objects.ContainsKey(obj))
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
                _objects.Add(obj, hobj);
            }
            else
            {
                if (!_objects.ContainsKey(parent))
                {
                    AddObject(parent);
                }
                
                HierarchyObject hparent = _objects[parent];
                
                HierarchyObject hobj = Instantiate(objectPrefab, hierarchyWindow);
                hobj.Initialize(obj, hparent, hierarchyWindow);
                hobj.AddExpandButtonClickListener(Refresh);
                _objects.Add(obj, hobj);
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
        }
    }
}
