using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayback
{
    private const int BASE_LAYER = 0;
    private const int UPPER_BODY_LAYER = 1;
    private const float LAYER_FADE = 0.12f;
    private const float RESTART_AT_NORM_TIME = 0.98f;
    private const float DEAD_ZONE = 0.10f;
    private const float MIN_ANIMATOR_SPEED = 0.1f;
    private const float DEFAULT_ANIMATOR_SPEED = 1f;
    
    private const string SPEED_PARAMETER = "Speed";
    private const string GROUNDED_PARAMETER = "IsGrounded";
    private const string JUMP_PARAMETER = "Jump";
    private const string DASH_PARAMETER = "Dash";
    private const string PUSH_PARAMETER = "Push";
    private const string PUSH_STATE_TAG = "Push";
    private const string PUSH_STATE_FULL_PATH = "UpperBodyLayer.Push";
    private const string CLIMB_BOOL_PARAMETER = "IsClimbing";
    private const string CLIMB_CYCLE_PARAMETER = "ClimbSpeed";
    private const string CLIMB_SIGNED_PARAMETER = "ClimbSigned";

    private static readonly int SpeedHash = Animator.StringToHash(SPEED_PARAMETER);
    private static readonly int IsGroundedHash = Animator.StringToHash(GROUNDED_PARAMETER);
    private static readonly int JumpTriggerHash = Animator.StringToHash(JUMP_PARAMETER);
    private static readonly int DashTriggerHash = Animator.StringToHash(DASH_PARAMETER);
    private static readonly int PushTriggerHash = Animator.StringToHash(PUSH_PARAMETER);
    private static readonly int PushStateHash = Animator.StringToHash(PUSH_STATE_FULL_PATH);
    private static readonly int ClimbBoolHash = Animator.StringToHash(CLIMB_BOOL_PARAMETER);
    private static readonly int ClimbCycleHash = Animator.StringToHash(CLIMB_CYCLE_PARAMETER);
    private static readonly int ClimbSignedHash = Animator.StringToHash(CLIMB_SIGNED_PARAMETER);

    private readonly Animator animator;

    private RuntimeAnimatorController _cachedController;
    private ClimbPhaseType _phaseType = ClimbPhaseType.None;
    private HashSet<int> _animatorFloatParams;
    
    private int _cachedLayerCount = -1;
    private float _globalAnimatorSpeedMultiplier = DEFAULT_ANIMATOR_SPEED;
    private float _lastSigned = 0f;
    private float _upperBodyTargetWeight = 0f;
    private float _upperBodyWeight = 0f;
    private float _upperBodyFadeVel;

    private bool _hasUpperBodyLayer;
    private bool _hasClimbSigned;
    private bool _hasClimbCycle;
    private bool _isClimbing = false;
    
    public AnimationPlayback(Animator animator)
    {
        this.animator = animator;
        
        if (this.animator == null)
            return;
        
        DetectControllerSwap(true);
        
        _hasUpperBodyLayer = this.animator.layerCount > UPPER_BODY_LAYER;
        
        animator.SetLayerWeight(UPPER_BODY_LAYER, 0f);
        animator.speed = DEFAULT_ANIMATOR_SPEED;
    }
    
    public void UpdateAnimation(Vector3 velocity, bool isGrounded)
    {
        DetectControllerSwap();
        
        if (animator == null)
            return;
        
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat(SpeedHash, horizontalSpeed);
        animator.SetBool(IsGroundedHash, isGrounded);
        
        if (_hasUpperBodyLayer)
        {
            _upperBodyWeight = Mathf.SmoothDamp(_upperBodyWeight, _upperBodyTargetWeight, ref _upperBodyFadeVel, LAYER_FADE);
            animator.SetLayerWeight(UPPER_BODY_LAYER, _upperBodyWeight);
        }
    }

    public void PlayJump()
    {
        if (animator == null)
            return;
        
        animator.SetTrigger(JumpTriggerHash);
    }

    public void PlayDash()
    {
        if (animator == null)
            return;
        
        animator.SetTrigger(DashTriggerHash);
    }

    public void PlayPush()
    {
        if (animator == null)
            return;
        
        if (_hasUpperBodyLayer)
        {
            _upperBodyTargetWeight = 1f;
            _upperBodyWeight = 1f;
            
            animator.SetLayerWeight(UPPER_BODY_LAYER, 1f);

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(UPPER_BODY_LAYER);
            bool inPushNow = state.IsTag(PUSH_STATE_TAG);

            if (inPushNow)
            {
                animator.Play(PushStateHash, UPPER_BODY_LAYER, 0f);
            }
            else
            {
                animator.ResetTrigger(PushTriggerHash);
                animator.SetTrigger(PushTriggerHash);
            }
        }
        else
        {
            animator.ResetTrigger(PushTriggerHash);
            animator.SetTrigger(PushTriggerHash);
        }
    }

    public void SetClimbState(bool isClimbing)
    {
        DetectControllerSwap();
        
        if (animator == null)
            return;

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
        DetectControllerSwap();
        
        if (animator == null)
            return;
       
        if (_hasClimbCycle)
            animator.SetFloat(ClimbCycleHash, Mathf.Clamp01(cycleSpeed01));
    }

    public void SetClimbSignedSpeed(float signedSpeed01)
    {
        DetectControllerSwap();
        
        if (animator == null)
            return;
        
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

    public void SetAnimatorSpeedMultiplier(float multiplier)
    {
        if (animator == null)
            return;
        
        _globalAnimatorSpeedMultiplier = Mathf.Max(MIN_ANIMATOR_SPEED, multiplier);
        animator.speed = _globalAnimatorSpeedMultiplier;
    }

    public void ResetAnimatorSpeedMultiplier()
    {
        if (animator == null)
            return;
        
        _globalAnimatorSpeedMultiplier = DEFAULT_ANIMATOR_SPEED;
        animator.speed = DEFAULT_ANIMATOR_SPEED;
    }
    
    public void RefreshAnimatorParamsPresence()
    {
        _hasClimbSigned = HasFloatParam(ClimbSignedHash);
        _hasClimbCycle = HasFloatParam(ClimbCycleHash);
    }
    
    private void DetectControllerSwap(bool logOnChange = false)
    {
        if (animator == null)
            return;

        var controller = animator.runtimeAnimatorController;
        var layers = animator.layerCount;

        if (controller != _cachedController || layers != _cachedLayerCount)
        {
            _cachedController = controller;
            _cachedLayerCount = layers;
            
            _hasUpperBodyLayer = layers > UPPER_BODY_LAYER;
            
            RefreshAnimatorParamsPresence();
        }
    }
    

    private bool HasFloatParam(int hash)
    {
        if (ReferenceEquals(animator, null))
            return false;
        
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Float && parameter.nameHash == hash)
                return true;
        }

        return false;
    }
}