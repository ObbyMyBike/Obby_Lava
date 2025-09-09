public class LavaPreCountdownPanel : CountdownPanelBase
{
    public void Connect(LavaProgressionFlow flow)
    {
        flow.OnPreCountdownDisplayed += BeginCountdown;
        flow.OnPhaseChanged += HandlePhaseChanged;
    }
    
    protected override IPanelAnimator CreateAnimator() => null;
    
    private void HandlePhaseChanged(LavaPhaseType phase, int _)
    {
        if (phase == LavaPhaseType.Countdown || phase == LavaPhaseType.Rising || phase == LavaPhaseType.Completed)
            Hide();
    }
}