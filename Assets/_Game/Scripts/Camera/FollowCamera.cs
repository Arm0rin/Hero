using UnityEngine;
using UnityEngine.EventSystems;

namespace Hero.CameraSystem
{
    public sealed class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public LayerMask collisionMask = ~0;
        public float distance = 5f;
        public float sensitivity = 0.22f;
        float yaw;
        float pitch = 18f;
        int finger = -1;
        Vector2 previous;

        void Start() { if (target) yaw = target.eulerAngles.y; }
        void LateUpdate()
        {
            if (!target) return;
            foreach (Touch touch in Input.touches)
            {
                if (finger < 0 && touch.phase == TouchPhase.Began &&
                    touch.position.x > Screen.width * 0.35f &&
                    !(EventSystem.current && EventSystem.current.IsPointerOverGameObject(touch.fingerId)))
                { finger = touch.fingerId; previous = touch.position; }
                if (touch.fingerId != finger) continue;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) { finger = -1; continue; }
                Vector2 delta = touch.position - previous;
                previous = touch.position;
                yaw += delta.x * sensitivity;
                pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -15f, 65f);
            }
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * 3f;
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 3f, -15f, 65f);
            }
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 pivot = target.position + Vector3.up * 1.35f;
            Vector3 direction = rotation * Vector3.back;
            float actualDistance = distance;
            if (Physics.SphereCast(pivot, 0.18f, direction, out RaycastHit hit,
                distance, collisionMask, QueryTriggerInteraction.Ignore))
                actualDistance = Mathf.Max(0.3f, hit.distance - 0.12f);
            transform.position = Vector3.Lerp(transform.position, pivot + direction * actualDistance,
                1f - Mathf.Exp(-14f * Time.deltaTime));
            transform.rotation = rotation;
        }
    }
}
