using System;

public interface IPanelAnimator : IDisposable
{
    public void InitializeLayout();
    
    public void PlayShow();
    
    public void PlayHide();
}