using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of object name field.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class ObjectNameField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
    {
        [Tooltip("The color of the field when the object is selected.")]
        [SerializeField] private Color selectColor;

        [Tooltip("The color of the field when the cursor is on the object name field.")]
        [SerializeField] private Color hoverColor;

        [Tooltip("The text of the field.")]
        [SerializeField] private TextMeshProUGUI nameText;

        [Tooltip("The parent UI of the field.")]
        [SerializeField] private HierarchyObject objectUI;
        
        [Serializable]
        public class ObjectNameFieldEvent : UnityEvent<ObjectNameField> { }
        
        /// <summary>
        /// Event called when the pointer down event occured on the object.
        /// </summary>
        public ObjectNameFieldEvent onObjectPointerDown;
        
        /// <summary>
        /// Event called when the pointer up event occured on the object.
        /// </summary>
        public ObjectNameFieldEvent onObjectPointerUp;

        private RawImage _field;
        private bool _selected;
        private bool _hover;

        /// <summary>
        /// Gets and sets whether the object is selected.
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (_selected)
                {
                    _field.color = selectColor;
                }
                else
                {
                    if (_hover)
                    {
                        _field.color = hoverColor;
                    }
                    else
                    {
                        _field.color = new Color(0, 0, 0, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Gets whether the cursor is on the object.
        /// </summary>
        public bool Hover
        {
            get => _hover;
            private set
            {
                _hover = value;
                if (_hover)
                {
                    if (_selected)
                    {
                        _field.color = selectColor;
                    }
                    else
                    {
                        _field.color = hoverColor;
                    }
                }
                else
                {
                    Color col = selectColor;
                    col.a *= _selected ? 1 : 0;
                    _field.color = col;
                }
            }
        }

        /// <summary>
        /// Gets and sets the name text of the field.
        /// </summary>
        public string Text
        {
            get => nameText.text;
            set => nameText.text = value;
        }
        
        /// <summary>
        /// Gets the parent UI that represents the object in the stage editor.
        /// </summary>
        public HierarchyObject ObjectUI => objectUI;

        void Awake()
        {
            onObjectPointerDown ??= new ObjectNameFieldEvent();
            onObjectPointerUp ??= new ObjectNameFieldEvent();
            
            _field = GetComponent<RawImage>();
            Selected = false;
            Hover = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hover = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Selected = true;
                onObjectPointerDown.Invoke(this);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && _hover)
            {
                onObjectPointerUp.Invoke(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onObjectPointerUp.Invoke(this);
            }
        }
    }
}
