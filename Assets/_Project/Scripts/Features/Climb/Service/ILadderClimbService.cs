using UnityEngine;

public interface ILadderClimbService
{
    public event OnClimbEnter OnClimbEntered;
    public event OnClimbExit OnClimbExited;
    public event OnClimbStateChange OnClimbStateChanged;
    public event OnClimbSpeedChange OnClimbSpeedChanged;
    
    public bool IsClimbing { get; }
    public bool CanEnterNow { get; }
    
    public void TryEnter(Transform ladderFacing, Collider lateralBounds);
    
    public void TryExit(Transform ladderFacing);
    
    public void Tick(Vector3 moveDirectionWorld, float deltaTime);
}