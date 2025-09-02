public class GoldWallet
{
    public event OnGoldChanged OnGoldChanged;

    public GoldWallet(int startingGold)
    {
        CurrentGold = startingGold;
        OnGoldChanged?.Invoke(CurrentGold);
    }
    
    public int CurrentGold { get; private set; }
    
    public bool TrySpend(int amount)
    {
        if (CurrentGold < amount)
            return false;
        
        CurrentGold -= amount;
        
        OnGoldChanged?.Invoke(CurrentGold);
        
        return true;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        
        OnGoldChanged?.Invoke(CurrentGold);
    }
}