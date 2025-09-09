using UnityEngine;

public class LadderClimb
{
    private const float REENTER_FALLBACK_SECONDS = 0.25f;

    public event OnClimbEnter OnClimbEntered;
    public event OnClimbExit OnClimbExited;
    public event OnClimbStateChange OnClimbStateChanged;
    public event OnClimbSpeedChange OnClimbSpeedChanged;

    private readonly SpawnedPlayerAccessor accessor;
    private readonly LayerMask ladderMask;
    private readonly LayerMask groundMask;
    private readonly LadderSettingsConfig config;
    
    private readonly LadderClimbState state = new LadderClimbState();
    private readonly LadderClimbInputMapper inputMapper = new LadderClimbInputMapper();
    
    private CharacterController _controller;
    private LadderPlaneCoordinator _planeCoordinator;
    private LadderClimbKinematics _kinematics;
    private LadderClimbExitEvaluator _exitEvaluator;

    public bool IsClimbing { get; private set; }
    public bool CanEnterNow => state.CanEnterNow;

    public LadderClimb(SpawnedPlayerAccessor accessor, LayerMask ladderMask, LayerMask groundMask, LadderSettingsConfig config)
    {
        this.accessor = accessor;
        this.ladderMask = ladderMask;
        this.groundMask = groundMask;
        this.config = config;
    }

    public void TryEnter(Transform ladderFacing, Collider lateralBounds)
    {
        if (!EnsureInitialized())
            return;
        
        state.BeginClimb(ladderFacing, lateralBounds, _controller.bounds.center.y);
        IsClimbing = true;

        _planeCoordinator.AlignRotationToLadder(ladderFacing);
        _planeCoordinator.SnapToDesiredPlaneOffset(ladderFacing);

        OnClimbEntered?.Invoke(ladderFacing);
        OnClimbStateChanged?.Invoke(true);
        OnClimbSpeedChanged?.Invoke(0f);
    }

    public void TryExit(Transform ladderFacing)
    {
        if (!CanEnterNow || state.CurrentLadderFacing != ladderFacing)
            return;

        ForceExit(false);
    }

    public void Tick(Vector3 moveDirectionWorld, float deltaTime)
    {
        if (state.ReenterBlockTimer > 0f)
            state.ReenterBlockTimer = Mathf.Max(0f, state.ReenterBlockTimer - deltaTime);

        if (!IsClimbing || state.CurrentLadderFacing == null)
            return;

        if (!EnsureInitialized())
        {
            ForceExit(false);
            
            return;
        }
        
        state.TimeSinceEnter += deltaTime;

        LadderClimbInput mapped = inputMapper.MapToLadderLocal(moveDirectionWorld, state.CurrentLadderFacing);

        _kinematics.ApplyVertical(mapped.ClimbSigned, deltaTime);
        
        OnClimbSpeedChanged?.Invoke(Mathf.Abs(mapped.ClimbSigned) > 0.001f ? mapped.ClimbSigned : 0f);

        if (!_kinematics.TryApplyLateral(state.CurrentLadderFacing, state.CurrentLateralBounds, mapped.Lateral, deltaTime))
        {
            ForceExit(false);
            
            return;
        }

        _planeCoordinator.MaintainDesiredPlaneOffset(state.CurrentLadderFacing);

        if (mapped.ClimbSigned > 0.10f)
        {
            if (_exitEvaluator.IsReadyForTopExit(state.EnterCenterY, state.TimeSinceEnter) && _exitEvaluator.TrySoftTopExit(state.CurrentLadderFacing))
            {
                ForceExit(true);
                
                return;
            }
        }

        if (mapped.ClimbSigned < -0.10f && _exitEvaluator.IsGroundClose())
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
    
    private bool EnsureInitialized()
    {
        if (_controller != null)
            return true;

        _controller = accessor.CharacterController;
        
        if (_controller == null)
            return false;

        _planeCoordinator = new LadderPlaneCoordinator(_controller, config);
        _kinematics = new LadderClimbKinematics(_controller, config);
        _exitEvaluator = new LadderClimbExitEvaluator(_controller, groundMask, config);
        
        return true;
    }
}