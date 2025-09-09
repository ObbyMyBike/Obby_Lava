using UnityEngine;
using Zenject;

public class AutoPush : ITickable
{
    private const float DEFAULT_TICK_INTERVAL = 0.25f;
    private const float ORIGIN_UP_OFFSET = 0.5f;
    private const int MAX_PUSHES_PER_TICK = 4;

    public event OnPush OnPushed;

    private readonly PushInteraction pushService;
    private readonly SpawnedPlayerAccessor accessor;
    private readonly float pushRange;

    private float _tickTimer;
    private bool _isEnabled;
    
    public AutoPush(PushInteraction pushService, SpawnedPlayerAccessor accessor, float pushRange)
    {
        this.pushService = pushService;
        this.accessor = accessor;
        this.pushRange = pushRange;

        _tickTimer = 0f;
        _isEnabled = false;
    }
    
    void ITickable.Tick()
    {
        if (!_isEnabled)
            return;
        
        Transform playerTransform = accessor.Transform;
        
        if (playerTransform == null)
            return;

        _tickTimer -= Time.deltaTime;
        
        if (_tickTimer > 0f)
            return;

        _tickTimer = DEFAULT_TICK_INTERVAL;

        Vector3 origin = playerTransform.position + Vector3.up * ORIGIN_UP_OFFSET;
        int pushed = pushService.TryPush(origin, pushRange, MAX_PUSHES_PER_TICK);

        if (pushed > 0)
            OnPushed?.Invoke();
    }
    
    public void Enable() => _isEnabled = true;
    
    public void Disable() => _isEnabled = false;
}