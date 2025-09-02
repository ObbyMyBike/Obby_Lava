using UnityEngine;

public class PlayerAnimationPlayback
{
    private const int BASE_LAYER = 0;
    private const int UPPER_BODY_LAYER = 1;
    private const float LAYER_FADE = 0.12f;
    private const float RESTART_AT_NORM_TIME = 0.98f;
    private const float DEAD_ZONE = 0.10f;

    private const string SPEED_PARAMETER = "Speed";
    private const string GROUNDED_PARAMETER = "IsGrounded";
    private const string JUMP_PARAMETER = "Jump";
    private const string DASH_PARAMETER = "Dash";
    private const string PUSH_PARAMETER = "Push";
    private const string CLIMB_BOOL_PARAMETER = "IsClimbing";
    private const string CLIMB_CYCLE_PARAMETER = "ClimbSpeed";
    private const string CLIMB_SIGNED_PARAMETER = "ClimbSigned";

    private static readonly int SpeedHash = Animator.StringToHash(SPEED_PARAMETER);
    private static readonly int IsGroundedHash = Animator.StringToHash(GROUNDED_PARAMETER);
    private static readonly int JumpTriggerHash = Animator.StringToHash(JUMP_PARAMETER);
    private static readonly int DashTriggerHash = Animator.StringToHash(DASH_PARAMETER);
    private static readonly int PushTriggerHash = Animator.StringToHash(PUSH_PARAMETER);
    private static readonly int ClimbBoolHash = Animator.StringToHash(CLIMB_BOOL_PARAMETER);
    private static readonly int ClimbCycleHash = Animator.StringToHash(CLIMB_CYCLE_PARAMETER);
    private static readonly int ClimbSignedHash = Animator.StringToHash(CLIMB_SIGNED_PARAMETER);

    private readonly Animator animator;

    private ClimbPhaseType _phaseType = ClimbPhaseType.None;
    private float _lastSigned = 0f;
    private float _upperBodyTargetWeight = 0f;
    private float _upperBodyWeight = 0f;
    private float _upperBodyFadeVel;

    private bool _hasClimbSigned;
    private bool _hasClimbCycle;
    private bool _isClimbing = false;

    public PlayerAnimationPlayback(Animator animator)
    {
        this.animator = animator;

        RefreshAnimatorParamsPresence();

        animator.SetLayerWeight(UPPER_BODY_LAYER, 0f);
    }

    public void UpdateAnimation(Vector3 velocity, bool isGrounded)
    {
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

        animator.SetFloat(SpeedHash, horizontalSpeed);
        animator.SetBool(IsGroundedHash, isGrounded);

        var state = animator.GetCurrentAnimatorStateInfo(UPPER_BODY_LAYER);
        bool inPush = state.tagHash == PushTriggerHash && state.normalizedTime < 0.99f;
        
        if (inPush)
            _upperBodyTargetWeight = 1f;
        else if (_upperBodyTargetWeight > 0f && state.normalizedTime >= 0.99f && !animator.IsInTransition(UPPER_BODY_LAYER))
            _upperBodyTargetWeight = 0f;

        _upperBodyWeight = Mathf.SmoothDamp(_upperBodyWeight, _upperBodyTargetWeight, ref _upperBodyFadeVel, LAYER_FADE);
        animator.SetLayerWeight(UPPER_BODY_LAYER, _upperBodyWeight);
    }

    public void PlayJump() => animator.SetTrigger(JumpTriggerHash);

    public void PlayDash() => animator.SetTrigger(DashTriggerHash);

    public void PlayPush()
    {
        animator.ResetTrigger(PushTriggerHash);
        animator.SetTrigger(PushTriggerHash);
        
        _upperBodyTargetWeight = 1f;
    }

    public void SetClimbState(bool isClimbing)
    {
        _isClimbing = isClimbing;

        animator.SetBool(ClimbBoolHash, isClimbing);

        if (!isClimbing)
        {
            _phaseType = ClimbPhaseType.None;

            if (_hasClimbSigned)
                animator.SetFloat(ClimbSignedHash, 0f);

            if (_hasClimbCycle)
                animator.SetFloat(ClimbCycleHash, 0f);
        }
        else
        {
            if (_hasClimbSigned)
                animator.SetFloat(ClimbSignedHash, 0f);

            if (_hasClimbCycle)
                animator.SetFloat(ClimbCycleHash, 0f);

            _phaseType = ClimbPhaseType.Idle;
        }
    }

    public void SetClimbCycleSpeed(float cycleSpeed01)
    {
        if (_hasClimbCycle)
            animator.SetFloat(ClimbCycleHash, Mathf.Clamp01(cycleSpeed01));
    }

    public void SetClimbSignedSpeed(float signedSpeed01)
    {
        _lastSigned = Mathf.Clamp(signedSpeed01, -1f, 1f);

        if (Mathf.Abs(_lastSigned) <= DEAD_ZONE)
        {
            if (_hasClimbSigned)
                animator.SetFloat(ClimbSignedHash, 0f);

            _phaseType = _isClimbing ? ClimbPhaseType.Idle : ClimbPhaseType.None;

            return;
        }

        if (_hasClimbSigned)
            animator.SetFloat(ClimbSignedHash, _lastSigned);

        ClimbPhaseType newPhaseType = _lastSigned > 0f ? ClimbPhaseType.Up : ClimbPhaseType.Down;

        if (_isClimbing && newPhaseType == _phaseType)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(BASE_LAYER);
            AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(BASE_LAYER);
            bool isLoop = clips != null && clips.Length > 0 && clips[0].clip != null && clips[0].clip.isLooping;

            if (!isLoop && stateInfo.normalizedTime >= RESTART_AT_NORM_TIME)
                animator.Play(stateInfo.fullPathHash, BASE_LAYER, 0f);
        }

        _phaseType = newPhaseType;
    }

    private void RefreshAnimatorParamsPresence()
    {
        _hasClimbSigned = HasFloatParam(ClimbSignedHash);
        _hasClimbCycle = HasFloatParam(ClimbCycleHash);
    }

    private bool HasFloatParam(int hash)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Float && parameter.nameHash == hash)
                return true;
        }

        return false;
    }
}