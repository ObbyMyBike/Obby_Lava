using System;
using UnityEngine;
using DG.Tweening;

public class CountdownPanelScaleFadeAnimator : IPanelAnimator
{
    private const float SCALE_SHOW_DURATION = 0.25f;
    private const float SCALE_HIDE_DURATION = 0.20f;
    private const float FADE_SHOW_DURATION  = 0.20f;
    private const float FADE_HIDE_DURATION  = 0.15f;
    
    private readonly RectTransform rootRect;
    private readonly CanvasGroup group;
    private readonly Vector2 targetSize;

    private Tween _scaleTween;
    private Tween _fadeTween;
    
    public CountdownPanelScaleFadeAnimator(RectTransform rootRect, CanvasGroup group, Vector2 targetSize)
    {
        this.rootRect = rootRect;
        this.group = group;
        this.targetSize = targetSize;
    }

    void IDisposable.Dispose() => KillActiveTweens();
    
    public void InitializeLayout()
    {
        if (rootRect != null)
        {
            rootRect.sizeDelta = targetSize;
            rootRect.localScale = Vector3.zero;
        }

        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    public void PlayShow()
    {
        KillActiveTweens();

        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        if (rootRect != null)
        {
            rootRect.localScale = Vector3.zero;
            _scaleTween = rootRect.DOScale(Vector3.one, SCALE_SHOW_DURATION).SetEase(Ease.OutBack);
        }

        if (group != null)
            _fadeTween = group.DOFade(1f, FADE_SHOW_DURATION);
    }

    public void PlayHide()
    {
        KillActiveTweens();

        if (rootRect != null)
            _scaleTween = rootRect.DOScale(Vector3.zero, SCALE_HIDE_DURATION).SetEase(Ease.InQuad);

        if (group != null)
        {
            _fadeTween = group.DOFade(0f, FADE_HIDE_DURATION).OnComplete(() => 
            {
                group.interactable = false;
                group.blocksRaycasts = false;
            });
        }
    }
    
    private void KillActiveTweens()
    {
        _scaleTween?.Kill();
        _fadeTween?.Kill();
        
        _scaleTween = null;
        _fadeTween = null;
    }
}