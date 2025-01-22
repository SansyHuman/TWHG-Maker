using System;
using System.Collections;
using System.Collections.Generic;
using SansyHuman.TWHG.Core.Collections;
using SansyHuman.TWHG.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using InputSystem = SansyHuman.TWHG.Core.InputSystem;

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

        [Tooltip("Object that shows selected objects with rectangle.")]
        [SerializeField] private SelectedObjects selectionRect;

        [Tooltip("Prefab for UI element of object in hierarchy window.")]
        [SerializeField] private HierarchyObject objectPrefab;

        // Objects in the hierarchy window in the order of UI elements.
        private IndexedLinkedList<HierarchyObject, ObjectEditorData> _objects;

        // Objects currently selected in the hierarchy.
        private IndexedLinkedList<ObjectEditorData, ObjectEditorData> _selectedObjects;
        // Object lastly selected in the hierarchy.
        private ObjectEditorData _lastSelectedObject;

        void Awake()
        {
            _objects = new IndexedLinkedList<HierarchyObject, ObjectEditorData>();
            _selectedObjects = new IndexedLinkedList<ObjectEditorData, ObjectEditorData>();
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
                InitializeHierarchyObject(hobj, obj, null);
                
                _objects.AddLast(obj, hobj); // Most recent root object is always at the last.
            }
            else
            {
                if (!_objects.ContainsKey(parent))
                {
                    AddObject(parent);
                }
                
                LinkedListNode<HierarchyObject> parentNode = _objects.GetNode(parent);
                ObjectEditorData lastChild = parentNode.Value.LastChild;
                
                HierarchyObject hobj = Instantiate(objectPrefab, hierarchyWindow);
                InitializeHierarchyObject(hobj, obj, parentNode.Value);

                if (!lastChild)
                {
                    _objects.AddAfter(parentNode,obj, hobj);
                }
                else
                {
                    LinkedListNode<HierarchyObject> lastChildNode = _objects.GetNode(lastChild);
                    _objects.AddAfter(lastChildNode, obj, hobj);
                }
            }

            Refresh();
        }

        /// <summary>
        /// Destroys the object and removes it from the hierarchy window.
        /// </summary>
        /// <param name="obj">Object to destroy.</param>
        public void DestroyObject(ObjectEditorData obj)
        {
            if (!_objects.ContainsKey(obj))
            {
                UnityEngine.Debug.LogWarning($"The object {obj.name} does not exist.");
                return;
            }

            LinkedListNode<HierarchyObject> objNode = _objects.GetNode(obj);
            LinkedListNode<HierarchyObject> lastChildNode = _objects.GetNode(objNode.Value.LastChild ?? obj);
            LinkedListNode<HierarchyObject> lastChildNextNode = lastChildNode.Next;

            HierarchyObject objUi = objNode.Value;
            
            if (objUi.Parent)
            {
                _objects[objUi.Parent].RemoveChild(objUi, false);
            }

            LinkedListNode<HierarchyObject> current = objNode;
            while (current != lastChildNextNode)
            {
                LinkedListNode<HierarchyObject> next = current.Next;
                
                RemoveSelectedObjects(current.Value);
                _objects.RemoveByKey(current.Value.ConnectedObject);

                current = next;
            }
            
            Destroy(objUi.ConnectedObject.gameObject);
            Destroy(objUi.gameObject);
            
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
                if (_selectedObjects.ContainsKey(_lastPointedField.ObjectUI.ConnectedObject))
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
                if (!_lastSelectedObject || !_objects[_lastSelectedObject].gameObject.activeInHierarchy)
                {
                    ClearSelectedObjects();
                    AddSelectedObjects(nameField.ObjectUI);
                }
                else
                {
                    // Select all objects between nameField and _lastSelectedObject
                    LinkedListNode<HierarchyObject> lastSelected = _objects.GetNode(_lastSelectedObject);
                    LinkedListNode<HierarchyObject> lastPointed = _objects.GetNode(nameField.ObjectUI.ConnectedObject);
                    if (lastSelected == lastPointed)
                    {
                        return;
                    }

                    LinkedListNode<HierarchyObject> prevTmp = lastSelected;
                    LinkedListNode<HierarchyObject> nextTmp = lastSelected;
                    bool prev = false;
                    bool next = false;

                    while (prevTmp != null || nextTmp != null)
                    {
                        prevTmp = prevTmp?.Previous;
                        nextTmp = nextTmp?.Next;

                        if (prevTmp == lastPointed)
                        {
                            prev = true;
                            break;
                        }
                        if (nextTmp == lastPointed)
                        {
                            next = true;
                            break;
                        }
                    }

                    if (prev)
                    {
                        AddSelectedObjects(lastPointed, lastSelected);
                    }
                    else if (next)
                    {
                        AddSelectedObjects(lastSelected, lastPointed);
                    }
                }
                
                _shiftPressed = true;
            }
            else
            {
                if (_selectedObjects.ContainsKey(_lastPointedField.ObjectUI.ConnectedObject))
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
                _lastSelectedObject = null;
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
                        _lastPointedField = null;
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
            else // Let selected objects to be the children of nameField.
            {
                // ... only when the new parent is not selected object.
                if (!_selectedObjects.ContainsKey(nameField.ObjectUI.ConnectedObject))
                {
                    HierarchyObject newParent = nameField.ObjectUI;
                    foreach (var obj in _selectedObjects)
                    {
                        ChangeParent(_objects[obj], newParent, false);
                    }
                
                    if (!newParent.Expanded)
                    {
                        newParent.OnExpandButtonClick();
                    }

                    Refresh();
                }
            }
                
            _lastSelectedObject = _lastPointedField?.ObjectUI.ConnectedObject;

            _pointerDown = false;
            _ctrlPressed = false;
            _shiftPressed = false;
            _lastPointedFieldAlreadySelected = false;
        }

        private void OnScrollRectPointerDown()
        {
            // Deselect all objects.
            ClearSelectedObjects();
            _lastPointedField = null;
            _lastSelectedObject = null;
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
                if (!_objects[obj].Parent)
                {
                    continue;
                }
                ChangeParent(_objects[obj], null, false);
            }
            
            Refresh();
            
            _lastSelectedObject = _lastPointedField?.ObjectUI.ConnectedObject;
            
            _pointerDown = false;
            _ctrlPressed = false;
            _shiftPressed = false;
            _lastPointedFieldAlreadySelected = false;
        }
        
        private void OnDeletePressed(InputAction.CallbackContext context)
        {
            while (_selectedObjects.Count > 0)
            {
                DestroyObject(_selectedObjects.First.Value);
            }
        }

        private void AddSelectedObjects(params HierarchyObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (!objects[i].ConnectedObject.selectable)
                {
                    continue;
                }
                
                ObjectEditorData obj = objects[i].ConnectedObject;
                if (_selectedObjects.ContainsKey(obj))
                {
                    continue;
                }
                
                _selectedObjects.AddLast(obj, obj);
                objects[i].Selected = true;
                selectionRect.AddSelectedObject(objects[i].ConnectedObject.gameObject);
            }
        }
        
        private void AddSelectedObjects(LinkedListNode<HierarchyObject> first, LinkedListNode<HierarchyObject> last)
        {
            for (var current = first; current != last.Next; current = current.Next)
            {
                if (current == null)
                {
                    break;
                }

                if (!current.Value.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (!current.Value.ConnectedObject.selectable)
                {
                    continue;
                }
                
                ObjectEditorData obj = current.Value.ConnectedObject;
                if (_selectedObjects.ContainsKey(obj))
                {
                    continue;
                }

                _selectedObjects.AddLast(obj, obj);
                current.Value.Selected = true;
                selectionRect.AddSelectedObject(current.Value.ConnectedObject.gameObject);
            }
        }

        private void RemoveSelectedObjects(params HierarchyObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                ObjectEditorData obj = objects[i].ConnectedObject;
                if (!_selectedObjects.ContainsKey(obj))
                {
                    continue;
                }

                _selectedObjects.RemoveByKey(_selectedObjects[obj]);
                objects[i].Selected = false;
                selectionRect.RemoveSelectedObject(objects[i].ConnectedObject.gameObject);
            }
        }

        private void ClearSelectedObjects()
        {
            foreach (var obj in _selectedObjects)
            {
                if (!_objects.ContainsKey(obj))
                {
                    continue;
                }

                _objects[obj].Selected = false;
                selectionRect.RemoveSelectedObject(obj.gameObject);
            }

            _selectedObjects.Clear();
        }

        private Stack<LinkedListNode<HierarchyObject>> _changeParentTmp = new Stack<LinkedListNode<HierarchyObject>>();
        private void ChangeParent(HierarchyObject child, HierarchyObject newParent, bool refresh = true)
        {
            if (!child.ConnectedObject.canHaveParent)
            {
                return;
            }

            if (newParent && !newParent.ConnectedObject.canHaveChildren)
            {
                return;
            }
            
            if (newParent && child.Parent == newParent.ConnectedObject)
            {
                return;
            }
            
            LinkedListNode<HierarchyObject> childNode = _objects.GetNode(child.ConnectedObject);
            LinkedListNode<HierarchyObject> prevChildNode = childNode.Previous;
            LinkedListNode<HierarchyObject> childLastChildNode = _objects.GetNode(child.LastChild ?? child.ConnectedObject);

            if (child.Parent)
            {
                _objects[child.Parent].RemoveChild(child, !newParent);
            }
            LinkedListNode<HierarchyObject> newPrevChildNode = null;
            if (newParent)
            {
                newPrevChildNode = _objects.GetNode(newParent.LastChild ?? newParent.ConnectedObject);
                newParent.AddChild(child);
            }
            else
            {
                newPrevChildNode = _objects.Last;
                if (newPrevChildNode == childNode)
                {
                    // The child node is the last object of the hierarchy. Change new prev child to previous one.
                    newPrevChildNode = newPrevChildNode.Previous;
                }
            }
            
            LinkedListNode<HierarchyObject> nextToRemove = childLastChildNode;
            do
            {
                LinkedListNode<HierarchyObject> tmp = nextToRemove.Previous;
                _changeParentTmp.Push(nextToRemove);
                _objects.RemoveByKey(nextToRemove.Value.ConnectedObject);
                nextToRemove = tmp;
            } while (nextToRemove != prevChildNode);

            LinkedListNode<HierarchyObject> addNextTo = newPrevChildNode;
            while (_changeParentTmp.Count > 0)
            {
                LinkedListNode<HierarchyObject> tmp = _changeParentTmp.Pop();
                _objects.AddAfter(addNextTo, tmp.Value.ConnectedObject, tmp);
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

        private void OnEnable()
        {
            InputSystem.Actions.Editor.Del.performed += OnDeletePressed;
        }

        void OnDisable()
        {
            InputSystem.Actions.Editor.Del.performed -= OnDeletePressed;
        }
    }
}
