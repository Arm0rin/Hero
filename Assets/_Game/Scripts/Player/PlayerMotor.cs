using UnityEngine;

namespace Hero.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public PlayerMovementConfig config;
        public Transform cameraTransform;
        public MobileInput input;
        CharacterController controller;
        Vector3 horizontalVelocity;
        float verticalVelocity;
        float lastGrounded = float.NegativeInfinity;
        float lastJumpPressed = float.NegativeInfinity;
        bool jumpConsumed;

        void Awake() => controller = GetComponent<CharacterController>();

        void Update()
        {
            if (config == null || input == null) return;
            bool grounded = controller.isGrounded;
            if (grounded)
            {
                lastGrounded = Time.time;
                if (verticalVelocity < 0f) verticalVelocity = -2f;
                jumpConsumed = false;
            }
            if (input.ConsumeJump()) lastJumpPressed = Time.time;

            Vector2 axes = input.Move;
            Vector3 forward = cameraTransform ? cameraTransform.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 desired = (right * axes.x + forward * axes.y) * config.moveSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, desired,
                config.acceleration * (grounded ? 1f : config.airControl) * Time.deltaTime);
            if (!jumpConsumed && Time.time - lastGrounded <= config.coyoteTime &&
                Time.time - lastJumpPressed <= config.jumpBuffer)
            {
                verticalVelocity = Mathf.Sqrt(2f * config.gravity * config.jumpHeight);
                jumpConsumed = true;
                lastJumpPressed = float.NegativeInfinity;
            }
            verticalVelocity -= config.gravity * Time.deltaTime;
            controller.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            if (horizontalVelocity.sqrMagnitude > 0.04f)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(horizontalVelocity), 12f * Time.deltaTime);
        }
    }
}
