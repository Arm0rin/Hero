using UnityEngine;

namespace Hero.Player
{
    [CreateAssetMenu(menuName = "HERO/Player Movement Config")]
    public sealed class PlayerMovementConfig : ScriptableObject
    {
        [Min(0.1f)] public float moveSpeed = 5.5f;
        [Min(0.1f)] public float acceleration = 24f;
        [Range(0f, 1f)] public float airControl = 0.7f;
        [Min(0.1f)] public float jumpHeight = 1.95f;
        [Min(0.1f)] public float gravity = 22f;
        [Min(0f)] public float coyoteTime = 0.12f;
        [Min(0f)] public float jumpBuffer = 0.12f;
    }
}
