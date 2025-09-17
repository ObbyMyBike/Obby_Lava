using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class BotAgent : MonoBehaviour
{
    public event OnAgentDie OnDied;

    [SerializeField] private BotNameUI _botNameUI;
    [SerializeField] private SkinsCatalogConfig _skinsCatalogConfig;
    [SerializeField] private List<GameObject> _skins;    
    
    private BotDataConfig _config;
    private BotHealth _health;
    private BotAI _ai;
    
    private BotRespawn _respawn;
    private BotLadderBridge _ladderBridge;
    private MotionSolver _motionSolver;
    
    private CharacterController _controller;
    private Animator _animator;
    private AnimationPlayback _animationPlayback;

    private int _uniqueSeed;
    private SkinIdType _currentSkinId;

    private bool _isPushed = false;

    public SkinIdType CurrentSkinId => _currentSkinId;

    [Inject]
    public void Construct(int uniqueSeed, Transform startSpawnPoint,  BotRespawnDirectory respawnDirectory, BotDataConfig botDataConfig,
        WaypointPath waypointPath, ActiveBots agentsRegistry, LadderSettingsConfig ladderSettings, MainCameraProvider mainCameraProvider )
    {
        _config = botDataConfig;
        _uniqueSeed = uniqueSeed;

        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _animationPlayback = new AnimationPlayback(_animator);
        _motionSolver = new MotionSolver(_controller, botDataConfig.MoveSpeed, botDataConfig.JumpForce, botDataConfig.Gravity,
            botDataConfig.DashDistance, botDataConfig.DashDuration, botDataConfig.DashCooldown);
        _respawn = new BotRespawn(startSpawnPoint, _config.RespawnProgressMinDelta);
        respawnDirectory.Register(this, _respawn);

        _ladderBridge = new BotLadderBridge(_controller, _animationPlayback, ladderSettings, _config, botDataConfig.GroundMask);

        _ai = new BotAI(this, _motionSolver, _respawn, waypointPath, botDataConfig, agentsRegistry, _uniqueSeed, _ladderBridge);
        
        _ai.OnDied += HandleAiDied;
        
        _health = new BotHealth(_config.MaxHealth, _config.MaxHealth);

        SetRandomSkin();

        _botNameUI.SetMainCameraProvider(mainCameraProvider);
        _botNameUI.SetName(BotRandomNamesProvider.GetNextNickname());
    }
    
    public void SetPushed(bool isPushed)
    {
        _isPushed = isPushed;
    }

    public CharacterController Controller => _controller;
    public AnimationPlayback Animation => _animationPlayback;
    public BotHealth Health => _health;

    private void SetRandomSkin()
    {
        int randIndex = Random.Range(0, _skinsCatalogConfig.Skins.Count);
        var config = _skinsCatalogConfig.Skins[randIndex];
        _skins.ForEach(skin => skin.SetActive(false));
        _skins.First(skin => skin.gameObject.name == config.ChildNameUnderPlayerModel).SetActive(true);
        _currentSkinId = config.Id;
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += OnHealthDied;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= OnHealthDied;
        
        if (_ai != null)
            _ai.OnDied -= HandleAiDied;
    }

    private void Update()
    {
        if (_ai == null || _isPushed)
            return;
        
        _ai.Tick(Time.deltaTime);
    }

    public void TryTeleport(Vector3 worldPosition, bool resetVelocity)
    {
        if (resetVelocity)
            _motionSolver.ResetVerticalVelocity();

        _controller.enabled = false;
        transform.position = worldPosition;
        _controller.enabled = true;
    }

    public void FaceTowards(Vector3 directionWorld, float rotationSpeed)
    {
        if (directionWorld.sqrMagnitude <= _config.DirectionSqrEpsilon)
            return;

        Quaternion target = Quaternion.LookRotation(directionWorld, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
    }
    
    public void RestoreFullHealth() => _health?.RestoreToMax();
    
    public void NotifyDiedByEnvironment() => _ai.KillInstantly();
    
    private void OnHealthDied() => _ai.KillInstantly();
    
    private void HandleAiDied(BotAgent _) => OnDied?.Invoke(this);
}