using UnityEngine;

public class PlayerMotionSolver
{
    private const float SMALL_NEGATIVE_Y = -2f;
    private const float GROUND_STICK_Y = -0.12f;
    private const float NUDGE_MIN_SECONDS = 0.06f;
    private const float NUDGE_EPSILON = 0.0001f;

    public event OnUsed OnJumpUsed;
    public event OnUsed OnDashUsed;

    private readonly CharacterController controller;

    private readonly float moveSpeed;
    private readonly float jumpForce;
    private readonly float gravity;
    private readonly float dashDistance;
    private readonly float dashDuration;
    private readonly float dashCooldown;

    private Vector3 _velocity;
    private Vector3 _dashStartPosition;
    private Vector3 _dashDirection;
    private Vector3 _additiveNudgeTotalWorld;
    private Vector3 _additiveNudgeAppliedWorld;

    private float _additiveNudgeDurationSeconds;
    private float _additiveNudgeElapsedSeconds;
    private float _lastDashTime = -Mathf.Infinity;
    private float _dashTimer;
    private float _postLandingGraceTimer = 0f;
    private float _movementSpeedMultiplier = 1f;
    private float _jumpForceMultiplier = 1f;

    private bool _wasGrounded;

    public PlayerMotionSolver(CharacterController controller, float moveSpeed, float jumpForce, float gravity, float dashDistance, float dashDuration, float dashCooldown)
    {
        this.controller = controller;
        this.moveSpeed = moveSpeed;
        this.jumpForce = jumpForce;
        this.gravity = gravity;
        this.dashDistance = dashDistance;
        this.dashDuration = dashDuration;
        this.dashCooldown = dashCooldown;
    }

    public float MovementSpeedMultiplier
    {
        get => _movementSpeedMultiplier;
        private set => _movementSpeedMultiplier = Mathf.Max(0.01f, value);
    }
    public float JumpForceMultiplier
    {
        get => _jumpForceMultiplier;
        private set => _jumpForceMultiplier = Mathf.Max(0.01f, value);
    }
    public Vector3 CurrentVelocity => _velocity;
    public bool IsGrounded => controller.isGrounded;

    public void SetMovementSpeedMultiplier(float multiplier) => MovementSpeedMultiplier = multiplier;
    
    public void SetJumpForceMultiplier(float multiplier) => JumpForceMultiplier = multiplier;

    public void ResetMovementSpeedMultiplier() => MovementSpeedMultiplier = 1f;
    
    public void ResetJumpForceMultiplier() => JumpForceMultiplier = 1f;
    
    public void ResetVerticalVelocity() => _velocity.y = 0f;

    public void BeginPostLandingGrace(float seconds) => _postLandingGraceTimer = Mathf.Max(_postLandingGraceTimer, seconds);
    
    public void BeginAdditiveNudge(Vector3 totalDisplacementWorld, float durationSeconds)
    {
        _additiveNudgeTotalWorld = totalDisplacementWorld;
        _additiveNudgeAppliedWorld = Vector3.zero;
        _additiveNudgeDurationSeconds = Mathf.Max(durationSeconds, NUDGE_MIN_SECONDS);
        _additiveNudgeElapsedSeconds = 0f;
    }

    public void UpdateMovement(Vector3 moveDirection, float deltaTime)
    {
        Vector3 horizontalVelocity;

        if (_dashTimer > 0f)
        {
            float dashSpeed = dashDistance / dashDuration;
            horizontalVelocity = _dashDirection * dashSpeed;

            float travelled = Vector3.Distance(controller.transform.position, _dashStartPosition);
            _dashTimer = (travelled >= dashDistance) ? 0f : Mathf.Max(0f, _dashTimer - deltaTime);

            _velocity.y = 0f;
        }
        else
        {
            float effectiveMoveSpeed = moveSpeed * MovementSpeedMultiplier;
            horizontalVelocity = moveDirection * effectiveMoveSpeed;

            if (_postLandingGraceTimer > 0f)
            {
                _postLandingGraceTimer = Mathf.Max(0f, _postLandingGraceTimer - deltaTime);

                if (controller.isGrounded && _velocity.y < 0f)
                    _velocity.y = 0f;
            }
            else
            {
                if (controller.isGrounded)
                {
                    if (!_wasGrounded)
                        _velocity.y = GROUND_STICK_Y;
                    else if (_velocity.y < GROUND_STICK_Y)
                        _velocity.y = GROUND_STICK_Y;
                }
                else
                {
                    _velocity.y += gravity * deltaTime;
                }
            }
        }

        _velocity.x = horizontalVelocity.x;
        _velocity.z = horizontalVelocity.z;
        
        Vector3 additiveStep = Vector3.zero;
        
        if (_additiveNudgeDurationSeconds > 0f && _additiveNudgeElapsedSeconds < _additiveNudgeDurationSeconds)
        {
            float t1 = Mathf.Clamp01(_additiveNudgeElapsedSeconds / _additiveNudgeDurationSeconds);
            float e1 = EaseOutCubic(t1);

            Vector3 targetApplied = _additiveNudgeTotalWorld * e1;
            
            additiveStep = targetApplied - _additiveNudgeAppliedWorld;
            _additiveNudgeAppliedWorld = targetApplied;
            _additiveNudgeElapsedSeconds += deltaTime;

            if ((_additiveNudgeTotalWorld - _additiveNudgeAppliedWorld).sqrMagnitude <= NUDGE_EPSILON * NUDGE_EPSILON)
                CancelAdditiveNudge();
        }

        controller.Move((_velocity * deltaTime) + additiveStep);
        _wasGrounded = controller.isGrounded;
    }

    public void TryJump()
    {
        if (!controller.isGrounded)
            return;

        float effectiveJumpForce = jumpForce * JumpForceMultiplier;
        _velocity.y = Mathf.Sqrt(effectiveJumpForce * SMALL_NEGATIVE_Y * gravity);

        OnJumpUsed?.Invoke();
    }

    public void TryDash()
    {
        if (Time.time < _lastDashTime + dashCooldown)
            return;

        if (dashDistance <= 0f || dashDuration <= 0f)
            return;

        _dashStartPosition = controller.transform.position;
        _dashDirection = controller.transform.forward.normalized;
        _dashTimer = dashDuration;
        _lastDashTime = Time.time;

        OnDashUsed?.Invoke();
    }
    
    private void CancelAdditiveNudge()
    {
        _additiveNudgeTotalWorld = Vector3.zero;
        _additiveNudgeAppliedWorld = Vector3.zero;
        _additiveNudgeDurationSeconds = 0f;
        _additiveNudgeElapsedSeconds = 0f;
    }
    
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}