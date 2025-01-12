using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of scroll rect which disables mouse drag.
    /// </summary>
    public class ScrollRectNoDrag : ScrollRect, IPointerDownHandler, IPointerUpHandler, IDropHandler
    {
        [Serializable]
        public class ScrollRectClickEvent : UnityEvent {}

        /// <summary>
        /// Event called when the pointer down event occured on the scroll rect.
        /// </summary>
        public ScrollRectClickEvent onPointerDown;
        
        /// <summary>
        /// Event called when the pointer up event occured on the scroll rect.
        /// </summary>
        public ScrollRectClickEvent onPointerUp;

        protected override void Awake()
        {
            base.Awake();
            
            onPointerDown ??= new ScrollRectClickEvent();
            onPointerUp ??= new ScrollRectClickEvent();
        }

        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke();
        }

        public void OnDrop(PointerEventData eventData)
        {
            onPointerUp.Invoke();
        }
    }
}
