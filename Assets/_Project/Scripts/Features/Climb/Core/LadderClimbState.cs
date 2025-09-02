using UnityEngine;

public class LadderClimbState
{
    private Transform _currentLadderFacing;
    private Collider _currentLateralBounds;
    private float _reenterBlockTimer;
    private float _enterCenterY;
    private float _timeSinceEnter;
    
    public Transform CurrentLadderFacing { get => _currentLadderFacing; private set => _currentLadderFacing = value; }
    public Collider CurrentLateralBounds { get => _currentLateralBounds; private set => _currentLateralBounds = value; }
    public float ReenterBlockTimer { get => _reenterBlockTimer; set => _reenterBlockTimer = value; }
    public float EnterCenterY { get => _enterCenterY; set => _enterCenterY = value; }
    public float TimeSinceEnter { get => _timeSinceEnter; set => _timeSinceEnter = value; }

    public bool CanEnterNow => _reenterBlockTimer <= 0f;

    public void BeginClimb(Transform ladderFacing, Collider lateralBounds, float enterCenterY)
    {
        _currentLadderFacing = ladderFacing;
        _currentLateralBounds = lateralBounds;
        _enterCenterY = enterCenterY;
        _timeSinceEnter = 0f;
    }

    public void EndClimb()
    {
        _currentLadderFacing = null;
        _currentLateralBounds = null;
    }
}