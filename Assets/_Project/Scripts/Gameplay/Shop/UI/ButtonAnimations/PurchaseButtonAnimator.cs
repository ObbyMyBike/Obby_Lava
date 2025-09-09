using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PurchaseButtonAnimator
{
    private const float RANDOMNESS_DEGREES = 90f;
    
    private readonly Transform buttonTransform;
    private readonly RectTransform rectTransform;
    private readonly Vector3 originalScale;
    private readonly Vector3 originalLocalPos;
    private readonly Vector2 originalAnchoredPos;
    private readonly Image purchasedImage;
    private readonly float pressScale;
    private readonly float pressTime;
    private readonly float shakeTime;
    private readonly float shakeStrength;
    private readonly int shakeVibrato;

    public PurchaseButtonAnimator(Transform buttonTransform, Image purchasedImage, float pressScale, float pressTime, float shakeTime, float shakeStrength, int shakeVibrato)
    {
        this.buttonTransform = buttonTransform;
        this.rectTransform = buttonTransform as RectTransform;
        this.originalScale = buttonTransform.localScale;
        this.originalLocalPos = buttonTransform.localPosition;
        this.originalAnchoredPos = this.rectTransform != null ? this.rectTransform.anchoredPosition : Vector2.zero;
        this.purchasedImage = purchasedImage;
        this.pressScale = pressScale;
        this.pressTime = pressTime;
        this.shakeTime = shakeTime;
        this.shakeStrength = shakeStrength;
        this.shakeVibrato = shakeVibrato;

        if (this.purchasedImage != null)
            this.purchasedImage.gameObject.SetActive(false);
    }

    public void PressDown()
    {
        buttonTransform.DOScale(originalScale * pressScale, pressTime).SetEase(Ease.OutQuad);
    }

    public void Success()
    {
        buttonTransform.DOScale(Vector3.zero, pressTime).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (purchasedImage != null)
                purchasedImage.gameObject.SetActive(true);
        });
    }

    public void Fail()
    {
        buttonTransform.DOScale(originalScale, pressTime).SetEase(Ease.OutQuad);
        
        if (rectTransform != null)
        {
            rectTransform.DOShakeAnchorPos(shakeTime, shakeStrength, shakeVibrato, RANDOMNESS_DEGREES, false)
                .OnComplete(() => { rectTransform.anchoredPosition = originalAnchoredPos; });
        }
        else
        {
            buttonTransform.DOShakePosition(shakeTime, shakeStrength, shakeVibrato, RANDOMNESS_DEGREES, false)
                .OnComplete(() => { buttonTransform.localPosition = originalLocalPos; });
        }
    }
}