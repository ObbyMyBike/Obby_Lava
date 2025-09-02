using UnityEngine;
using Zenject;

public class DesktopInput : IInput, ITickable
{
    public event OnPressed OnJumpPressed;
    public event OnPressed OnDashPressed;
    public event OnPressed OnPushPressed;

    private readonly IMainCamera mainCamera;
    private readonly InputConfig inputConfig;
    private readonly string xInputName;
    private readonly string yInputName;
    
    private Vector2 _inputDirection;
    private Vector3 _moveDirection;

    public DesktopInput(IMainCamera mainCamera, InputConfig inputConfig)
    {
        this.mainCamera = mainCamera;
        this.inputConfig = inputConfig;
        this.xInputName = inputConfig.XInput.ToString();
        this.yInputName = inputConfig.YInput.ToString();
    }

    public Vector2 InputDirection => _inputDirection;
    public Vector3 MoveDirection => _moveDirection;

    public void Tick() => HandleInput();

    private void HandleInput()
    {
        float x = Input.GetAxis(this.xInputName);
        float y = Input.GetAxis(this.yInputName);
        
        _inputDirection = new Vector2(x, y);
        _moveDirection = this.mainCamera.ForwardDirection * y + this.mainCamera.RightDirection * x;

        if (Input.GetKeyDown(this.inputConfig.JumpButton))
            OnJumpPressed?.Invoke();

        if (Input.GetKeyDown(this.inputConfig.DashButton))
            OnDashPressed?.Invoke();

        if (Input.GetKeyDown(this.inputConfig.PushButton))
            OnPushPressed?.Invoke();
    }
}