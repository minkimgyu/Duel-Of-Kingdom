using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPP.Battle.UI
{
    public class DraggableCard : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private RectTransform _parent;
        private RectTransform _rectTransform;
        private void Awake()
        {
            _parent = transform.parent.GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
