using UnityEngine;

public class BotLadderBridge
{
    private readonly CharacterController controller;
    private readonly AnimationPlayback animation;
    private readonly LadderSettingsConfig ladderConfig;
    private readonly BotDataConfig botConfig;
    private readonly LayerMask groundMask;

    private readonly LadderClimbKinematics kinematics;
    private readonly LadderPlaneCoordinator plane;
    private readonly LadderClimbExitEvaluator exitEval;

    private Transform _currentFacing;
    private Collider _currentBounds;

    private float _enterCenterY;
    private float _timeSinceEnter;
    private bool _isClimbing;

    public BotLadderBridge(CharacterController controller, AnimationPlayback animation, LadderSettingsConfig ladderConfig, BotDataConfig botConfig, LayerMask groundMask)
    {
        this.controller = controller;
        this.animation = animation;
        this.ladderConfig = ladderConfig;
        this.botConfig = botConfig;
        this.groundMask = groundMask;

        kinematics = new LadderClimbKinematics(this.controller, this.ladderConfig);
        plane = new LadderPlaneCoordinator(this.controller, this.ladderConfig);
        exitEval = new LadderClimbExitEvaluator(this.controller, this.groundMask, this.ladderConfig);
    }
    
    public bool IsClimbing => _isClimbing;

    public void BeginClimb(Transform facing, Collider lateralBounds)
    {
        if (controller == null || ladderConfig == null || facing == null)
            return;

        _currentFacing = facing;
        _currentBounds = lateralBounds;
        _enterCenterY = controller.bounds.center.y;
        _timeSinceEnter = 0f;
        _isClimbing = true;

        plane.AlignRotationToLadder(_currentFacing);
        plane.SnapToDesiredPlaneOffset(_currentFacing);

        animation.RefreshAnimatorParamsPresence();
        animation.SetClimbState(true);
        animation.SetClimbCycleSpeed(0f);
        animation.SetClimbSignedSpeed(0f);
    }

    public void Tick(float deltaTime)
    {
        if (!_isClimbing || _currentFacing == null)
            return;

        _timeSinceEnter += deltaTime;
        
        float climbInput = botConfig.ClimbAutoInputUp;
        float minAbsForAnim = botConfig.ClimbAnimMinAbsSpeed;
        
        kinematics.ApplyVertical(climbInput, deltaTime);
        animation.SetClimbSignedSpeed(climbInput);
        animation.SetClimbCycleSpeed(Mathf.Abs(climbInput) >= minAbsForAnim ? Mathf.Abs(climbInput) : 0f);
        plane.MaintainDesiredPlaneOffset(_currentFacing);
        
        if (exitEval.IsReadyForTopExit(_enterCenterY, _timeSinceEnter) && exitEval.TrySoftTopExit(_currentFacing))
            EndClimb(withReenterBlock:false);
    }

    public void ForceExitIfGroundClose()
    {
        if (!_isClimbing)
            return;

        if (exitEval.IsGroundClose())
            EndClimb(withReenterBlock:false);
    }

    private void EndClimb(bool withReenterBlock)
    {
        _isClimbing = false;
        _currentFacing = null;
        _currentBounds = null;

        animation.SetClimbState(false);
        animation.SetClimbCycleSpeed(0f);
        animation.SetClimbSignedSpeed(0f);
    }
}