using UnityEngine;

[CreateAssetMenu(fileName = "InputConfig_", menuName = "Configs/Input")]
public class InputConfig : ScriptableObject
{
    [field: SerializeField] public KeyCode JumpButton { get; private set; }
    [field: SerializeField] public KeyCode DashButton { get; private set; }
    [field: SerializeField] public KeyCode PushButton { get; private set; }
    [field: SerializeField] public AxisOptions XInput { get; private set; }
    [field: SerializeField] public AxisOptions YInput { get; private set; }
}