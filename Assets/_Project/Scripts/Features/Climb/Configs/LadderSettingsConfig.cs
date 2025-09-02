using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Climb/Ladder Settings", fileName = "New Ladder Settings")]
public class LadderSettingsConfig : ScriptableObject
{
   [Header("Speeds")]
    [SerializeField] private float _climbSpeed = 3.0f;

    [Header("Auto Enter")]
    [SerializeField] private float _enterIntentThreshold = 0.35f;
    [SerializeField] private float _maxPlaneOffset = 0.45f;

    [Header("Top Exit Probe")]
    [SerializeField] private float _topForwardProbeDistance = 0.90f;
    [SerializeField] private float _topUpOffset = 1.05f;
    [SerializeField] private float _topDowncastDistance = 2.10f;
    [SerializeField] private float _topMinUpDot = 0.85f;

    [Header("Top Exit Nudge (fallback)")]
    [SerializeField] private float _topExtraUpNudge = 0.12f;

    [Header("Reenter Block")]
    [SerializeField] private float _reenterBlockSeconds = 0.25f;

    [Header("Capsule Clearance")]
    [SerializeField] private float _forwardClearEps = 0.08f;

    [Header("Exit Guards")]
    [SerializeField] private float _minDeltaYBeforeTopExit = 1.2f;
    [SerializeField] private float _minTimeBeforeTopExit = 0.30f;

    [Header("Side Move")]
    [SerializeField] private float _climbSideSpeed = 1.8f;

    [Header("Climb Plane Offset")]
    [SerializeField] private float _climbPlaneOffset = -0.60f;
    [SerializeField] private float _climbPlaneOffsetEps = 0.005f;

    public float ClimbSpeed => _climbSpeed;
    public float EnterIntentThreshold => _enterIntentThreshold;
    public float MaxPlaneOffset => _maxPlaneOffset;
    public float TopForwardProbeDistance => _topForwardProbeDistance;
    public float TopUpOffset => _topUpOffset;
    public float TopDowncastDistance => _topDowncastDistance;
    public float TopMinUpDot => _topMinUpDot;
    public float TopExtraUpNudge => _topExtraUpNudge;
    public float ReenterBlockSeconds => _reenterBlockSeconds;
    public float ForwardClearEps => _forwardClearEps;
    public float MinDeltaYBeforeTopExit => _minDeltaYBeforeTopExit;
    public float MinTimeBeforeTopExit => _minTimeBeforeTopExit;
    public float ClimbSideSpeed => _climbSideSpeed;
    public float ClimbPlaneOffset => _climbPlaneOffset;
    public float ClimbPlaneOffsetEps => _climbPlaneOffsetEps;
}