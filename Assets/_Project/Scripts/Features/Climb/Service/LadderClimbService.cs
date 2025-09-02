using UnityEngine;

public class LadderClimbService : ILadderClimbService
{
    private const float REENTER_FALLBACK_SECONDS = 0.25f;

    public event OnClimbEnter OnClimbEntered;
    public event OnClimbExit OnClimbExited;
    public event OnClimbStateChange OnClimbStateChanged;
    public event OnClimbSpeedChange OnClimbSpeedChanged;

    private readonly CharacterController controller;
    private readonly Transform playerTransform;
    private readonly LayerMask ladderMask;
    private readonly LadderSettingsConfig config;

    private readonly LadderClimbState state;
    private readonly LadderClimbInputMapper inputMapper;
    private readonly LadderPlaneCoordinator planeCoordinator;
    private readonly LadderClimbKinematics kinematics;
    private readonly LadderClimbExitEvaluator exitEvaluator;

    public bool IsClimbing { get; private set; }
    public bool CanEnterNow => state.CanEnterNow;

    public LadderClimbService(CharacterController controller, Transform playerTransform, LayerMask ladderMask, LayerMask groundMask, LadderSettingsConfig config)
    {
        this.controller = controller;
        this.playerTransform = playerTransform;
        this.ladderMask = ladderMask;
        this.config = config;

        state = new LadderClimbState();
        inputMapper = new LadderClimbInputMapper();
        planeCoordinator = new LadderPlaneCoordinator(this.controller, this.playerTransform, this.config);
        kinematics = new LadderClimbKinematics(this.controller, this.config);
        exitEvaluator = new LadderClimbExitEvaluator(this.controller, groundMask, this.config);
    }

    void ILadderClimbService.TryEnter(Transform ladderFacing, Collider lateralBounds)
    {
        state.BeginClimb(ladderFacing, lateralBounds, controller.bounds.center.y);
        IsClimbing = true;

        planeCoordinator.AlignRotationToLadder(ladderFacing);
        planeCoordinator.SnapToDesiredPlaneOffset(ladderFacing);

        OnClimbEntered?.Invoke(ladderFacing);
        OnClimbStateChanged?.Invoke(true);
        OnClimbSpeedChanged?.Invoke(0f);
    }

    void ILadderClimbService.TryExit(Transform ladderFacing)
    {
        if (!CanEnterNow || state.CurrentLadderFacing != ladderFacing)
            return;

        ForceExit(false);
    }

    void ILadderClimbService.Tick(Vector3 moveDirectionWorld, float deltaTime)
    {
        if (state.ReenterBlockTimer > 0f)
            state.ReenterBlockTimer = Mathf.Max(0f, state.ReenterBlockTimer - deltaTime);

        if (!IsClimbing || state.CurrentLadderFacing == null)
            return;

        state.TimeSinceEnter += deltaTime;

        LadderClimbInput mapped = inputMapper.MapToLadderLocal(moveDirectionWorld, state.CurrentLadderFacing);

        kinematics.ApplyVertical(mapped.ClimbSigned, deltaTime);
        
        OnClimbSpeedChanged?.Invoke(Mathf.Abs(mapped.ClimbSigned) > 0.001f ? mapped.ClimbSigned : 0f);

        if (!kinematics.TryApplyLateral(state.CurrentLadderFacing, state.CurrentLateralBounds, mapped.Lateral, deltaTime))
        {
            ForceExit(false);
            
            return;
        }

        planeCoordinator.MaintainDesiredPlaneOffset(state.CurrentLadderFacing);

        if (mapped.ClimbSigned > 0.10f)
        {
            if (exitEvaluator.IsReadyForTopExit(state.EnterCenterY, state.TimeSinceEnter) && exitEvaluator.TrySoftTopExit(state.CurrentLadderFacing))
            {
                ForceExit(true);
                
                return;
            }
        }

        if (mapped.ClimbSigned < -0.10f && exitEvaluator.IsGroundClose())
        {
            ForceExit(false);
            
            return;
        }
    }
    
    private void ForceExit(bool withReenterBlock)
    {
        if (!IsClimbing)
            return;

        Transform preview = state.CurrentLadderFacing;

        IsClimbing = false;
        
        state.EndClimb();

        OnClimbStateChanged?.Invoke(false);
        OnClimbSpeedChanged?.Invoke(0f);
        OnClimbExited?.Invoke(preview);

        if (withReenterBlock)
            state.ReenterBlockTimer = Mathf.Max(0f, config != null ? config.ReenterBlockSeconds : REENTER_FALLBACK_SECONDS);
    }
}