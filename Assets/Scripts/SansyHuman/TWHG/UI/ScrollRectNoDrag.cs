using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SansyHuman.TWHG.UI
{
    /// <summary>
    /// Component of scroll rect which disables mouse drag.
    /// </summary>
    public class ScrollRectNoDrag : ScrollRect, IPointerDownHandler, IPointerUpHandler, IDropHandler
    {
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
        public void OnPointerDown(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("Scroll rect pointer down.");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("Scroll rect pointer up.");
        }

        public void OnDrop(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("Scroll rect drop.");
        }
    }
}
