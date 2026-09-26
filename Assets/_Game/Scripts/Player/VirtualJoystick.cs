using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hero.Player
{
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform baseRect;
        public RectTransform knob;
        public float radius = 68f;
        public Vector2 Value { get; private set; }
        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);
        public void OnDrag(PointerEventData eventData)
        {
            if (!baseRect || !knob) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect,
                eventData.position, eventData.pressEventCamera, out Vector2 local))
            {
                Value = Vector2.ClampMagnitude(local / radius, 1f);
                knob.anchoredPosition = Value * radius;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (knob) knob.anchoredPosition = Vector2.zero;
        }
        void OnDisable()
        {
            Value = Vector2.zero;
            if (knob) knob.anchoredPosition = Vector2.zero;
        }
    }
}
