using UnityEngine;

public class BotAI
{
    public event OnAgentDie OnDied;

    private readonly BotAiContext context;
    private readonly CheckpointProgressSync checkpointSync;
    private readonly WaypointNavigator navigator;
    private readonly WaypointActionResolver actionResolver;
    private readonly WaypointLocomotion locomotion;

    private BotStateType _state = BotStateType.Idle;

    public BotAI(BotAgent botAgent, MotionSolver motion, BotRespawn respawn, WaypointPath path, BotDataConfig config,
                 ActiveBots registry, int seed, BotLadderBridge ladder)
    {
        context = new BotAiContext(botAgent, motion, respawn, path, config, registry, seed, ladder);
        checkpointSync = new CheckpointProgressSync(context);
        navigator = new WaypointNavigator(context);
        actionResolver = new WaypointActionResolver(context);
        locomotion = new WaypointLocomotion(context);
    }

    public void Tick(float deltaTime)
    {
        checkpointSync.EnsureStartSync();
        checkpointSync.TickCooldown(deltaTime);
        
        if (context.Climb.IsClimbing)
        {
            _state = BotStateType.Climbing;
            actionResolver.TickClimb(deltaTime, out bool finished);
            
            if (finished)
            {
                navigator.Advance();
                
                _state = BotStateType.Idle;
            }
            
            return;
        }

        if (navigator.IsFinished)
        {
            if (_state != BotStateType.Finished)
            {
                _state = BotStateType.Finished;
                
                KillInstantly();
            }
            
            return;
        }

        Transform waypoint = navigator.Current;
        
        if (waypoint == null)
        {
            navigator.Advance();
            
            return;
        }
        
        if (navigator.IsWaiting)
        {
            _state = BotStateType.Waiting;
            
            context.Motion.IdlePose(deltaTime);
            navigator.TickWait(deltaTime);
            
            return;
        }

        if (checkpointSync.TryResyncIfNeeded(deltaTime, out Transform newWaypoint))
        {
            waypoint = newWaypoint;
            
            if (waypoint == null)
                return;
        }

        if (navigator.Reached(waypoint, out _))
        {
            if (navigator.TryEnterAndMaybeQueueWait(deltaTime, waypoint, out bool keepWaiting) && keepWaiting)
            {
                _state = BotStateType.Waiting;
                
                context.Motion.IdlePose(deltaTime);
                
                return;
            }

            WaypointActionResult result = actionResolver.HandleAtWaypoint(deltaTime, waypoint);
            
            switch (result)
            {
                case WaypointActionResult.BeganClimb:
                    _state = BotStateType.Climbing;
                    return;
                
                case WaypointActionResult.DidJumpAndAdvance:
                    _state = BotStateType.Jumping; navigator.Advance();
                    return;
                
                case WaypointActionResult.Sabotaged:
                    _state = context.Motion.IsGrounded ? BotStateType.Falling : BotStateType.Jumping;
                    return;
                
                case WaypointActionResult.WaitingForJump:
                    _state = BotStateType.Waiting;
                    return;
                
                default:
                    navigator.Advance(); _state = BotStateType.Idle;
                    return;
            }
        }

        _state = context.Motion.IsGrounded ? BotStateType.Moving : BotStateType.Falling;
        locomotion.MoveTowards(waypoint, deltaTime);
    }

    public void KillInstantly()
    {
        if (_state is BotStateType.Dead or BotStateType.Respawning or BotStateType.Finished)
            return;
        
        _state = BotStateType.Dead;
        
        OnDied?.Invoke(context.Agent);
    }
}

 // private const float ZERO_TOLERANCE = 0.0001f;
    // private const float RESYNC_Y_BELOW = 1.2f;
    // private const float RESYNC_FAR_XZ_SQR = 25f;
    // private const float RESYNC_COOLDOWN = 0.75f;
    //
    // private const float START_MAX_ABOVE = 0.80f;
    // private const float START_MAX_BELOW = 4.00f;
    //
    // private const float RESYNC_MAX_ABOVE = 0.60f;
    // private const float RESYNC_MAX_BELOW = 1000f;
    //
    // public event OnBotDie OnBotDied;
    //
    // private readonly BotDataConfig config;
    // private readonly BotAgent botAgent;
    // private readonly BotRespawn respawn;
    //
    // private readonly ActiveBots registry;
    // private readonly WaypointTrack track;
    // private readonly MotionAndPose motion;
    // private readonly ProximitySteering steer = new ProximitySteering();
    // private readonly JumpDecision jump;
    // private readonly ClimbActivator climb;
    // private readonly FailureDice failDice;
    // private readonly BotFailureDecider failureDecider;
    //
    // private BotStateType _state = BotStateType.Idle;
    // private Transform _waitingWaypoint;
    // private Transform _enteredWaypoint;
    //
    // private float _waitTimer;
    // private float _resyncCooldownTimer;
    // private bool _syncedToCheckpoint;
    //
    // public BotAI(BotAgent botAgent, MotionSolver motion, BotRespawn respawn, WaypointPath path, BotDataConfig config,
    //     ActiveBots registry, int seed, BotLadderBridge ladder)
    // {
    //     this.botAgent = botAgent;
    //     this.respawn = respawn;
    //     this.config = config;
    //     this.registry = registry;
    //     
    //     track = new WaypointTrack(path);
    //     GroundContactCheck ground = new GroundContactCheck(botAgent.Controller, config);
    //     jump = new JumpDecision(motion, config, ground);
    //     climb = new ClimbActivator(ladder);
    //     failDice = new FailureDice(seed, config, motion);
    //     this.motion = new MotionAndPose(botAgent, motion, config);
    //     failureDecider = new BotFailureDecider(failDice);
    //     
    //     if (this.respawn != null)
    //         this.respawn.OnRespawned += OnRespawnedHandled;
    // }
    //
    // public void Tick(float deltaTime)
    // {
    //     if (!_syncedToCheckpoint)
    //     {
    //         track.SyncToProgress(respawn.LastCheckpointProgress);
    //         
    //         Vector3 botPosition = botAgent.Controller.transform.position;
    //         bool snapped = track.SyncToNearestInYBand(botPosition, START_MAX_ABOVE, START_MAX_BELOW);
    //         
    //         if (!snapped)
    //         {
    //             // track.SyncToNearest(pos);
    //         }
    //
    //         _syncedToCheckpoint = true;
    //         
    //         D($"Sync start: yCP={respawn.LastCheckpointProgress:F3}, wp='{Name(track.Current)}'");
    //     }
    //     
    //     if (_resyncCooldownTimer > 0f)
    //         _resyncCooldownTimer -= deltaTime;
    //
    //     if (climb.IsClimbing)
    //     {
    //         _state = BotStateType.Climbing;
    //         climb.Tick(deltaTime);
    //         climb.TryExitIfGroundClose();
    //
    //         bool grounded = botAgent.Controller != null && botAgent.Controller.isGrounded;
    //         botAgent.Animation.UpdateAnimation(Vector3.zero, grounded);
    //
    //         if (!climb.IsClimbing)
    //         {
    //             D($"Climb finished → Advance from '{Name(track.Current)}'");
    //             track.Advance();
    //             _state = BotStateType.Idle;
    //             motion.IdlePose(deltaTime);
    //             failureDecider.ResetOnAdvance();
    //         }
    //         return;
    //     }
    //
    //     if (_state is BotStateType.Dead or BotStateType.Respawning)
    //         return;
    //
    //     if (track.IsFinished)
    //     {
    //         if (_state != BotStateType.Finished)
    //         {
    //             D("Track finished");
    //             // чтобы не висели на месте у финиша — сразу «завершаем»
    //             _state = BotStateType.Finished;
    //             KillInstantly();
    //         }
    //         return;
    //     }
    //
    //     Transform waypointTransform = track.Current;
    //     if (waypointTransform == null)
    //     {
    //         D("Current waypoint is null → Advance");
    //         track.Advance();
    //         motion.IdlePose(deltaTime);
    //         failureDecider.ResetOnAdvance();
    //         return;
    //     }
    //
    //     if (_waitTimer > 0f)
    //     {
    //         _waitTimer -= deltaTime;
    //         if (_waitTimer <= 0f)
    //         {
    //             D($"Wait done at '{Name(_waitingWaypoint)}' → Advance");
    //             _waitingWaypoint = null;
    //             track.Advance();
    //             _state = BotStateType.Idle;
    //             motion.IdlePose(deltaTime);
    //             failureDecider.ResetOnAdvance();
    //             return;
    //         }
    //
    //         _state = BotStateType.Waiting;
    //         motion.IdlePose(deltaTime);
    //         return;
    //     }
    //
    //     waypointTransform.TryGetComponent(out Waypoint waypoint);
    //     float reachedRadius = config.ReachedDistance;
    //     if (waypoint != null && waypoint.WaitBeforeProceedSeconds > 0f)
    //         reachedRadius = Mathf.Max(reachedRadius, waypoint.WaitAreaRadius);
    //
    //     float targetY = waypointTransform.position.y;
    //     float selfY   = botAgent.Controller.transform.position.y;
    //
    //     Vector3 toTarget = waypointTransform.position - botAgent.Controller.transform.position;
    //     Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
    //     float distance = flat.magnitude;
    //
    //     bool belowCurrent = (selfY + RESYNC_Y_BELOW) < targetY;
    //     bool farInXZ = (flat.sqrMagnitude > RESYNC_FAR_XZ_SQR);
    //
    //     if (_resyncCooldownTimer <= 0f && motion.IsGrounded && (belowCurrent || farInXZ))
    //     {
    //         Vector3 pos = botAgent.Controller.transform.position;
    //         
    //         bool snapped = track.SyncToNearestInYBand(pos, RESYNC_MAX_ABOVE, RESYNC_MAX_BELOW);
    //         
    //         if (!snapped)
    //             track.SyncToNearest(pos);
    //
    //         failureDecider.ResetOnAdvance();
    //         _resyncCooldownTimer = RESYNC_COOLDOWN;
    //
    //         D($"ResyncToNearest(Y-band) at pos=({pos.x:F2},{pos.y:F2},{pos.z:F2}) → wp='{Name(track.Current)}'");
    //
    //         waypointTransform = track.Current;
    //         
    //         if (waypointTransform == null)
    //             return;
    //
    //         waypointTransform.TryGetComponent(out waypoint);
    //         toTarget = waypointTransform.position - botAgent.Controller.transform.position;
    //         flat = new Vector3(toTarget.x, 0f, toTarget.z);
    //     }
    //
    //     if (distance <= reachedRadius)
    //     {
    //         if (_enteredWaypoint != waypointTransform)
    //         {
    //             _enteredWaypoint = waypointTransform;
    //             D($"Entered WP '{Name(waypointTransform)}'");
    //             failureDecider.OnEnteredWaypoint(waypointTransform, waypoint != null ? waypoint.FailProbability : 0f);
    //             if (waypoint != null)
    //                 D($"Roll at '{Name(waypointTransform)}': p={waypoint.FailProbability:F2} → fail={(failureDecider.HasPendingSabotage ? "True" : "False")}");
    //         }
    //
    //         if (waypoint != null && waypoint.RequireClimb && climb.TryBegin(waypoint.ClimbFacing, waypoint.ClimbLateralBounds))
    //         {
    //             D($"Begin climb at '{Name(waypointTransform)}'");
    //             _state = BotStateType.Climbing;
    //             motion.IdlePose(deltaTime);
    //             return;
    //         }
    //
    //         jump.TickCooldown(deltaTime);
    //
    //         if (failureDecider.HasPendingSabotage)
    //         {
    //             if (waypoint != null && waypoint.RequireJump)
    //             {
    //                 bool jumped = jump.TryJumpNow(botAgent.Animation);
    //                 if (!jumped)
    //                 {
    //                     bool forced = jump.ForceJumpIfGrounded(botAgent.Animation);
    //                     D($"FAIL jump at '{Name(waypointTransform)}': TryJumpNow={jumped}, ForceJump={forced}");
    //                 }
    //                 else
    //                 {
    //                     D($"FAIL jump at '{Name(waypointTransform)}': started normally");
    //                 }
    //
    //                 failDice.ApplyFatalKick(botAgent);
    //                 failureDecider.MarkSabotageApplied();
    //                 _state = motion.IsGrounded ? BotStateType.Falling : BotStateType.Jumping;
    //                 motion.IdlePose(deltaTime);
    //                 return;
    //             }
    //             else
    //             {
    //                 D($"FAIL (non-jump) at '{Name(waypointTransform)}' → sabotage fall");
    //                 failDice.ApplyFatalKick(botAgent);
    //                 failureDecider.MarkSabotageApplied();
    //                 _state = BotStateType.Falling;
    //                 motion.IdlePose(deltaTime);
    //                 return;
    //             }
    //         }
    //
    //         if (waypoint != null && waypoint.RequireJump)
    //         {
    //             if (jump.TryJumpNow(botAgent.Animation))
    //             {
    //                 D($"Jump OK at '{Name(waypointTransform)}' → Advance");
    //                 _state = BotStateType.Jumping;
    //                 track.Advance();
    //                 motion.IdlePose(deltaTime);
    //                 failureDecider.ResetOnAdvance();
    //                 return;
    //             }
    //
    //             _state = BotStateType.Waiting;
    //             motion.IdlePose(deltaTime);
    //             return;
    //         }
    //
    //         if (waypoint != null && waypoint.WaitBeforeProceedSeconds > 0f)
    //         {
    //             if (_waitingWaypoint != waypointTransform)
    //             {
    //                 _waitingWaypoint = waypointTransform;
    //                 _waitTimer = waypoint.WaitBeforeProceedSeconds;
    //                 D($"Begin wait {waypoint.WaitBeforeProceedSeconds:F2}s at '{Name(waypointTransform)}'");
    //             }
    //
    //             _state = BotStateType.Waiting;
    //             motion.IdlePose(deltaTime);
    //             return;
    //         }
    //
    //         D($"Advance from '{Name(waypointTransform)}'");
    //         track.Advance();
    //         _state = BotStateType.Idle;
    //         motion.IdlePose(deltaTime);
    //         failureDecider.ResetOnAdvance();
    //         return;
    //     }
    //
    //     Vector3 direction = (flat.sqrMagnitude > ZERO_TOLERANCE) ? flat.normalized : Vector3.zero;
    //     Vector3 separation = steer.ComputeSeparation(botAgent, registry.Active, config.MinDistanceBetweenBots);
    //     if (separation.sqrMagnitude > 0f)
    //     {
    //         Vector3 blend = direction + separation * config.SeparationWeight;
    //         if (blend.sqrMagnitude > ZERO_TOLERANCE)
    //             direction = blend.normalized;
    //     }
    //
    //     botAgent.FaceTowards(direction, config.RotationSpeed);
    //     _state = motion.IsGrounded ? BotStateType.Moving : BotStateType.Falling;
    //
    //     jump.TickCooldown(deltaTime);
    //     motion.MoveAndAnimate(direction, deltaTime);
    // }
    //
    // public void KillInstantly()
    // {
    //     if (_state is BotStateType.Dead or BotStateType.Respawning or BotStateType.Finished)
    //         return;
    //
    //     _state = BotStateType.Dead;
    //     
    //     D("Killed instantly");
    //     
    //     OnBotDied?.Invoke(botAgent);
    // }
    //
    // private void OnRespawnedHandled(BotAgent bot, Transform target)
    // {
    //     track.SyncToProgress(respawn.LastCheckpointProgress);
    //     failureDecider.ResetOnAdvance();
    //     _syncedToCheckpoint = true;
    //     _resyncCooldownTimer = 0f;
    //     
    //     D($"Respawned → resync to y={respawn.LastCheckpointProgress:F3}, wp='{Name(track.Current)}'");
    // }
    //
    // private void D(string msg)
    // {
    //     Debug.Log($"[BotAI] {botAgent.name} | {msg}");
    // }
    //
    // private string Name(Transform t) => t != null ? t.name : "(null)";