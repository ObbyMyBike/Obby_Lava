using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private ProgressPortrait _progressPortraitPrefab;
    [SerializeField] private RectTransform _portraitsParent;

    private List<ProgressBroadcaster> _progressBroadcasters;

    [Inject]
    public void Construct(List<ProgressBroadcaster> progressBroadcasters)
    {
        _progressBroadcasters = progressBroadcasters;
    }

    private void Start()
    {
        foreach (ProgressBroadcaster broadCaster in _progressBroadcasters)
        {
            ProgressPortrait portratit = Instantiate(_progressPortraitPrefab, _portraitsParent);
            portratit.Initialize(broadCaster, _portraitsParent);
        }
    }
}
