using System;
using UnityEngine;

[Serializable]
public struct LavaStepConfig
{
    public AnimationCurve RiseCurve;
    public float TargetHeight;
    public float WaitBeforeCountdownSeconds;
    public float CountdownDurationSeconds;
    public float RiseDurationSeconds;
    public float HoldDurationSeconds;
}