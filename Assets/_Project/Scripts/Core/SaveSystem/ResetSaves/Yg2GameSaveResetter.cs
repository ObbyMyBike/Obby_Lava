using YG;

public class Yg2GameSaveResetter
{
    private const string DEFAULT_SKIN_STRING = nameof(SkinIdType.Default);

    public void ResetAllSaves()
    {
        if (YG2.saves.OwnedSkinsIds != null)
            YG2.saves.OwnedSkinsIds.Clear();

        YG2.saves.SelectedSkinId = DEFAULT_SKIN_STRING;

        YG2.SaveProgress();
    }
}