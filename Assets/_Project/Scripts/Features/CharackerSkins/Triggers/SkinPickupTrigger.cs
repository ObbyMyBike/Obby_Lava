using System;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class SkinPickupTrigger : MonoBehaviour
{
    private const float FILL_SECONDS_DEFAULT = 3f;
    private const float DEFAULT_AUTO_ROTATION_SPEED = 20f;

    [SerializeField] private SkinIdType _skinId;
    [SerializeField] private SkinPickupProgressBarView _progressView;
    [SerializeField] private Transform _visualAnchor;
    [SerializeField] private float _fillSeconds = FILL_SECONDS_DEFAULT;
    
    private readonly SkinPickupSpinner spinner = new SkinPickupSpinner();
    private readonly float autoRotationSpeedSkinPerSecond = DEFAULT_AUTO_ROTATION_SPEED;

    private SkinsCatalogConfig _catalog;
    private YandexGamesRewardedAd _rewarded;
    private YGSkinsSaveRepository _repository;
    private SkinUnlockAndApply _unlockApply;
    private IDisposable _fillSubscription;
    
    private Collider _collider;
    private GameObject _spawnedVisual;

    [Inject]
    public void Construct(YandexGamesRewardedAd rewarded, YGSkinsSaveRepository repository, SkinUnlockAndApply unlockApply, SkinsCatalogConfig catalog)
    {
        _rewarded = rewarded;
        _repository = repository;
        _unlockApply = unlockApply;
        _catalog = catalog;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        
        if (_visualAnchor == null)
            _visualAnchor = transform;
    }
    
    private void OnDisable()
    {
        spinner.StopSpin();
    }
    
    private void Start()
    {
        if (_repository == null || _catalog == null || _rewarded == null || _unlockApply == null)
        {
            Debug.LogWarning("[SkinPickupTrigger] Not injected yet.");
            
            return;
        }

        if (_repository.HasSkin(_skinId))
        {
            gameObject.SetActive(false);
            
            return;
        }

        TrySpawnPickupVisual();
        
        spinner.StartSpin(_visualAnchor, autoRotationSpeedSkinPerSecond);
        
        if (_progressView != null)
            _progressView.Hide();
    }

    private void OnDestroy()
    {
        spinner.StopSpin();
        
        if (_spawnedVisual != null)
            Destroy(_spawnedVisual);
        
        _spawnedVisual = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player _))
            return;

        StartFill();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Player _))
            return;

        ResetFill();
    }

    private void StartFill()
    {
        ResetFill();
        _progressView.Show();

        _fillSubscription = Observable.EveryUpdate().Scan(0f, (acc, _) => acc + Time.deltaTime)
            .TakeWhile(time => time < _fillSeconds).Do(time => _progressView?.SetProgress(time / _fillSeconds))
            .LastOrDefault().Subscribe(_ =>
            {
                _progressView?.SetProgress(1f);
                TryShowRewardedAndGrant();
            });
    }
    
    private void ResetFill()
    {
        _fillSubscription?.Dispose();
        _progressView?.SetProgress(0f);
        _progressView?.Hide();
    }

    private void TryShowRewardedAndGrant()
    {
        if (!_catalog.TryGetDefinition(_skinId, out SkinDefinition definition))
        {
            gameObject.SetActive(false);
            
            return;
        }
        
        _rewarded.ShowRewarded(definition.RewardedAdvId, () =>
        {
            _unlockApply.UnlockAndApply(_skinId);
            spinner.StopSpin();
            gameObject.SetActive(false);
        });
    }
    
    private void TrySpawnPickupVisual()
    {
        if (!_catalog.TryGetDefinition(_skinId, out SkinDefinition definition))
            return;

        if (definition.PickupVisualPrefab == null)
            return;
        
        if (_spawnedVisual != null)
        {
            Destroy(_spawnedVisual);
            
            _spawnedVisual = null;
        }

        _spawnedVisual = Instantiate(definition.PickupVisualPrefab, _visualAnchor);
        
        _spawnedVisual.transform.localPosition = Vector3.zero;
        _spawnedVisual.transform.localRotation = Quaternion.identity;
        _spawnedVisual.transform.localScale = Vector3.one;
    }
}