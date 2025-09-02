using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IPlayerMoveDirectionProvider
{
    private const float POST_EXIT_FORWARD_NUDGE = 0.08f;
    private const float POST_EXIT_UP_NUDGE = 0.05f;
    private const float POST_EXIT_BLEND_SECONDS_AIR = 0.18f;
    private const float POST_EXIT_BLEND_SECONDS_GROUNDED = 0.12f;
    private const float POST_EXIT_GRACE_SEC = 0.28f;

    [FormerlySerializedAs("_data")] [SerializeField] private PlayerDataConfig _dataConfig;

    private IInput _input;
    private ILadderClimbService _ladder;
    private PlayerMotionSolver _movement;
    private ForwardPushCaster _forwardPushCaster;
    private PlayerRotator _rotator;
    private PlayerAnimationPlayback _animationPlayback;
    private CharacterController _controller;
    private PlayerLadderBridge _ladderBridge;
    private Animator _animator;

    private Vector3 _currentMoveDirectionWorld;
    private float _jumpMultiplier = 1f;

    [Inject]
    public void Construct(IInput input, ILadderClimbService ladder)
    {
        _input = input;
        _ladder = ladder;
    }

    public PlayerDataConfig DataConfig => _dataConfig;
    public Vector3 CurrentMoveDirectionWorld => _currentMoveDirectionWorld;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _movement = new PlayerMotionSolver(_controller, _dataConfig.MoveSpeed, _dataConfig.JumpForce, _dataConfig.Gravity, _dataConfig.DashDistance, _dataConfig.DashDuration, _dataConfig.DashCooldown);
        _forwardPushCaster = new ForwardPushCaster(_controller, _dataConfig.PushForce, _dataConfig.PushRange, _dataConfig.PushRadius, _dataConfig.PushCooldown);
        _animationPlayback = new PlayerAnimationPlayback(_animator);
        _rotator = new PlayerRotator(transform, _dataConfig.RotationSpeed);
        _ladderBridge = new PlayerLadderBridge(_ladder, _input, _controller, _animationPlayback); 
    }

    private void OnEnable()
    {
        if (_input != null)
        {
            _input.OnJumpPressed += _movement.TryJump;
            _input.OnDashPressed += _movement.TryDash;
            _input.OnPushPressed += _forwardPushCaster.TryPush;
        }

        _movement.OnJumpUsed += _animationPlayback.PlayJump;
        _movement.OnDashUsed += _animationPlayback.PlayDash;
        _forwardPushCaster.OnPushUsed += _animationPlayback.PlayForwardPush;

        if (_ladder != null)
            _ladder.OnClimbExited += HandleOnClimbExited;
        
        _ladderBridge?.Enable();
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnJumpPressed -= _movement.TryJump;
            _input.OnDashPressed -= _movement.TryDash;
            _input.OnPushPressed -= _forwardPushCaster.TryPush;
        }

        _movement.OnJumpUsed -= _animationPlayback.PlayJump;
        _movement.OnDashUsed -= _animationPlayback.PlayDash;
        _forwardPushCaster.OnPushUsed -= _animationPlayback.PlayForwardPush;

        if (_ladder != null)
            _ladder.OnClimbExited -= HandleOnClimbExited;
        
        _ladderBridge?.Disable();
    }

    private void Update()
    {
        _ladderBridge?.Tick(Time.deltaTime);
        _currentMoveDirectionWorld = _ladderBridge != null ? _ladderBridge.CurrentMoveDirectionWorld : (_input != null ? _input.MoveDirection : Vector3.zero);
        
        if (_ladder != null && _ladder.IsClimbing)
        {
            _animationPlayback.UpdateAnimation(Vector3.zero, _movement.IsGrounded);
            
            return;
        }
        
        _movement.UpdateMovement(_currentMoveDirectionWorld, Time.deltaTime);
        _rotator.UpdateRotation(_currentMoveDirectionWorld, Time.deltaTime);
        _animationPlayback.UpdateAnimation(_movement.CurrentVelocity, _movement.IsGrounded);
    }

    Vector3 IPlayerMoveDirectionProvider.CurrentMoveDirectionWorld => CurrentMoveDirectionWorld;
    
    public void SetJumpMultiplier(float multiplier)
    {
        _jumpMultiplier = multiplier;
    }

    private void HandleOnClimbExited(Transform ladderFacing)
    {
        _movement.ResetVerticalVelocity();
        _movement.BeginPostLandingGrace(POST_EXIT_GRACE_SEC);

        if (ladderFacing == null)
            return;

        Bounds bounds = _controller.bounds;
        Vector3 feet = new Vector3(bounds.center.x, bounds.min.y + _controller.skinWidth + 0.01f, bounds.center.z);
        bool hasGround = Physics.Raycast(feet, Vector3.down, out _, 0.15f, LayerMask.GetMask("Default", "Ground"), QueryTriggerInteraction.Ignore);

        Vector3 forwardToPlatform = -ladderFacing.forward;

        Vector3 targetNudge = hasGround ? (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) : (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) + (Vector3.up * POST_EXIT_UP_NUDGE);

        float blendSeconds = hasGround ? POST_EXIT_BLEND_SECONDS_GROUNDED : POST_EXIT_BLEND_SECONDS_AIR;

        _movement.BeginAdditiveNudge(targetNudge, blendSeconds);
    }
    
    // private IInput _input;
    // private PlayerMotionSolver _movement;
    // private ForwardPushCaster _forwardPushCaster;
    // private PlayerRotator _rotator;
    // private PlayerAnimationPlayback _animationPlayback;
    // //private LadderClimbMotor _ladder;
    // private CharacterController _controller;
    // private Animator _animator;
    //
    // private float _jumpMultiplier = 1f;
    //
    // [Inject]
    // public void Construct(IInput input)
    // {
    //     _input = input;
    // }
    //
    // public PlayerData Data => _data;
    // // public bool IsClimbing => _ladder != null && _ladder.IsClimbing;
    // // public bool CanEnterLadderNow => _ladder != null && _ladder.CanEnterNow;
    // public Vector3 CurrentMoveDirectionWorld { get; private set; }
    //
    // private void Awake()
    // {
    //     _animator = GetComponent<Animator>();
    //     _controller = GetComponent<CharacterController>();
    //
    //     _movement = new PlayerMotionSolver(_controller, _data.MoveSpeed, _data.JumpForce, _data.Gravity, _data.DashDistance, _data.DashDuration, _data.DashCooldown);
    //     _forwardPushCaster = new ForwardPushCaster(_controller, _data.PushForce, _data.PushRange, _data.PushRadius, _data.PushCooldown);
    //     _animationPlayback = new PlayerAnimationPlayback(_animator);
    //     _rotator = new PlayerRotator(transform, _data.RotationSpeed);
    //
    //     LayerMask ladderMask = LayerMask.GetMask("Ladder");
    //     LayerMask groundMask = LayerMask.GetMask("Default", "Ground");
    //
    //     //_ladder = new LadderClimbMotor(_controller, transform, _data.ClimbSpeed, _animationPlayback, ladderMask, groundMask);
    // }
    //
    // private void OnEnable()
    // {
    //     _input.OnJumpPressed += _movement.TryJump;
    //     _input.OnDashPressed += _movement.TryDash;
    //     _input.OnPushPressed += _forwardPushCaster.TryPush;
    //
    //     _movement.OnJumpUsed += _animationPlayback.PlayJump;
    //     _movement.OnDashUsed += _animationPlayback.PlayDash;
    //     _forwardPushCaster.OnPushUsed += _animationPlayback.PlayForwardPush;
    //
    //     // if (_ladder != null)
    //     //     _ladder.OnClimbExited += HandleOnClimbExited;
    // }
    //
    // private void OnDisable()
    // {
    //     _input.OnJumpPressed -= _movement.TryJump;
    //     _input.OnDashPressed -= _movement.TryDash;
    //     _input.OnPushPressed -= _forwardPushCaster.TryPush;
    //
    //     _movement.OnJumpUsed -= _animationPlayback.PlayJump;
    //     _movement.OnDashUsed -= _animationPlayback.PlayDash;
    //     _forwardPushCaster.OnPushUsed -= _animationPlayback.PlayForwardPush;
    //
    //     // if (_ladder != null)
    //     //     _ladder.OnClimbExited -= HandleOnClimbExited;
    // }
    //
    // private void Update()
    // {
    //     CurrentMoveDirectionWorld = _input.MoveDirection;
    //
    //     //_ladder?.UpdateNonClimbingStateTimers(Time.deltaTime);
    //
    //     //bool climbingNow = IsClimbing;
    //
    //     // if (climbingNow)
    //     // {
    //     //     //_ladder.Tick(CurrentMoveDirectionWorld, Time.deltaTime);
    //     //     _animationPlayback.UpdateAnimation(Vector3.zero, _movement.IsGrounded);
    //     // }
    //     // else
    //     // {
    //     //     _movement.UpdateMovement(CurrentMoveDirectionWorld, Time.deltaTime);
    //     //     _rotator.UpdateRotation(CurrentMoveDirectionWorld, Time.deltaTime);
    //     //     _animationPlayback.UpdateAnimation(_movement.CurrentVelocity, _movement.IsGrounded);
    //     // }
    // }
    //
    // public void SetJumpMultiplier(float multiplier)
    // {
    //     _jumpMultiplier = multiplier;
    // }
    //
    // public void BeginClimb(Transform ladderFacing)
    // {
    //     // if (!IsClimbing && CanEnterLadderNow)
    //     //     _ladder?.Enter(ladderFacing);
    // }
    //
    // public void EndClimb(Transform ladderFacing)
    // {
    //     //_ladder?.Exit(ladderFacing);
    // }
    //
    // private void HandleOnClimbExited(Transform ladderFacing)
    // {
    //     _movement.ResetVerticalVelocity();
    //     _movement.BeginPostLandingGrace(POST_EXIT_GRACE_SEC);
    //
    //     if (ladderFacing == null)
    //         return;
    //     
    //     Bounds b = _controller.bounds;
    //     Vector3 feet = new Vector3(b.center.x, b.min.y + _controller.skinWidth + 0.01f, b.center.z);
    //     bool hasGround = Physics.Raycast(feet, Vector3.down, out _, 0.15f, LayerMask.GetMask("Default", "Ground"), QueryTriggerInteraction.Ignore);
    //
    //     Vector3 forwardToPlatform = -ladderFacing.forward;
    //
    //     Vector3 targetNudge = hasGround ? (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) : (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) + (Vector3.up * POST_EXIT_UP_NUDGE);
    //
    //     float blendSeconds = hasGround ? POST_EXIT_BLEND_SECONDS_GROUNDED : POST_EXIT_BLEND_SECONDS_AIR;
    //     
    //     _movement.BeginAdditiveNudge(targetNudge, blendSeconds);
    // }

    // private void OnDrawGizmos()
    // {
    //     if (_forwardPushCaster != null)
    //         _forwardPushCaster.DrawGizmos();
    // }
}