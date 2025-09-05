using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class LadderTriggerVolume : MonoBehaviour
{
    [SerializeField] private Transform _facing;

    private Collider _collider;
    private LadderClimb _climb;
    private LadderSettingsConfig _config;

    private Transform Facing => _facing != null ? _facing : transform;

    [Inject]
    private void Construct(LadderClimb climb, LadderSettingsConfig config)
    {
        _climb = climb;
        _config = config;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_climb == null || !_climb.CanEnterNow || _climb.IsClimbing)
            return;
        
        if (!other.TryGetComponent(out Player player))
            return;

        IPlayerMoveDirectionProvider provider = player;
        Vector3 move = provider.CurrentMoveDirectionWorld;
        
        if (!HasEnterIntent(move, Facing, out _) || !IsCloseToLadderPlane(other.transform.position, Facing, out _))
            return;
        
        _climb.TryEnter(Facing, _collider);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_climb == null || !_climb.IsClimbing)
            return;

        if (other.TryGetComponent(out Player _))
            _climb.TryExit(Facing);
    }

    private bool HasEnterIntent(Vector3 moveDirWorld, Transform facing, out float into)
    {
        Vector3 planar = Vector3.ProjectOnPlane(moveDirWorld, Vector3.up);
        
        if (planar.sqrMagnitude < 0.0001f)
        {
            into = 0f;
            
            return false;
        }

        Vector3 local = facing.InverseTransformDirection(planar).normalized;
        into = Mathf.Clamp01(-local.z);
        float thr = _config != null ? _config.EnterIntentThreshold : 0.35f;
        
        return into >= thr;
    }

    private bool IsCloseToLadderPlane(Vector3 playerPos, Transform facing, out float planeOffset)
    {
        Vector3 origin = facing.position;
        Vector3 normal = facing.forward;
        planeOffset = Mathf.Abs(Vector3.Dot(playerPos - origin, normal));
        float max = _config != null ? _config.MaxPlaneOffset : 0.45f;
        
        return planeOffset <= max;
    }
}