using System.Collections;
using System.Collections.Generic;
using SansyHuman.TWHG.Core;
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

        [Tooltip("Object that contains the scroll rect which contains all objects.")]
        [SerializeField] private ScrollRectNoDrag hierarchyViewer;

        [Tooltip("Prefab for UI element of object in hierarchy window.")]
        [SerializeField] private HierarchyObject objectPrefab;

        // Objects in the hierarchy window in the order of UI elements.
        private LinkedList<HierarchyObject> _objects;
        // Dictionary for quick search for HierarchyObject connected to the object.
        private Dictionary<ObjectEditorData, LinkedListNode<HierarchyObject>> _objNodePairs;

        // Objects currently selected in the hierarchy.
        private LinkedList<ObjectEditorData> _selectedObjects;
        private Dictionary<ObjectEditorData, LinkedListNode<ObjectEditorData>> _selectedObjNodePairs;
        // Object lastly selected in the hierarchy.
        private ObjectEditorData _lastSelectedObject;

        void Awake()
        {
            _objects = new LinkedList<HierarchyObject>();
            _objNodePairs = new Dictionary<ObjectEditorData, LinkedListNode<HierarchyObject>>();
            _selectedObjects = new LinkedList<ObjectEditorData>();
            _selectedObjNodePairs = new Dictionary<ObjectEditorData, LinkedListNode<ObjectEditorData>>();
            _lastSelectedObject = null;
            
            hierarchyViewer.onPointerDown.AddListener(OnScrollRectPointerDown);
            hierarchyViewer.onPointerUp.AddListener(OnScrollRectPointerUp);
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
                InitializeHierarchyObject(hobj, obj, null);
                
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
                InitializeHierarchyObject(hobj, obj, parentNode.Value);
                
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

        private void InitializeHierarchyObject(HierarchyObject hobj, ObjectEditorData obj, HierarchyObject parent)
        {
            hobj.Initialize(obj, parent, hierarchyWindow);
            hobj.AddExpandButtonClickListener(Refresh);
            hobj.AddObjectPointerDownListener(OnObjectPointerDown);
            hobj.AddObjectPointerUpListener(OnObjectPointerUp);
        }

        private bool _pointerDown = false;
        private ObjectNameField _lastPointedField = null;
        private bool _ctrlPressed = false;
        private bool _shiftPressed = false;
        private bool _lastPointedFieldAlreadySelected = false;
        
        private void OnObjectPointerDown(ObjectNameField nameField)
        {
            _pointerDown = true;
            _lastPointedField = nameField;
            if (InputSystem.Actions.Editor.Ctrl.IsPressed())
            {
                if (_selectedObjNodePairs.ContainsKey(_lastPointedField.ObjectUI.ConnectedObject))
                {
                    // Deselect object after the pointer is up on the same field.
                    _lastPointedFieldAlreadySelected = true;
                }
                else
                {
                    AddSelectedObjects(nameField.ObjectUI);
                    _lastPointedFieldAlreadySelected = false;
                }
                
                _ctrlPressed = true;
            }
            else if (InputSystem.Actions.Editor.Shift.IsPressed())
            {
                _shiftPressed = true;
            }
            else
            {
                if (_selectedObjNodePairs.ContainsKey(_lastPointedField.ObjectUI.ConnectedObject))
                {
                    // Select only this object after the pointer is up on the same field.
                    _lastPointedFieldAlreadySelected = true;
                }
                else
                {
                    ClearSelectedObjects();
                    AddSelectedObjects(nameField.ObjectUI);
                }
            }
        }

        private void OnObjectPointerUp(ObjectNameField nameField)
        {
            if (!_lastPointedField || !_pointerDown)
            {
                return;
            }
            
            if (nameField == _lastPointedField)
            {
                if (_ctrlPressed) // Reverse the selection state of the object
                {
                    if (_lastPointedFieldAlreadySelected)
                    {
                        _lastPointedField.Selected = false;
                        RemoveSelectedObjects(_lastPointedField.ObjectUI);
                    }
                }
                else if (_shiftPressed)
                {
                    
                }
                else
                {
                    if (_lastPointedFieldAlreadySelected)
                    {
                        ClearSelectedObjects();
                        AddSelectedObjects(_lastPointedField.ObjectUI);
                    }
                }
            }
            else // Let selected object to be the children of nameField.
            {
                HierarchyObject newParent = nameField.ObjectUI;
                foreach (var obj in _selectedObjects)
                {
                    ChangeParent(_objNodePairs[obj].Value, newParent, false);
                }
                
                if (!newParent.Expanded)
                {
                    newParent.OnExpandButtonClick();
                }

                Refresh();
            }
                
            _lastSelectedObject = _lastPointedField.ObjectUI.ConnectedObject;

            _pointerDown = false;
            _ctrlPressed = false;
            _shiftPressed = false;
            _lastPointedFieldAlreadySelected = false;
        }

        private void OnScrollRectPointerDown()
        {
            // Deselect all objects.
            ClearSelectedObjects();
        }

        private void OnScrollRectPointerUp()
        {
            if (!_pointerDown)
            {
                return;
            }
            
            // Make all selected objects to root objects
            foreach (var obj in _selectedObjects)
            {
                ChangeParent(_objNodePairs[obj].Value, null, false);
            }
            
            Refresh();
            
            _pointerDown = false;
            _ctrlPressed = false;
            _shiftPressed = false;
            _lastPointedFieldAlreadySelected = false;
        }

        private void AddSelectedObjects(params HierarchyObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                ObjectEditorData obj = objects[i].ConnectedObject;
                if (_selectedObjNodePairs.ContainsKey(obj))
                {
                    continue;
                }

                LinkedListNode<ObjectEditorData> newNode = _selectedObjects.AddLast(obj);
                _selectedObjNodePairs.Add(obj, newNode);
                objects[i].Selected = true;
            }
        }

        private void RemoveSelectedObjects(params HierarchyObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                ObjectEditorData obj = objects[i].ConnectedObject;
                if (!_selectedObjNodePairs.ContainsKey(obj))
                {
                    continue;
                }

                _selectedObjects.Remove(_selectedObjNodePairs[obj]);
                _selectedObjNodePairs.Remove(obj);
                objects[i].Selected = false;
            }
        }

        private void ClearSelectedObjects()
        {
            foreach (var obj in _selectedObjects)
            {
                if (!_objNodePairs.ContainsKey(obj))
                {
                    continue;
                }

                _objNodePairs[obj].Value.Selected = false;
            }

            _selectedObjects.Clear();
            _selectedObjNodePairs.Clear();
        }

        private Stack<LinkedListNode<HierarchyObject>> _changeParentTmp = new Stack<LinkedListNode<HierarchyObject>>();
        private void ChangeParent(HierarchyObject child, HierarchyObject newParent, bool refresh = true)
        {
            if (newParent && child.Parent == newParent.ConnectedObject)
            {
                return;
            }
            
            LinkedListNode<HierarchyObject> childNode = _objNodePairs[child.ConnectedObject];
            LinkedListNode<HierarchyObject> prevChildNode = childNode.Previous;
            LinkedListNode<HierarchyObject> childLastChildNode = _objNodePairs[child.LastChild ?? child.ConnectedObject];

            if (child.Parent)
            {
                LinkedListNode<HierarchyObject> parentNode = _objNodePairs[child.Parent];
                parentNode.Value.RemoveChild(child, !newParent);
            }
            LinkedListNode<HierarchyObject> newPrevChildNode = null;
            if (newParent)
            {
                newPrevChildNode = _objNodePairs[newParent.LastChild ?? newParent.ConnectedObject];
                newParent.AddChild(child);
            }
            else
            {
                newPrevChildNode = _objects.Last;
            }
            
            LinkedListNode<HierarchyObject> nextToRemove = childLastChildNode;
            do
            {
                LinkedListNode<HierarchyObject> tmp = nextToRemove.Previous;
                _changeParentTmp.Push(nextToRemove);
                _objects.Remove(nextToRemove);
                nextToRemove = tmp;
            } while (nextToRemove != prevChildNode);

            LinkedListNode<HierarchyObject> addNextTo = newPrevChildNode;
            while (_changeParentTmp.Count > 0)
            {
                LinkedListNode<HierarchyObject> tmp = _changeParentTmp.Pop();
                _objects.AddAfter(addNextTo, tmp);
                addNextTo = tmp;
            }

            if (refresh)
            {
                Refresh();
            }
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
