using UnityEngine;

public class FinishProcessor
{
    private readonly FinishView _finishView;

    private float _startTime = 0f;

    public FinishProcessor(FinishView finishView)
    {
        _finishView = finishView;
        _finishView.OnPlayerFinished += OnFinished;
        _startTime = Time.timeSinceLevelLoad;
    }

    private void OnFinished()
    {
        float time = Time.timeSinceLevelLoad - _startTime;
        _finishView.ActivateFinishing(time);
    }
}