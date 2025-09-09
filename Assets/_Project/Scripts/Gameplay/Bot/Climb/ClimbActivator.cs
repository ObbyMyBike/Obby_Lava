using UnityEngine;

public class ClimbActivator
{
    private readonly BotLadderBridge ladder;

    public ClimbActivator(BotLadderBridge ladder) => this.ladder = ladder;

    public bool TryBegin(Transform facing, Collider lateralBounds)
    {
        if (ladder == null || facing == null)
            return false;
        
        ladder.BeginClimb(facing, lateralBounds);
        
        return true;
    }

    public bool IsClimbing => ladder != null && ladder.IsClimbing;

    public void Tick(float delta) => ladder?.Tick(delta);
    
    public void TryExitIfGroundClose() => ladder?.ForceExitIfGroundClose();
}