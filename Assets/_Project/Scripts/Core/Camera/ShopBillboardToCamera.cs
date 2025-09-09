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
    
    private MainCameraProvider _cameraProvider;
    private Camera _fallbackCamera;
    private bool _warnedNoCameraOnce;

    [Inject]
    public void Construct(MainCameraProvider cameraProvider)
    {
        _cameraProvider = cameraProvider;
    }
    
    private void Awake()
    {
        if (_pivotToRotate == null)
            _pivotToRotate = transform;
        
        _fallbackCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Transform cameraTransform = (_cameraProvider != null ? _cameraProvider.CameraTransform : null) ?? (_fallbackCamera != null ? _fallbackCamera.transform : null);

        if (cameraTransform == null || _pivotToRotate == null)
        {
            if (!_warnedNoCameraOnce)
                _warnedNoCameraOnce = true;
            
            return;
        }

        Vector3 toCamera = cameraTransform.position - _pivotToRotate.position;

        if (_rotateOnlyYaw)
            toCamera.y = 0f;

        if (toCamera.sqrMagnitude < MIN_DIRECTION_SQR_MAGNITUDE)
            return;

        Quaternion target = Quaternion.LookRotation(toCamera.normalized, Vector3.up);

        float speed = Mathf.Max(_rotationSpeedDegPerSec, 0f);
        
        if (speed <= 0f)
            _pivotToRotate.rotation = target;
        else
            _pivotToRotate.rotation = Quaternion.RotateTowards(_pivotToRotate.rotation, target, speed * Time.deltaTime);
    }
}