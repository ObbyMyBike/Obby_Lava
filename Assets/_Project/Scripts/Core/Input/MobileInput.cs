using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MobileInput : MonoBehaviour, IInput
{
    public event OnPressed OnJumpPressed;
    public event OnPressed OnDashPressed;
    public event OnPressed OnPushPressed;
    
    [SerializeField] private Joystick _moveJoystick;
    [SerializeField] private Button _jumpButton;
    [SerializeField] private Button _dashButton;
    [SerializeField] private Button _pushButton;

    private FollowCameraCinemachineBinder _mainCamera;
    
    private Vector2 _inputDirection;
    private Vector3 _moveDirection;

    [Inject]
    public void Construct(FollowCameraCinemachineBinder mainCamera) => _mainCamera = mainCamera;

    public Vector2 InputDirection => _inputDirection;
    public Vector3 MoveDirection => _moveDirection;

    private void OnEnable()
    {
        _jumpButton.onClick.AddListener(InvokeJump);
        _dashButton.onClick.AddListener(InvokeDash);
        _pushButton.onClick.AddListener(InvokePush);
    }

    private void OnDisable()
    {
        _jumpButton.onClick.RemoveListener(InvokeJump);
        _dashButton.onClick.RemoveListener(InvokeDash);
        _pushButton.onClick.RemoveListener(InvokePush);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        _inputDirection = _moveJoystick.Direction;
        _moveDirection = _mainCamera.ForwardDirection * _inputDirection.y + _mainCamera.RightDirection * _inputDirection.x;
    }

    private void InvokeJump() => OnJumpPressed?.Invoke();

    private void InvokeDash() => OnDashPressed?.Invoke();
    
    private void InvokePush() => OnPushPressed?.Invoke();
}