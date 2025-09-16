using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ProgressDisplayingProcessor : IInitializable
{
    private readonly ProgressBar _progressBar;
    private readonly GlobalCoroutineRunner _coroutineRunner;
    private readonly ActiveBots _activeBots;
    private readonly SpawnedPlayerAccessor _playerAccessor;
    private readonly SkinsCatalogConfig _skinsCatalogConfig;
    private readonly YGSkinsSaveRepository _skinsRepository;

    private List<CheckpointTrigger> _allCheckpoints;
    private Vector3 _finishPosition;
    private Dictionary<BotAgent, Sprite> _botPortraitMapper = new();
    private Dictionary<BotAgent, float> _progresses = new();

    public ProgressDisplayingProcessor(
        ProgressBar progressBar, 
        GlobalCoroutineRunner coroutineRunner,
        ActiveBots activeBots,
        SpawnedPlayerAccessor playerAccessor,
        SkinsCatalogConfig skinCatalogConfig,
        YGSkinsSaveRepository skinsRepository
        )
    {
        _progressBar = progressBar;
        _coroutineRunner = coroutineRunner;
        _activeBots = activeBots;
        _playerAccessor = playerAccessor;
        _skinsCatalogConfig = skinCatalogConfig;
        _skinsRepository = skinsRepository;
    }

    public void Initialize()
    {
        _allCheckpoints = Object.FindObjectsOfType<CheckpointTrigger>().ToList();
        _finishPosition = _allCheckpoints
            .OrderByDescending(point => point.transform.position.y)
            .First().transform.position;
        BuildBotPortraitMapper();
        _progressBar.Init(_botPortraitMapper);
        _coroutineRunner.StartCoroutine(ProgressUpdatingCoroutine());
    }

    private void BuildBotPortraitMapper()
    {
        foreach (var bot in _activeBots.Active)
        {
            _botPortraitMapper.Add(bot, _skinsCatalogConfig.Skins.First(skin => skin.Id == bot.CurrentSkinId).SkinPortraitSprite);
            _progresses.Add(bot, 0f);
        }
    }

    private IEnumerator ProgressUpdatingCoroutine()
    {
        while (true)
        {
            yield return null;
            UpdateProgress();
        }
    }

    private void UpdateProgress()
    {
        foreach (var bot in _activeBots.Active)
        {
            float progress = bot.transform.position.y / _finishPosition.y;
            _progresses[bot] = progress;
        }
        _progressBar.UpdateBotsProgresses(_progresses);
        float playerProgress = _playerAccessor.Transform.position.y / _finishPosition.y;
        _progressBar.UpdatePlayerProgress(_skinsRepository.SelectedSkin, playerProgress);
    }
}