using UnityEngine;

public class PlayerLadderBridge
{
    private const float PSEUDO_VELOCITY_MULTIPLIER = 3.0f;

    private readonly ILadderClimbService climbService;
    private readonly IInput inputService;
    private readonly CharacterController characterController;
    private readonly PlayerAnimationPlayback animationPlayback;

    private Vector3 _currentMoveDirectionWorld;

    public Vector3 CurrentMoveDirectionWorld => _currentMoveDirectionWorld;

    public PlayerLadderBridge(ILadderClimbService climbService, IInput inputService, CharacterController characterController, PlayerAnimationPlayback animationPlayback)
    {
        this.climbService = climbService;
        this.inputService = inputService;
        this.characterController = characterController;
        this.animationPlayback = animationPlayback;
    }

    public void Enable()
    {
        if (climbService == null)
            return;

        climbService.OnClimbStateChanged += HandleClimbStateChanged;
        climbService.OnClimbSpeedChanged += HandleClimbSpeedChanged;
    }

    public void Disable()
    {
        if (climbService == null)
            return;

        climbService.OnClimbStateChanged -= HandleClimbStateChanged;
        climbService.OnClimbSpeedChanged -= HandleClimbSpeedChanged;
    }

    public void Tick(float deltaTime)
    {
        _currentMoveDirectionWorld = inputService.MoveDirection;

        climbService?.Tick(_currentMoveDirectionWorld, deltaTime);

        bool grounded = characterController != null && characterController.isGrounded;
        Vector3 pseudoVelocity = (climbService != null && climbService.IsClimbing) ? Vector3.zero : _currentMoveDirectionWorld * PSEUDO_VELOCITY_MULTIPLIER;

        animationPlayback.UpdateAnimation(pseudoVelocity, grounded);
    }

    private void HandleClimbStateChanged(bool isEnabled)
    {
        animationPlayback.SetClimbState(isEnabled);

        if (!isEnabled)
        {
            animationPlayback.SetClimbCycleSpeed(0f);
            animationPlayback.SetClimbSignedSpeed(0f);
        }
    }

    private void HandleClimbSpeedChanged(float signedSpeed)
    {
        float abs = Mathf.Abs(signedSpeed);
        
        animationPlayback.SetClimbSignedSpeed(signedSpeed);
        animationPlayback.SetClimbCycleSpeed(abs);
    }
}