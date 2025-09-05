using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ThreeCheckpointPlacer : ITickable, IInitializable, IDisposable
{
    private const string LAYER_GROUND = "Ground";
    private const int LEFT_MOUSE_BUTTON = 0;
    private const int RIGHT_MOUSE_BUTTON = 1;
    private const float MAX_RAY_DISTANCE = 200f;
    private const float PREVIEW_AHEAD_METERS = 2.5f;
    private const float RAYCAST_UP_OFFSET = 5f;
    private const float SURFACE_LIFT = 1.2f;

    public event OnPlacementModeChange OnPlacementModeChanged;

    private readonly PlayerRespawn respawn;
    private readonly ThreeCheckpointsInventory inventory;
    private readonly SimpleGameObjectPool pool;
    private readonly DiContainer container;
    private readonly Camera camera;
    private readonly GameObject previewInstance;
    private readonly SpawnedPlayerAccessor accessor;
    
    private readonly int groundMask;
    private readonly int preload = 3;
    private readonly bool isMobilePlatform;

    private Vector3 _lastValidPoint;
    private Vector3 _lastValidNormal = Vector3.up;
    private bool _placementEnabled;
    private bool _hasValidHit;

    [Inject]
    public ThreeCheckpointPlacer(MainCameraProvider camera, PlayerRespawn respawn, ThreeCheckpointsInventory inventory,
        [Inject(Id = "ThreeCheckpointPrefab")] GameObject checkpointPrefab, [Inject(Id = "ThreeCheckpointParent")] Transform parent,
        [Inject(Id = "ThreeCheckpointPreviewPrefab")] GameObject previewPrefab, PlatformDefinition platformDefinition,
        SpawnedPlayerAccessor accessor, DiContainer container)
    {
        this.container = container;
        this.respawn = respawn;
        this.inventory = inventory;
        this.accessor = accessor;

        this.camera = camera != null ? (camera.CameraTransform != null ? camera.CameraTransform.GetComponent<Camera>() : Camera.main) : Camera.main;
        
        this.pool = new SimpleGameObjectPool(checkpointPrefab, parent, preload);
        this.previewInstance = Object.Instantiate(previewPrefab);
        this.previewInstance.SetActive(false);

        this.groundMask = LayerMask.GetMask(LAYER_GROUND);
        this.isMobilePlatform = platformDefinition != null && platformDefinition.IsMobile;
    }

    public bool IsPlacementEnabled => _placementEnabled;

    void IInitializable.Initialize()
    {
        inventory.OnCountChanged += OnCountChanged;
        inventory.OnEmptied += OnEmptied;

        DisablePlacementInternal();
    }

    void IDisposable.Dispose()
    {
        inventory.OnCountChanged -= OnCountChanged;
        inventory.OnEmptied -= OnEmptied;

        if (previewInstance != null)
            Object.Destroy(previewInstance);
    }

    void ITickable.Tick()
    {
        if (camera == null)
            return;

        if (!isMobilePlatform)
        {
            if (!_placementEnabled)
            {
                if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON))
                    TryBeginPlacement();

                return;
            }

            UpdatePreview();

            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
            {
                ConfirmPlacement();
                return;
            }

            if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON))
            {
                CancelPlacement();
                return;
            }

            return;
        }

        if (_placementEnabled)
            UpdatePreview();
    }

    public void TryBeginPlacement()
    {
        if (_placementEnabled || inventory.Count <= 0)
            return;

        _placementEnabled = true;
        previewInstance.SetActive(true);

        UpdatePreview();

        OnPlacementModeChanged?.Invoke(true);
    }

    public void ConfirmPlacement()
    {
        if (!_placementEnabled || !_hasValidHit)
            return;

        if (!TryConsumeOneSafe())
        {
            DisablePlacementInternal();

            return;
        }

        GameObject instance = pool.Get();
        container.InjectGameObject(instance);
        
        Transform instanceTransform = instance.transform;
        instanceTransform.position = _lastValidPoint;
        instanceTransform.up = _lastValidNormal;
        
        float progress = instanceTransform.position.y;
        respawn.TrySetCheckpoint(instanceTransform, progress, false);
        
        if (instance.TryGetComponent(out CheckpointTrigger trigger))
            trigger.MarkActivated();

        DisablePlacementInternal();
    }

    public void CancelPlacement()
    {
        if (!_placementEnabled)
            return;

        DisablePlacementInternal();
    }

    private bool TryConsumeOneSafe()
    {
        if (!_hasValidHit)
            return false;

        return inventory.TryConsumeOne();
    }

    private void OnEmptied(ItemType _) => DisablePlacementInternal();

    private void OnCountChanged(ItemType _, int count, Sprite __)
    {
        if (count <= 0)
            DisablePlacementInternal();
    }

    private void DisablePlacementInternal()
    {
        _placementEnabled = false;
        _hasValidHit = false;

        if (previewInstance != null)
            previewInstance.SetActive(false);

        OnPlacementModeChanged?.Invoke(false);
    }

    private void UpdatePreview()
    {
        Transform playerTransform = accessor != null ? accessor.Transform : null;
        
        if (playerTransform == null)
        {
            _hasValidHit = false;
            
            return;
        }

        Vector3 playerPosition = playerTransform.position;
        Vector3 forward = playerTransform.forward.sqrMagnitude < 0.0001f ? Vector3.forward : playerTransform.forward.normalized;
        Vector3 ahead = playerPosition + forward * PREVIEW_AHEAD_METERS;
        Vector3 rayOrigin = ahead + Vector3.up * RAYCAST_UP_OFFSET;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, MAX_RAY_DISTANCE, groundMask, QueryTriggerInteraction.Ignore))
        {
            _hasValidHit = true;

            Vector3 liftedPoint = hit.point + hit.normal * SURFACE_LIFT;
            _lastValidPoint = liftedPoint;
            _lastValidNormal = hit.normal;

            if (previewInstance != null)
            {
                previewInstance.transform.position = liftedPoint;
                previewInstance.transform.up = hit.normal;
            }
        }
        else
        {
            _hasValidHit = false;
        }
    }
}