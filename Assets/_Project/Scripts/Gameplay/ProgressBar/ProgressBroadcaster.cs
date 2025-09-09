using UnityEngine;

public class ProgressBroadcaster : MonoBehaviour
{
    [field: SerializeField] public Sprite Portrait { get; private set; }
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _finish;

    private float _startY;
    private float _finishY;
    private float _totalHeight;

    public float Progress { get; private set; }

    private void Awake()
    {
        _startY = _start.position.y;
        _finishY = _finish.position.y;

        _totalHeight = Mathf.Abs(_finishY - _startY);
    }

    private void Update()
    {
        float normalizedY = Mathf.InverseLerp(_startY, _finishY, transform.position.y);
        Progress = Mathf.Clamp01(normalizedY);
    }

    private void OnDrawGizmosSelected()
    {
        if (_start && _finish)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_start.position, _finish.position);
            Gizmos.DrawSphere(_start.position, 0.5f);
            Gizmos.DrawSphere(_finish.position, 0.5f);
        }
    }
}