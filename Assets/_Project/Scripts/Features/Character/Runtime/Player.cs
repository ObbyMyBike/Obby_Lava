using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IPlayerMoveDirectionProvider
{
    private const string NAME_LAYER_GROUND = "Ground";
    private const string NAME_LAYER_DEFAULT = "Default";
    private const string MODEL_ROOT_NAME = "PlayerModel";
    private const float POST_EXIT_FORWARD_NUDGE = 0.08f;
    private const float POST_EXIT_UP_NUDGE = 0.05f;
    private const float POST_EXIT_BLEND_SECONDS_AIR = 0.18f;
    private const float POST_EXIT_BLEND_SECONDS_GROUNDED = 0.12f;
    private const float POST_EXIT_GRACE_SEC = 0.28f;

    [SerializeField] private PlayerDataConfig _dataConfig;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Transform _playerModelRoot;

    private IInput _input;
    private LadderClimb _ladder;
    private PushInteraction _pushInteraction;
    
    private CharacterController _controller;
    private PlayerAnimationPlayback _animationPlayback;
    private PlayerMotionSolver _movement;
    private PlayerRotator _rotator;
    private PushCaster _pushCaster;
    private AutoPush _autoPush;
    private PlayerLadderBridge _ladderBridge;
    private PlayerTrailUsed _trail;
    private Animator _animator;

    private Vector3 _currentMoveDirectionWorld;
    
    [Inject]
    public void Construct(IInput input, LadderClimb ladder, AutoPush autoPush, PushInteraction pushInteraction)
    {
        _input = input;
        _ladder = ladder;
        _autoPush = autoPush;
        _pushInteraction = pushInteraction;
    }

    public Transform PlayerModelRoot { get; private set; }
    public PlayerDataConfig DataConfig => _dataConfig;
    public PlayerTrailUsed Trail => _trail;
    public Vector3 CurrentMoveDirectionWorld => _currentMoveDirectionWorld;
    
    Vector3 IPlayerMoveDirectionProvider.CurrentMoveDirectionWorld => CurrentMoveDirectionWorld;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _movement = new PlayerMotionSolver(_controller, _dataConfig.MoveSpeed, _dataConfig.JumpForce, _dataConfig.Gravity, _dataConfig.DashDistance, _dataConfig.DashDuration, _dataConfig.DashCooldown);
        _pushCaster = new PushCaster(_controller, _pushInteraction, _dataConfig.PushRange, _dataConfig.PushRadius, _dataConfig.PushCooldown);
        _animationPlayback = new PlayerAnimationPlayback(_animator);
        _rotator = new PlayerRotator(transform, _dataConfig.RotationSpeed);
        _ladderBridge = new PlayerLadderBridge(_ladder, _input, _controller, _animationPlayback); 
        _trail = new PlayerTrailUsed(_trailRenderer);
        
        _trail.Disable();
        
        PlayerModelRoot = _playerModelRoot != null ? _playerModelRoot : transform.Find(MODEL_ROOT_NAME);

        if (PlayerModelRoot == null)
            Debug.LogWarning($"[Player] PlayerModelRoot not set and '{MODEL_ROOT_NAME}' not found under player!");
    }

    private void OnEnable()
    {
        if (_input != null)
        {
            _input.OnJumpPressed += _movement.TryJump;
            _input.OnDashPressed += _movement.TryDash;
            _input.OnPushPressed += _pushCaster.TryPush;
        }

        _movement.OnJumpUsed += _animationPlayback.PlayJump;
        _movement.OnDashUsed += _animationPlayback.PlayDash;
        _pushCaster.OnPushUsed += _animationPlayback.PlayPush;
        
        if (_autoPush != null)
            _autoPush.OnPushed += OnAutoPushAnimation;

        if (_ladder != null)
            _ladder.OnClimbExited += OnClimbExited;
        
        _ladderBridge?.Enable();
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnJumpPressed -= _movement.TryJump;
            _input.OnDashPressed -= _movement.TryDash;
            _input.OnPushPressed -= _pushCaster.TryPush;
        }

        _movement.OnJumpUsed -= _animationPlayback.PlayJump;
        _movement.OnDashUsed -= _animationPlayback.PlayDash;
        _pushCaster.OnPushUsed -= _animationPlayback.PlayPush;
        
        if (_autoPush != null)
            _autoPush.OnPushed -= OnAutoPushAnimation;

        if (_ladder != null)
            _ladder.OnClimbExited -= OnClimbExited;
        
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
    
    public void SetJumpMultiplier(float multiplier) => _movement.SetJumpForceMultiplier(multiplier);
    
    public void ResetJumpMultiplier() => _movement.ResetJumpForceMultiplier();

    public void SetMoveSpeedMultiplier(float multiplier) => _movement.SetMovementSpeedMultiplier(multiplier);

    public void ResetMoveSpeedMultiplier() => _movement.ResetMovementSpeedMultiplier();

    public void SetAnimationSpeedMultiplier(float multiplier) => _animationPlayback.SetAnimatorSpeedMultiplier(multiplier);

    public void ResetAnimationSpeedMultiplier() => _animationPlayback.ResetAnimatorSpeedMultiplier();

    private void OnClimbExited(Transform ladderFacing)
    {
        _movement.ResetVerticalVelocity();
        _movement.BeginPostLandingGrace(POST_EXIT_GRACE_SEC);

        if (ladderFacing == null)
            return;

        Bounds bounds = _controller.bounds;
        Vector3 feet = new Vector3(bounds.center.x, bounds.min.y + _controller.skinWidth + 0.01f, bounds.center.z);
        bool hasGround = Physics.Raycast(feet, Vector3.down, out _, 0.15f, LayerMask.GetMask(NAME_LAYER_DEFAULT, NAME_LAYER_GROUND), QueryTriggerInteraction.Ignore);

        Vector3 forwardToPlatform = -ladderFacing.forward;

        Vector3 targetNudge = hasGround ? (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) : (forwardToPlatform * POST_EXIT_FORWARD_NUDGE) + (Vector3.up * POST_EXIT_UP_NUDGE);

        float blendSeconds = hasGround ? POST_EXIT_BLEND_SECONDS_GROUNDED : POST_EXIT_BLEND_SECONDS_AIR;

        _movement.BeginAdditiveNudge(targetNudge, blendSeconds);
    }
    
    private void OnAutoPushAnimation()
    {
        if (_animationPlayback == null)
            return;

        _animationPlayback.PlayPush();
    }
}