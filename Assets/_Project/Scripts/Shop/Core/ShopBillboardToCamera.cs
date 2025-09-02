using UnityEngine;
using Zenject;

[DisallowMultipleComponent]
public class ShopBillboardToCamera : MonoBehaviour
{
    private const float DEFAULT_ROTATION_SPEED_DEG_PER_SEC = 540f;
    private const float MIN_DIRECTION_SQR_MAGNITUDE = 0.0001f;

    [SerializeField] private Transform _pivotToRotate;
    [SerializeField] private bool _rotateOnlyYaw = true;
    [SerializeField] private float _rotationSpeedDegPerSec = DEFAULT_ROTATION_SPEED_DEG_PER_SEC;
    
    private ICameraProvider _cameraProvider;

    [Inject]
    public void Construct(ICameraProvider cameraProvider)
    {
        _cameraProvider = cameraProvider;
    }
    
    private void Awake()
    {
        if (_pivotToRotate == null)
            _pivotToRotate = transform;
    }

    private void LateUpdate()
    {
        Transform cameraTransform = _cameraProvider.CameraTransform;
        
        if (cameraTransform == null || _pivotToRotate == null)
            return;

        Vector3 toCamera = cameraTransform.position - _pivotToRotate.position;
        
        if (_rotateOnlyYaw)
            toCamera.y = 0f;

        if (toCamera.sqrMagnitude < MIN_DIRECTION_SQR_MAGNITUDE)
            return;

        Quaternion target = Quaternion.LookRotation(toCamera.normalized, Vector3.up);

        if (_rotationSpeedDegPerSec <= 0f)
            _pivotToRotate.rotation = target;
        else
            _pivotToRotate.rotation = Quaternion.RotateTowards(_pivotToRotate.rotation, target, _rotationSpeedDegPerSec * Time.deltaTime);
    }
}