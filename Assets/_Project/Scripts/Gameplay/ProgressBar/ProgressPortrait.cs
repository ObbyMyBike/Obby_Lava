using UnityEngine;
using UnityEngine.UI;

public class ProgressPortrait : MonoBehaviour
{
    [SerializeField] private Image _portrait;

    private ProgressBroadcaster _progressBroadcaster;
    private RectTransform _rectTransform;

    private float _containerHeight;
    private float _currentProgress;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

    }

    public void Initialize(ProgressBroadcaster broadcaster, RectTransform container)
    {
        _portrait.sprite = broadcaster.Portrait;
        _progressBroadcaster = broadcaster;
        _currentProgress = _progressBroadcaster.Progress;
        _containerHeight = container.rect.height - 50;

    }

    private void Update()
    {
        float targetY = _progressBroadcaster.Progress * _containerHeight;

        Vector2 newPos = _rectTransform.anchoredPosition;
        newPos.y = Mathf.Lerp(
            newPos.y,
            targetY,
            Time.deltaTime * 5
        );

        _rectTransform.anchoredPosition = newPos;
    }
}
