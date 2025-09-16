using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private SkinsCatalogConfig _skinCatalogConfig;
    [SerializeField] private ProgressPortrait _progressPortraitPrefab;
    [SerializeField] private RectTransform _portraitsParent;

    private Dictionary<BotAgent, ProgressPortrait> _botProgressMarkMap = new();
    private ProgressPortrait _playerPortrait;

    public void Init(Dictionary<BotAgent, Sprite> botsPortraitsMap)
    {
        foreach (var kvp in botsPortraitsMap)
        {
            ProgressPortrait portrait = Instantiate(_progressPortraitPrefab, _portraitsParent);
            portrait.Initialize(kvp.Value, false);
            _botProgressMarkMap.Add(kvp.Key, portrait);
        }
    }

    public void UpdateBotsProgresses(Dictionary<BotAgent, float> botProgressMap)
    {
        foreach (var kvp in botProgressMap)
        {
            if (!_botProgressMarkMap.ContainsKey(kvp.Key))
            {
                var newPort = Instantiate(_progressPortraitPrefab, _portraitsParent);
                newPort.Initialize(_skinCatalogConfig.Skins.First(skin => skin.Id == kvp.Key.CurrentSkinId).SkinPortraitSprite, false);
                newPort.transform.SetAsFirstSibling();
                _botProgressMarkMap.Add(kvp.Key, newPort);
            }
            var portrait = _botProgressMarkMap[kvp.Key];
            portrait.RectTransform.anchorMin = new(0f, kvp.Value);
            portrait.RectTransform.anchorMax = new(0f, kvp.Value);
            portrait.RectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void UpdatePlayerProgress(SkinIdType skinId, float progress)
    {
        if (_playerPortrait == null)
        {
            _playerPortrait = Instantiate(_progressPortraitPrefab, _portraitsParent);
            _playerPortrait.Initialize(_skinCatalogConfig.Skins.First(skin => skin.Id == skinId).SkinPortraitSprite, true);
            _playerPortrait.transform.SetAsLastSibling();
        }
        _playerPortrait.RectTransform.anchorMin = new(0f, progress);
        _playerPortrait.RectTransform.anchorMax = new(0f, progress);
        _playerPortrait.RectTransform.anchoredPosition = Vector2.zero;
    }
}
