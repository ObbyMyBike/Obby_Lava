using YG;

public class YandexGamesRewardedAdService : IRewardedAdService
{
    private const bool TRY_PRELOAD_AT_CONSTRUCT = true;

    public YandexGamesRewardedAdService()
    {
        if (TRY_PRELOAD_AT_CONSTRUCT)
            YG2.optionalPlatform.LoadRewardedAdv();
    }

    void IRewardedAdService.ShowRewarded(string rewardId, OnRewardedAdComplete onCompleted)
    {
        YG2.RewardedAdvShow(rewardId, () => onCompleted?.Invoke());
    }
}