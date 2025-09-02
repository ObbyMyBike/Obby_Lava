using UnityEngine;
using UnityEngine.UI;

public class SkinPickupProgressBarView : MonoBehaviour
{
    private const float CANVAS_SHOW = 1f;
    private const float CANVAS_HIDE = 0f;
    
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _slider;

    public void Show()
    {
        _canvasGroup.alpha = CANVAS_SHOW;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = CANVAS_HIDE;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetProgress(float value)
    {
        _slider.value = Mathf.Clamp01(value); 
    }
}