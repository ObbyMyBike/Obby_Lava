using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LavaProgressionConfig", menuName = "Configs/Lava/Progression")]
public class LavaProgressionConfig : ScriptableObject
{
    [Header("Base")]
    [SerializeField] private float _baseSurfaceY = 0f;

    [Header("Defaults (used if step value <= 0)")]
    [SerializeField] private AnimationCurve _defaultRiseCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _defaultWaitBeforeCountdownSeconds = 15f;
    [SerializeField] private float _defaultCountdownDurationSeconds = 5f;
    [SerializeField] private float _defaultRiseDurationSeconds = 6f;
    [SerializeField] private float _defaultHoldDurationSeconds = 4f;

    [Header("Steps")]
    [SerializeField] private List<LavaStepConfig> _steps = new List<LavaStepConfig>()
    {
        new LavaStepConfig { TargetHeight = 2f },
        new LavaStepConfig { TargetHeight = 4.5f },
        new LavaStepConfig { TargetHeight = 7f }
    };

    public AnimationCurve DefaultRiseCurve => _defaultRiseCurve;
    public IReadOnlyList<LavaStepConfig> Steps => _steps;
    public float BaseSurfaceY => _baseSurfaceY;
    public float DefaultWaitBeforeCountdownSeconds => _defaultWaitBeforeCountdownSeconds;
    public float DefaultCountdownDurationSeconds => _defaultCountdownDurationSeconds;
    public float DefaultRiseDurationSeconds => _defaultRiseDurationSeconds;
    public float DefaultHoldDurationSeconds => _defaultHoldDurationSeconds;
}