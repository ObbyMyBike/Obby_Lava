using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataConfig", menuName = "Configs/PlayerData")]
public class PlayerDataConfig : ScriptableObject
{
    [Header("Economy Settings")]
    [SerializeField] private int _startingGold = 100;

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _secondsToDieInLava = 10f;
    [SerializeField] private float _regenPerSecondOutsideLava = 0f;
    
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 2.5f;
    [SerializeField] private float _gravity = -35f;

    [Header("Dash Settings")]
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 2f;

    [Header("Push Settings")]
    [SerializeField] private float _pushForce = 10f;
    [SerializeField] private float _pushRange = 1.5f;
    [SerializeField] private float _pushRadius = 0.5f;
    [SerializeField] private float _pushCooldown = 1f;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 720f;
    
    [Header("Climb Settings")]
    [SerializeField] private float _climbSpeed = 2.5f;
    
    public int StartingGold => _startingGold;
    public int MaxHealth => _maxHealth;
    public float SecondsToDieInLava => _secondsToDieInLava;
    public float RegenPerSecondOutsideLava => _regenPerSecondOutsideLava;
    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;
    public float Gravity => _gravity;
    public float DashDistance => _dashDistance;
    public float DashDuration => _dashDuration;
    public float DashCooldown => _dashCooldown;
    public float PushForce => _pushForce;
    public float PushRange => _pushRange;
    public float PushRadius => _pushRadius;
    public float PushCooldown => _pushCooldown;
    public float RotationSpeed => _rotationSpeed;
    public float ClimbSpeed => _climbSpeed;
}