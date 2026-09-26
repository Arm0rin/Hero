using UnityEngine;

namespace Hero.Player
{
    public sealed class MobileInput : MonoBehaviour
    {
        public VirtualJoystick joystick;
        bool queuedJump;
        public Vector2 Move
        {
            get
            {
                Vector2 touch = joystick ? joystick.Value : Vector2.zero;
                Vector2 keyboard = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                return (touch.sqrMagnitude > 0.01f ? touch : keyboard).normalized;
            }
        }
        public void PressJump() => queuedJump = true;
        public bool ConsumeJump()
        {
            bool pressed = queuedJump || Input.GetButtonDown("Jump");
            queuedJump = false;
            return pressed;
        }
    }
}
