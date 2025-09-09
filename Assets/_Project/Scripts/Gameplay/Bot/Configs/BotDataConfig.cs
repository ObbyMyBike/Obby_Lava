using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Bots/Bot Data Config", fileName = "BotDataConfig")]
public class BotDataConfig : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 50;
    
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _rotationSpeed = 9f;

    [Header("Jump / Gravity")]
    [SerializeField] private float _jumpForce = 2.5f;
    [SerializeField] private float _gravity = -35f;

    [Header("Dash")]
    [SerializeField] private float _dashDistance = 1f;
    [SerializeField] private float _dashDuration = 1f;
    [SerializeField] private float _dashCooldown = 1.2f;

    [Header("AI Behaviour")]
    [SerializeField] private float _reachedDistance = 0.35f;
    [SerializeField] private float _jumpMinCooldown = 1f;
    [SerializeField] private float _failFallDownVelocity = -8f;
    [SerializeField] private float _randomFailKickForce = 3.5f;

    [Header("Crowd / Separation")]
    [SerializeField] private float _minDistanceBetweenBots = 1f;
    [SerializeField] private float _separationWeight = 1f;
    [SerializeField] private float _directionSqrEpsilon = 0.0001f;
    
    [Header("Environment")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundProbeRadius = 0.18f;
    [SerializeField] private float _groundProbeDistance = 0.25f;
    [SerializeField] private float _groundProbeRadiusMin = 0.05f;
    
    [Header("Climb (bots-specific)")]
    [SerializeField] private float _climbAutoInputUp = 1f;
    [SerializeField] private float _climbAnimMinAbsSpeed = 0.001f;

    [Header("Respawn / Checkpoints")]
    [SerializeField] private float _respawnProgressMinDelta = 0.01f;

    public int MaxHealth => _maxHealth;
    
    public LayerMask GroundMask => _groundMask;
    public float GroundProbeRadius => _groundProbeRadius;
    public float GroundProbeDistance => _groundProbeDistance;
    public float GroundProbeRadiusMin => _groundProbeRadiusMin;

    public float MoveSpeed => _moveSpeed;
    public float RotationSpeed => _rotationSpeed;
    public float JumpForce => _jumpForce;
    public float Gravity => _gravity;
    public float DashDistance => _dashDistance;
    public float DashDuration => _dashDuration;
    public float DashCooldown => _dashCooldown;

    public float ReachedDistance => _reachedDistance;
    public float JumpMinCooldown => _jumpMinCooldown;
    public float FailFallDownVelocity => _failFallDownVelocity;
    public float RandomFailKickForce => _randomFailKickForce;

    public float MinDistanceBetweenBots => _minDistanceBetweenBots;
    public float SeparationWeight => _separationWeight;
    public float DirectionSqrEpsilon => _directionSqrEpsilon;

    public float ClimbAutoInputUp => _climbAutoInputUp;
    public float ClimbAnimMinAbsSpeed => _climbAnimMinAbsSpeed;

    public float RespawnProgressMinDelta => _respawnProgressMinDelta;
}