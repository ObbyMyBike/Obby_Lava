using UnityEngine;

public class PlayerLadderBridge
{
    private const float PSEUDO_VELOCITY_MULTIPLIER = 3.0f;

    private readonly LadderClimb climb;
    private readonly IInput inputService;
    private readonly CharacterController characterController;
    private readonly AnimationPlayback animationPlayback;

    private Vector3 _currentMoveDirectionWorld;

    public Vector3 CurrentMoveDirectionWorld => _currentMoveDirectionWorld;

    public PlayerLadderBridge(LadderClimb climb, IInput inputService, CharacterController characterController, AnimationPlayback animationPlayback)
    {
        this.climb = climb;
        this.inputService = inputService;
        this.characterController = characterController;
        this.animationPlayback = animationPlayback;
    }

    public void Enable()
    {
        if (climb == null)
            return;

        climb.OnClimbStateChanged += HandleClimbStateChanged;
        climb.OnClimbSpeedChanged += HandleClimbSpeedChanged;
    }

    public void Disable()
    {
        if (climb == null)
            return;

        climb.OnClimbStateChanged -= HandleClimbStateChanged;
        climb.OnClimbSpeedChanged -= HandleClimbSpeedChanged;
    }

    public void Tick(float deltaTime)
    {
        _currentMoveDirectionWorld = inputService.MoveDirection;

        climb?.Tick(_currentMoveDirectionWorld, deltaTime);

        bool grounded = characterController != null && characterController.isGrounded;
        Vector3 pseudoVelocity = (climb != null && climb.IsClimbing) ? Vector3.zero : _currentMoveDirectionWorld * PSEUDO_VELOCITY_MULTIPLIER;

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