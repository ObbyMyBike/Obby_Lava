using UnityEngine;
using DG.Tweening;

public class SkinPickupSpinner
{
    private const float FULL_TURN_DEGREES = 360f;

    private Transform _targetToRotate;
    private Tween _spinTween;
    private float _rotationSpeedDegPerSec;

    public void StartSpin(Transform targetToRotate, float rotationSpeedDegPerSec)
    {
        StopSpin();

        _targetToRotate = targetToRotate;
        _rotationSpeedDegPerSec = Mathf.Max(0f, rotationSpeedDegPerSec);

        if (_targetToRotate == null || _rotationSpeedDegPerSec <= 0f)
            return;

        float duration = FULL_TURN_DEGREES / _rotationSpeedDegPerSec;

        _spinTween = _targetToRotate.DORotate(new Vector3(0f, FULL_TURN_DEGREES, 0f), duration, RotateMode.WorldAxisAdd).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(_targetToRotate.gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void StopSpin()
    {
        if (_spinTween != null && _spinTween.IsActive())
            _spinTween.Kill(false);
        
        _spinTween = null;
        _targetToRotate = null;
    }
}