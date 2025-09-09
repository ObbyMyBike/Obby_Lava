using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private const float MIN_WAIT_SECONDS = 0f;
    private const float MAX_WAIT_SECONDS = 60f;
    private const float MIN_FAIL_PROBABILITY = 0f;
    private const float MAX_FAIL_PROBABILITY = 1f;
    
    [Header("Behaviour")]
    [SerializeField, Range(MIN_FAIL_PROBABILITY, MAX_FAIL_PROBABILITY)] private float _failProbability = 0f;
    [SerializeField, Range(MIN_WAIT_SECONDS, MAX_WAIT_SECONDS)] private float _waitBeforeProceedSeconds = 0f;
    [SerializeField] private bool _requireJump = false;
    
    [Header("Climb (optional)")]
    [SerializeField] private bool _requireClimb = false;
    [SerializeField] private Transform _climbFacing;
    [SerializeField] private Collider _climbLateralBounds;
    
    [Header("Crowd (optional)")]
    [SerializeField, Min(0f)] private float _waitAreaRadius = 0.8f;

    public Transform ClimbFacing => _climbFacing != null ? _climbFacing : transform;
    public Collider ClimbLateralBounds => _climbLateralBounds;
    public float FailProbability => _failProbability;
    public float WaitBeforeProceedSeconds => _waitBeforeProceedSeconds;
    public float WaitAreaRadius => _waitAreaRadius;
    public bool RequireJump => _requireJump;
    public bool RequireClimb => _requireClimb;
}