using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LavaCountdownPanel : CountdownPanelBase
{
    private readonly Vector2 TARGET_SIZE = new Vector2(900f, 260f);

    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Slider _progress;
    
    protected override void Awake()
    {
        if (_rootRect == null)
            _rootRect = transform as RectTransform;

        base.Awake();

        if (_titleText != null)
            _titleText.text = "Lava in:";
    }
    
    public void Connect(LavaProgressionFlow flow)
    {
        flow.OnCountdownDisplayed += BeginCountdown;
        flow.OnPhaseChanged += HandlePhaseChanged;
    }
    
    protected override IPanelAnimator CreateAnimator() => new CountdownPanelScaleFadeAnimator(_rootRect, Group, TARGET_SIZE);

    protected override void OnTick(float secondsLeft, float normalized, float totalSeconds)
    {
        if (_progress != null)
            _progress.value = normalized;
    }
    
    private void HandlePhaseChanged(LavaPhaseType phase, int _)
    {
        if (phase == LavaPhaseType.Rising || phase == LavaPhaseType.Holding || phase == LavaPhaseType.Completed)
            Hide();
    }
}