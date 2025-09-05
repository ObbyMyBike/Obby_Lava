using YG;

public class YandexGamesRewardedAd
{
    private const bool TRY_PRELOAD_AT_CONSTRUCT = true;

    public YandexGamesRewardedAd()
    {
        if (TRY_PRELOAD_AT_CONSTRUCT)
            YG2.optionalPlatform.LoadRewardedAdv();
    }

    public void ShowRewarded(string rewardId, OnRewardedAdComplete onCompleted)
    {
        YG2.RewardedAdvShow(rewardId, () => onCompleted?.Invoke());
    }
}