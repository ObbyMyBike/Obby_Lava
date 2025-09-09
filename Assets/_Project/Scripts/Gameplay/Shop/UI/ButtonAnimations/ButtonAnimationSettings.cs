using UnityEngine;

[CreateAssetMenu(fileName = "ButtonAnimationSettings", menuName = "Configs/Button/Button Animation Settings")]
public class ButtonAnimationSettings : ScriptableObject
{
    [SerializeField] private float _pressScale = 0.8f;
    [SerializeField] private float _pressTime = 0.1f;
    [SerializeField] private float _shakeTime = 0.3f;
    [SerializeField] private float _shakeStrength = 10f;
    [SerializeField] private int _shakeVibrato = 20;

    public float PressScale => _pressScale;
    public float PressTime => _pressTime;
    public float ShakeTime => _shakeTime;
    public float ShakeStrength => _shakeStrength;
    public int ShakeVibrato => _shakeVibrato;
}