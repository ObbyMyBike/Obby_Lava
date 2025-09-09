using UnityEngine;

public class WaypointNavigator
{
    private readonly BotAiContext context;

    private Transform _waitingWaypoint;
    private Transform _enteredWaypoint;
    private float _waitTimerSeconds;

    public WaypointNavigator(BotAiContext context) => this.context = context;

    public Transform Current => context.Track.Current;
    public bool IsFinished => context.Track.IsFinished;
    public bool IsWaiting => _waitTimerSeconds > 0f;

    public bool TryEnterAndMaybeQueueWait(float deltaTime, Transform waypointTransform, out bool keepWaiting)
    {
        keepWaiting = false;
        
        if (waypointTransform == null)
            return false;

        waypointTransform.TryGetComponent(out Waypoint waypoint);

        if (_enteredWaypoint != waypointTransform)
        {
            _enteredWaypoint = waypointTransform;
            context.Failure.OnEnteredWaypoint(waypointTransform, waypoint != null ? waypoint.FailProbability : 0f);
        }

        if (waypoint != null && waypoint.WaitBeforeProceedSeconds > 0f)
        {
            if (_waitingWaypoint != waypointTransform)
            {
                _waitingWaypoint = waypointTransform;
                _waitTimerSeconds = waypoint.WaitBeforeProceedSeconds;
            }

            keepWaiting = true;
            
            return true;
        }

        return false;
    }

    public bool Reached(Transform waypointTransform, out float reachedRadius)
    {
        reachedRadius = context.Config.ReachedDistance;
        
        if (waypointTransform == null)
            return false;

        waypointTransform.TryGetComponent(out Waypoint waypoint);
        
        if (waypoint != null && waypoint.WaitBeforeProceedSeconds > 0f)
            reachedRadius = Mathf.Max(reachedRadius, waypoint.WaitAreaRadius);

        Vector3 toTarget = waypointTransform.position - context.Agent.Controller.transform.position;
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
        
        return flat.magnitude <= reachedRadius;
    }

    public void TickWait(float deltaTime)
    {
        if (_waitTimerSeconds <= 0f) return;

        _waitTimerSeconds -= deltaTime;
        
        if (_waitTimerSeconds <= 0f)
        {
            _waitingWaypoint = null;
            
            Advance();
        }
    }

    public void Advance()
    {
        context.Track.Advance();
        context.Failure.ResetOnAdvance();
        context.Motion.IdlePose(Time.deltaTime);
    }
}