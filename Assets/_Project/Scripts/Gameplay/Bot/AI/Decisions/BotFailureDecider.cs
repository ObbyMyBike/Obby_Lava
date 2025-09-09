using UnityEngine;

public class BotFailureDecider
{
    private const float MIN_PROBABILITY = 0f;
    private const float MAX_PROBABILITY = 1f;

    private readonly FailureDice failureDice;

    private Transform _enteredWaypoint;
    private bool _hasRolledForEnteredWaypoint;
    private bool _rolledFailForEnteredWaypoint;
    private bool _sabotageAlreadyAppliedForEnteredWaypoint;

    public BotFailureDecider(FailureDice failureDice)
    {
        this.failureDice = failureDice;
    }
    
    public void OnEnteredWaypoint(Transform waypointTransform, float failProbability01)
    {
        if (waypointTransform == null)
            return;

        if (_enteredWaypoint != waypointTransform)
        {
            _enteredWaypoint = waypointTransform;
            _hasRolledForEnteredWaypoint = false;
            _sabotageAlreadyAppliedForEnteredWaypoint = false;
        }

        if (!_hasRolledForEnteredWaypoint)
        {
            float clamped = Mathf.Clamp(failProbability01, MIN_PROBABILITY, MAX_PROBABILITY);
            _rolledFailForEnteredWaypoint = failureDice.Roll(clamped);
            _hasRolledForEnteredWaypoint = true;
        }
    }

    public bool HasPendingSabotage => _hasRolledForEnteredWaypoint && _rolledFailForEnteredWaypoint && !_sabotageAlreadyAppliedForEnteredWaypoint;

    public void MarkSabotageApplied()
    {
        _sabotageAlreadyAppliedForEnteredWaypoint = true;
    }

    public void ResetOnAdvance()
    {
        _enteredWaypoint = null;
        _hasRolledForEnteredWaypoint = false;
        _rolledFailForEnteredWaypoint = false;
        _sabotageAlreadyAppliedForEnteredWaypoint = false;
    }
}