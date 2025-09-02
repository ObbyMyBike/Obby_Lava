using UnityEngine;

public interface IInput
{
    public event OnPressed OnJumpPressed;
    public event OnPressed OnDashPressed;
    public event OnPressed OnPushPressed;
    
    public Vector2 InputDirection { get; }
    public Vector3 MoveDirection { get; }
}