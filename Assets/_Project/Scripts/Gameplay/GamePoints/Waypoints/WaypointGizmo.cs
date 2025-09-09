using UnityEngine;

[ExecuteAlways]
public class WaypointGizmo : MonoBehaviour
{
    private const float REACHED_RADIUS_SCALE = 1f;

    [SerializeField] private Waypoint _waypoint;
    [SerializeField] private BotDataConfig _debugConfigForRadius;

    private void OnDrawGizmos()
    {
        if (_waypoint == null)
            _waypoint = GetComponent<Waypoint>();

        float radius = _debugConfigForRadius != null
            ? _debugConfigForRadius.ReachedDistance * REACHED_RADIUS_SCALE
            : 0.35f;
        
        Color c =
            (_waypoint != null && _waypoint.RequireClimb) ? new Color(1f, 0.55f, 0.15f, 0.6f) :
            (_waypoint != null && _waypoint.RequireJump) ? new Color(0.1f, 0.8f, 1f, 0.6f) :
            new Color(0.2f, 1f, 0.2f, 0.5f);

        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (_waypoint != null && _waypoint.RequireClimb && _waypoint.ClimbFacing != null)
        {
            Vector3 up = Vector3.up * 0.6f;
            Gizmos.DrawLine(transform.position, transform.position + up);
            // стрелка направления "в лестницу"
            Vector3 f = -_waypoint.ClimbFacing.forward * 0.6f;
            Gizmos.DrawLine(transform.position, transform.position + f);
        }
    }
}