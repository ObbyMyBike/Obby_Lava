// using UnityEngine;
//
// public class LadderClimbMotor
// {
//     public event OnClimbExit OnClimbExited;
//
//     private readonly CharacterController controller;
//     private readonly Transform playerTransform;
//     private readonly PlayerAnimationPlayback animation;
//
//     private readonly LadderInput input = new LadderInput();
//     private readonly LadderPhysics physics;
//     private readonly LadderColliderAdapter colliderAdapter;
//     private readonly LadderCollisionGate gate;
//     private readonly LadderExitPlanner exit;
//
//     private readonly LadderConfig config;
//     private readonly float climbSpeed;
//
//     private LadderState _state = new LadderState();
//
//     public LadderClimbMotor(CharacterController controller, Transform playerTransform, float climbSpeed, PlayerAnimationPlayback animation,
//         LayerMask ladderMask, LayerMask groundMask, LadderConfig config = default)
//     {
//         this.controller = controller;
//         this.playerTransform = playerTransform;
//         this.climbSpeed = climbSpeed;
//         this.animation = animation;
//         
//         LadderConfig ladderConfig = config;
//         
//         if (ladderConfig.LooksUninitialized())
//             ladderConfig = config.CreateDefault();
//
//         this.config = ladderConfig;
//
//         physics = new LadderPhysics(controller, playerTransform, ladderMask, groundMask, this.config);
//         colliderAdapter = new LadderColliderAdapter(controller, this.config);
//         gate = new LadderCollisionGate(controller, playerTransform, ladderMask, this.config);
//         exit = new LadderExitPlanner(controller, physics, groundMask, this.config);
//
//         gate.SetupLayers(_state);
//     }
//
//     public bool IsClimbing => _state.IsClimbing;
//     public bool CanEnterNow => _state.ReenterBlockTimer <= 0f;
//
//     public void UpdateNonClimbingStateTimers(float delta)
//     {
//         if (_state.ReenterBlockTimer > 0f)
//         {
//             _state.ReenterBlockTimer = Mathf.Max(0f, _state.ReenterBlockTimer - delta);
//         }
//
//         colliderAdapter.TickLandingBoost(_state, delta);
//         gate.TickIgnore(_state, delta);
//         physics.TickPostExitWatchdog(_state);
//     }
//
//     public void Enter(Transform ladderFacing)
//     {
//         if (!CanEnterNow)
//             return;
//
//         gate.EnsureNotIgnoring(_state);
//
//         _state.LadderFacing = ladderFacing;
//         _state.ResetOnEnter();
//
//         colliderAdapter.ApplyClimbCollider(_state);
//
//         animation?.SetClimbState(true);
//         animation?.SetClimbCycleSpeed(0f);
//         animation?.SetClimbSignedSpeed(0f);
//
//         AlignToLadderPlane();
//     }
//
//     public void Exit(Transform ladderFacing)
//     {
//         if (_state.LadderFacing == ladderFacing)
//             ForceExit();
//     }
//
//     public void Tick(Vector3 inputMoveDirWorld, float delta)
//     {
//         if (!_state.IsClimbing || _state.LadderFacing == null)
//         {
//             UpdateNonClimbingStateTimers(delta);
//             
//             return;
//         }
//
//         _state.TimeSinceEnter += delta;
//
//         if (!input.TryRead(_state.LadderFacing, inputMoveDirWorld, config.MinInputMagnitude, out Vector3 local, out Result result))
//         {
//             _state.ExitIntentTimer = 0f;
//             _state.AheadMissTimer  = 0f;
//             
//             animation?.SetClimbSignedSpeed(0f);
//             animation?.SetClimbCycleSpeed(0f);
//             
//             return;
//         }
//
//         float into = Mathf.Clamp01(-local.z);
//
//         if (input.SideExitIntent(ref _state, config, local, into))
//         {
//             ForceExit();
//             
//             return;
//         }
//
//         bool hasAhead = physics.HasLadderAhead(_state.LadderFacing, out _);
//
//         if (result.MovingUp)
//         {
//             if (exit.TrySoftTopExit(_state.LadderFacing))
//             {
//                 physics.SnapFeetToGroundIfCloseSafe();
//                 
//                 ForceExitWithCustomBlock(config.TopReenterBlockSeconds);
//                 return;
//             }
//
//             _state.AheadMissTimer = hasAhead ? 0f : _state.AheadMissTimer + delta;
//
//             if (_state.AheadMissTimer >= config.AheadMissDebounce)
//             {
//                 Vector3 nudge = (-_state.LadderFacing.forward * config.TopExitForwardNudgeFallback) + (Vector3.up * config.TopExitExtraUpNudge);
//                 controller.Move(nudge);
//                 physics.SnapFeetToGroundIfCloseSafe();
//                 
//                 ForceExit();
//                 
//                 return;
//             }
//         }
//
//         if (result.MovingDown)
//         {
//             float gd = physics.GetGroundDistance(out _);
//
//             if (controller.isGrounded || (gd >= 0f && gd <= config.BottomGroundThreshold))
//             {
//                 ForceExit();
//                 return;
//             }
//         }
//
//         controller.Move(Vector3.up * (result.ClimbSigned * climbSpeed * delta));
//         animation?.SetClimbCycleSpeed(Mathf.Abs(result.ClimbSigned));
//         animation?.SetClimbSignedSpeed(result.ClimbSigned);
//     }
//
//     private void ForceExitWithCustomBlock(float reenterBlockSeconds)
//     {
//         if (!_state.IsClimbing) return;
//
//         Transform prevFacing = _state.LadderFacing;
//
//         _state.ResetOnExit();
//         colliderAdapter.RestoreOriginalCollider(_state);
//         physics.SnapFeetToGroundIfCloseSafe();
//         colliderAdapter.BoostLandingStability(_state);
//
//         gate.BeginIgnore(_state, config.TopExitIgnoreSeconds);
//         _state.ReenterBlockTimer = Mathf.Max(reenterBlockSeconds, 0f);
//
//         animation?.SetClimbState(false);
//         animation?.SetClimbCycleSpeed(0f);
//         animation?.SetClimbSignedSpeed(0f);
//
//         physics.ArmPostExitWatchdog(_state);
//         
//         OnClimbExited?.Invoke(prevFacing);
//     }
//
//     private void ForceExit() => ForceExitWithCustomBlock(0.20f);
//
//     private void AlignToLadderPlane()
//     {
//         if (_state.LadderFacing == null)
//             return;
//
//         Vector3 faceDirection = -_state.LadderFacing.forward; faceDirection.y = 0f;
//
//         if (faceDirection.sqrMagnitude > 1e-4f)
//             playerTransform.rotation = Quaternion.LookRotation(faceDirection);
//     }
//     
//     //  private const float TOP_PLATFORM_FORWARD_CHECK_DISTANCE = 0.90f;
//     // private const float TOP_PLATFORM_UP_OFFSET = 1.05f;
//     // private const float TOP_PLATFORM_DOWNCAST_DISTANCE = 2.10f;
//     // private const float TOP_PLATFORM_MIN_UP_DOT = 0.85f;
//     // private const float TOP_PLATFORM_EXIT_FORWARD_NUDGE = 0.30f;
//     // private const float TOP_PLATFORM_EXIT_EXTRA_UP_NUDGE = 0.12f;
//     // private const float TOP_REENTER_BLOCK_SECONDS = 0.90f;
//     // private const float TOP_EXIT_UP_EXTRA_CLEARANCE = 0.10f;
//     // private const float LANDING_SLOPE_LIMIT = 80f;
//     // private const float LANDING_SLOPE_BOOST_SECONDS = 0.35f;
//     //
//     // private const float REENTER_BLOCK_SECONDS = 0.20f;
//     //
//     // private const float CONTROLLER_HEIGHT = 2.60f;
//     // private const float NORMAL_COLLIDER_CENTER_Y = 1.31f;
//     // private const float CLIMB_COLLIDER_CENTER_Y = 2.20f;
//     //
//     // private const float EXIT_SIDE_DOT_THRESHOLD = 0.70f;
//     // private const float EXIT_SIDE_HARD_THRESHOLD = 0.95f;
//     // private const float EXIT_DEBOUNCE_SECONDS = 0.12f;
//     // private const float GRACE_AFTER_ENTER_SECONDS = 0.25f;
//     //
//     // private const float CLIMB_STICK_POS_THRESHOLD = 0.25f;
//     // private const float MIN_INPUT_MAGNITUDE = 0.05f;
//     //
//     // private const float SNAP_EPSILON = 0.0025f;
//     // private const float MAX_SNAP_PER_TICK = 0.06f;
//     // private const float DESIRED_Z_OFFSET = 0.36f;
//     //
//     // private const float AHEAD_CHECK_UP_OFFSET = 1.0f;
//     // private const float AHEAD_CHECK_RADIUS_SCALE = 0.45f;
//     // private const float AHEAD_HIT_DISTANCE_OK = 0.04f;
//     // private const float TOP_EXIT_FORWARD_NUDGE_FALLBACK = 0.34f;
//     // private const float AHEAD_MISS_DEBOUNCE = 0.10f;
//     //
//     // private const float BOTTOM_GROUND_THRESHOLD = 0.14f;
//     // private const float GROUND_RAY_EXTRA = 0.30f;
//     //
//     // private const float CLIMB_STEP_OFFSET = 0.70f;
//     // private const float GROUND_SNAP_MAX_DISTANCE = 0.45f;
//     //
//     // private const bool ENABLE_POST_EXIT_WATCHDOG = true;
//     // private const float POST_EXIT_WATCHDOG_SECONDS = 0.35f;
//     // private const float WATCHDOG_EXTRA_DOWNCAST = 0.75f;
//     // private const float WATCHDOG_UP_NUDGE = 0.10f;
//     // private const float WATCHDOG_FORWARD_NUDGE = 0.10f;
//     //
//     // private const float TOP_EXIT_IGNORE_SECONDS = 0.55f;
//     // private const float FORWARD_CLEAR_EPS = 0.08f;
//     // private const float WARP_SKIN_PAD = 0.02f;
//     //
//     // public event OnClimbExit OnClimbExited;
//     //
//     // private readonly CharacterController controller;
//     // private readonly Transform playerTransform;
//     // private readonly PlayerAnimationPlayback animation;
//     // private readonly float climbSpeed;
//     //
//     // private readonly LayerMask ladderMask;
//     // private readonly LayerMask groundMask;
//     //
//     // private Transform _ladderFacing;
//     // private float _ignoreLadderTimer = 0f;
//     // private float _cachedStepOffset = 0f;
//     // private float _exitIntentTimerSeconds = 0f;
//     // private float _timeSinceEnter = 0f;
//     // private float _aheadMissTimer = 0f;
//     // private float _reenterBlockTimer = 0f;
//     // private float _postExitWatchdogTimer = 0f;
//     // private float _landingSlopeBoostTimer = 0f;
//     // private float _cachedSlopeLimit = 0f;
//     //
//     // private bool _isClimbing;
//     // private bool _ladderCollisionIgnored = false;
//     //
//     // private int _playerLayer = -1;
//     // private int _ladderLayer = -1;
//     //
//     // public LadderClimbMotor(CharacterController controller, Transform playerTransform, float climbSpeed,
//     //     PlayerAnimationPlayback animation, LayerMask ladderMask, LayerMask groundMask)
//     // {
//     //     this.controller = controller;
//     //     this.playerTransform = playerTransform;
//     //     this.climbSpeed = climbSpeed;
//     //     this.animation = animation;
//     //     this.ladderMask = ladderMask;
//     //     this.groundMask = groundMask;
//     //
//     //     _playerLayer = playerTransform.gameObject.layer;
//     //     _ladderLayer = FirstLayerFromMask(ladderMask);
//     // }
//     //
//     // public bool IsClimbing => _isClimbing;
//     // public bool CanEnterNow => _reenterBlockTimer <= 0f;
//     //
//     // public void UpdateNonClimbingStateTimers(float deltaTime)
//     // {
//     //     if (_reenterBlockTimer > 0f)
//     //         _reenterBlockTimer = Mathf.Max(0f, _reenterBlockTimer - deltaTime);
//     //
//     //     if (_landingSlopeBoostTimer > 0f)
//     //     {
//     //         _landingSlopeBoostTimer = Mathf.Max(0f, _landingSlopeBoostTimer - deltaTime);
//     //         if (_landingSlopeBoostTimer <= 0f)
//     //             controller.slopeLimit = _cachedSlopeLimit;
//     //     }
//     //
//     //     if (_ladderCollisionIgnored)
//     //     {
//     //         _ignoreLadderTimer = Mathf.Max(0f, _ignoreLadderTimer - deltaTime);
//     //         if (_ignoreLadderTimer <= 0f)
//     //             EndIgnoreLadderCollisions();
//     //     }
//     //
//     //     if (ENABLE_POST_EXIT_WATCHDOG && _postExitWatchdogTimer > 0f)
//     //     {
//     //         _postExitWatchdogTimer = Mathf.Max(0f, _postExitWatchdogTimer - deltaTime);
//     //
//     //         if (controller.isGrounded) return;
//     //
//     //         Bounds b = controller.bounds;
//     //         Vector3 feet = new Vector3(b.center.x, b.min.y + controller.skinWidth + 0.01f, b.center.z);
//     //
//     //         if (Physics.Raycast(feet, Vector3.down, out RaycastHit hit, WATCHDOG_EXTRA_DOWNCAST, groundMask, QueryTriggerInteraction.Ignore))
//     //         {
//     //             float desiredFeetY = hit.point.y + controller.skinWidth + 0.02f;
//     //             float currentFeetY = b.min.y;
//     //             float upDelta = Mathf.Max(0f, desiredFeetY - currentFeetY);
//     //
//     //             if (upDelta > 0f)
//     //             {
//     //                 Vector3 forward = (_ladderFacing == null ? playerTransform.forward : -_ladderFacing.forward);
//     //                 Vector3 nudge = (Vector3.up * Mathf.Min(upDelta, WATCHDOG_UP_NUDGE)) + (forward * WATCHDOG_FORWARD_NUDGE);
//     //                 controller.Move(nudge);
//     //             }
//     //         }
//     //     }
//     // }
//     //
//     // public void Enter(Transform ladderFacing)
//     // {
//     //     if (!CanEnterNow) return;
//     //
//     //     _ladderFacing = ladderFacing;
//     //     _isClimbing = true;
//     //     _exitIntentTimerSeconds = 0f;
//     //     _timeSinceEnter = 0f;
//     //     _aheadMissTimer = 0f;
//     //
//     //     CacheAndApplyClimbCollider();
//     //
//     //     animation?.SetClimbState(true);
//     //     animation?.SetClimbCycleSpeed(0f);
//     //     animation?.SetClimbSignedSpeed(0f);
//     //
//     //     AlignToLadderPlane();
//     // }
//     //
//     // public void Exit(Transform ladderFacing)
//     // {
//     //     if (_ladderFacing == ladderFacing)
//     //         ForceExit("[LADDER] EXIT by volume");
//     // }
//     //
//     // public void Tick(Vector3 inputMoveDirectionWorld, float deltaTime)
//     // {
//     //     if (_reenterBlockTimer > 0f)
//     //         _reenterBlockTimer = Mathf.Max(0f, _reenterBlockTimer - deltaTime);
//     //
//     //     if (!_isClimbing || _ladderFacing == null)
//     //         return;
//     //
//     //     _timeSinceEnter += deltaTime;
//     //
//     //     Vector3 planarInput = Vector3.ProjectOnPlane(inputMoveDirectionWorld, Vector3.up);
//     //     Vector3 localInput = _ladderFacing.InverseTransformDirection(planarInput);
//     //
//     //     if (localInput.sqrMagnitude < MIN_INPUT_MAGNITUDE * MIN_INPUT_MAGNITUDE)
//     //     {
//     //         _exitIntentTimerSeconds = 0f;
//     //         _aheadMissTimer = 0f;
//     //
//     //         SnapOutwardsToLadder();
//     //
//     //         animation?.SetClimbSignedSpeed(0f);
//     //         animation?.SetClimbCycleSpeed(0f);
//     //         return;
//     //     }
//     //
//     //     localInput.Normalize();
//     //
//     //     float intoLadder = Mathf.Clamp01(-localInput.z);
//     //     float awayFromLadder = Mathf.Clamp01(+localInput.z);
//     //     float sideAbs = Mathf.Abs(Mathf.Clamp(localInput.x, -1f, 1f));
//     //     float climbSigned = Mathf.Clamp(intoLadder - awayFromLadder, -1f, 1f);
//     //
//     //     bool graceActive = _timeSinceEnter < GRACE_AFTER_ENTER_SECONDS;
//     //     float effectiveSideThr = (intoLadder > CLIMB_STICK_POS_THRESHOLD) ? EXIT_SIDE_HARD_THRESHOLD : EXIT_SIDE_DOT_THRESHOLD;
//     //     bool sideExitIntent = !graceActive && (sideAbs >= effectiveSideThr);
//     //
//     //     if (sideExitIntent)
//     //     {
//     //         _exitIntentTimerSeconds += deltaTime;
//     //         if (_exitIntentTimerSeconds >= EXIT_DEBOUNCE_SECONDS)
//     //         {
//     //             ForceExit("[LADDER] EXIT by side intent");
//     //             return;
//     //         }
//     //     }
//     //     else
//     //     {
//     //         _exitIntentTimerSeconds = 0f;
//     //     }
//     //
//     //     float aheadDist;
//     //     bool hasAhead = HasLadderAhead(out aheadDist);
//     //
//     //     bool movingUp = climbSigned > +0.10f;
//     //     bool movingDown = climbSigned < -0.10f;
//     //
//     //     if (movingUp)
//     //     {
//     //         string topExitReason;
//     //
//     //         if (TrySoftTopExit_WithCapsuleFit(out topExitReason))
//     //         {
//     //             SnapFeetToGroundIfClose_Safe();
//     //             ForceExitWithCustomBlock($"[LADDER] EXIT by top platform | {topExitReason}", TOP_REENTER_BLOCK_SECONDS);
//     //             return;
//     //         }
//     //
//     //         if (!hasAhead) _aheadMissTimer += deltaTime; else _aheadMissTimer = 0f;
//     //
//     //         if (_aheadMissTimer >= AHEAD_MISS_DEBOUNCE)
//     //         {
//     //             Vector3 nudge = (-_ladderFacing.forward * TOP_EXIT_FORWARD_NUDGE_FALLBACK) + (Vector3.up * TOP_PLATFORM_EXIT_EXTRA_UP_NUDGE);
//     //             controller.Move(nudge);
//     //             SnapFeetToGroundIfClose_Safe();
//     //             ForceExit(null);
//     //             return;
//     //         }
//     //     }
//     //
//     //     float groundDist = GetGroundDistance(out _);
//     //     if (movingDown)
//     //     {
//     //         if (controller.isGrounded || (groundDist >= 0f && groundDist <= BOTTOM_GROUND_THRESHOLD))
//     //         {
//     //             ForceExit(null);
//     //             return;
//     //         }
//     //     }
//     //
//     //     Vector3 climbDelta = Vector3.up * (climbSigned * climbSpeed * deltaTime);
//     //     controller.Move(climbDelta);
//     //
//     //     SnapOutwardsToLadder();
//     //
//     //     animation?.SetClimbCycleSpeed(Mathf.Abs(climbSigned));
//     //     animation?.SetClimbSignedSpeed(climbSigned);
//     // }
//     //
//     // private bool TrySoftTopExit_WithCapsuleFit(out string reason)
//     // {
//     //     reason = string.Empty;
//     //     if (_ladderFacing == null) { reason = "no ladderFacing"; return false; }
//     //
//     //     Vector3 start = controller.bounds.center + Vector3.up * TOP_PLATFORM_UP_OFFSET;
//     //     Vector3 forwardToPlatform = -_ladderFacing.forward;
//     //     Vector3 probe = start + forwardToPlatform * TOP_PLATFORM_FORWARD_CHECK_DISTANCE;
//     //
//     //     if (!Physics.Raycast(probe, Vector3.down, out RaycastHit downHit, TOP_PLATFORM_DOWNCAST_DISTANCE, groundMask, QueryTriggerInteraction.Ignore))
//     //     { reason = "no ground below probe"; return false; }
//     //
//     //     float upDot = Vector3.Dot(downHit.normal, Vector3.up);
//     //     if (upDot < TOP_PLATFORM_MIN_UP_DOT)
//     //     { reason = $"bad ground normal (upDot={upDot:F2})"; return false; }
//     //
//     //     float skin = controller.skinWidth;
//     //     Bounds b = controller.bounds;
//     //     float currentFeetY = b.min.y;
//     //     float desiredFeetY = downHit.point.y + skin + 0.02f;
//     //
//     //     float upDelta = Mathf.Max(desiredFeetY - currentFeetY + TOP_EXIT_UP_EXTRA_CLEARANCE, TOP_PLATFORM_EXIT_EXTRA_UP_NUDGE);
//     //     controller.Move(Vector3.up * upDelta);
//     //
//     //     float minForwardClear = controller.radius + FORWARD_CLEAR_EPS;
//     //     float forwardClear = Mathf.Max(minForwardClear, TOP_PLATFORM_EXIT_FORWARD_NUDGE);
//     //
//     //     Vector3 targetCenter = controller.bounds.center + forwardToPlatform * forwardClear;
//     //
//     //     float capsuleHalf = (controller.height * 0.5f) - controller.radius;
//     //     Vector3 capBottom = targetCenter + Vector3.down * capsuleHalf;
//     //     Vector3 capTop = targetCenter + Vector3.up * capsuleHalf;
//     //     float capRadius = controller.radius - WARP_SKIN_PAD;
//     //
//     //     bool obstructed = Physics.CheckCapsule(capBottom, capTop, capRadius, groundMask, QueryTriggerInteraction.Ignore);
//     //     if (obstructed)
//     //     {
//     //         reason = "capsule obstructed on platform";
//     //         controller.Move(Vector3.down * Mathf.Min(upDelta, 0.06f));
//     //         return false;
//     //     }
//     //
//     //     controller.Move(forwardToPlatform * forwardClear);
//     //
//     //     reason = $"platform='{downHit.collider.name}' up={upDelta:F3} fwd={forwardClear:F3}";
//     //     return true;
//     // }
//     //
//     // private void ForceExitWithCustomBlock(string reason, float reenterBlockSeconds)
//     // {
//     //     if (!_isClimbing) return;
//     //
//     //     Transform prevFacing = _ladderFacing;
//     //     _isClimbing = false;
//     //     _ladderFacing = null;
//     //     _exitIntentTimerSeconds = 0f;
//     //     _timeSinceEnter = 0f;
//     //     _aheadMissTimer = 0f;
//     //
//     //     RestoreOriginalCollider();
//     //     SnapFeetToGroundIfClose_Safe();
//     //     BoostLandingStability();
//     //     BeginIgnoreLadderCollisions(TOP_EXIT_IGNORE_SECONDS);
//     //
//     //     _reenterBlockTimer = Mathf.Max(reenterBlockSeconds, 0f);
//     //
//     //     animation?.SetClimbState(false);
//     //     animation?.SetClimbCycleSpeed(0f);
//     //     animation?.SetClimbSignedSpeed(0f);
//     //
//     //     ArmPostExitWatchdog();
//     //
//     //     OnClimbExited?.Invoke(prevFacing);
//     // }
//     //
//     // private void ForceExit(string reason)
//     // {
//     //     ForceExitWithCustomBlock(reason, REENTER_BLOCK_SECONDS);
//     // }
//     //
//     // private void ArmPostExitWatchdog()
//     // {
//     //     if (ENABLE_POST_EXIT_WATCHDOG && !controller.isGrounded)
//     //         _postExitWatchdogTimer = POST_EXIT_WATCHDOG_SECONDS;
//     // }
//     //
//     // private void CacheAndApplyClimbCollider()
//     // {
//     //     _cachedStepOffset = controller.stepOffset;
//     //     controller.stepOffset = CLIMB_STEP_OFFSET;
//     //     _cachedSlopeLimit = controller.slopeLimit;
//     //
//     //     controller.height = CONTROLLER_HEIGHT;
//     //     var center = controller.center;
//     //     center.y = CLIMB_COLLIDER_CENTER_Y;
//     //     controller.center = center;
//     // }
//     //
//     // private void RestoreOriginalCollider()
//     // {
//     //     controller.stepOffset = _cachedStepOffset;
//     //
//     //     controller.height = CONTROLLER_HEIGHT;
//     //     Vector3 center = controller.center;
//     //     center.y = NORMAL_COLLIDER_CENTER_Y;
//     //     controller.center = center;
//     // }
//     //
//     // private void BoostLandingStability()
//     // {
//     //     controller.slopeLimit = LANDING_SLOPE_LIMIT;
//     //     _landingSlopeBoostTimer = LANDING_SLOPE_BOOST_SECONDS;
//     // }
//     //
//     // private void AlignToLadderPlane()
//     // {
//     //     if (_ladderFacing == null) return;
//     //
//     //     Vector3 faceDir = -_ladderFacing.forward; faceDir.y = 0f;
//     //     if (faceDir.sqrMagnitude > 1e-4f)
//     //         playerTransform.rotation = Quaternion.LookRotation(faceDir);
//     //
//     //     SnapOutwardsToLadder();
//     // }
//     //
//     // private float SnapOutwardsToLadder()
//     // {
//     //     Vector3 normal = _ladderFacing.forward;
//     //     Vector3 fromLadderToPlayer = playerTransform.position - _ladderFacing.position;
//     //
//     //     float curr = Vector3.Dot(fromLadderToPlayer, normal);
//     //     float target = DESIRED_Z_OFFSET;
//     //     float delta = target - curr;
//     //
//     //     if (delta > SNAP_EPSILON)
//     //     {
//     //         float step = Mathf.Min(delta, MAX_SNAP_PER_TICK);
//     //         Vector3 correction = normal * step;
//     //         controller.Move(correction);
//     //         return step;
//     //     }
//     //     return 0f;
//     // }
//     //
//     // private bool HasLadderAhead(out float hitDistance)
//     // {
//     //     hitDistance = -1f;
//     //
//     //     if (_ladderFacing == null)
//     //         return false;
//     //
//     //     float radius = controller.radius * AHEAD_CHECK_RADIUS_SCALE;
//     //     Vector3 origin = playerTransform.position + Vector3.up * AHEAD_CHECK_UP_OFFSET;
//     //
//     //     Vector3 toLadder = (_ladderFacing.position - origin);
//     //     float distToLadder = toLadder.magnitude;
//     //     if (distToLadder < 1e-4f)
//     //         return true;
//     //
//     //     Vector3 dir = toLadder / distToLadder;
//     //     float castDistance = Mathf.Max(distToLadder + 0.15f, DESIRED_Z_OFFSET + 0.50f);
//     //
//     //     bool hitSmth = Physics.SphereCast(origin, radius, dir, out RaycastHit hit, castDistance, ladderMask,
//     //         QueryTriggerInteraction.Collide);
//     //
//     //     if (hitSmth)
//     //     {
//     //         bool same = hit.transform == _ladderFacing || hit.transform.IsChildOf(_ladderFacing) || _ladderFacing.IsChildOf(hit.transform);
//     //         if (same)
//     //         {
//     //             hitDistance = hit.distance;
//     //             return hit.distance <= (distToLadder + AHEAD_HIT_DISTANCE_OK);
//     //         }
//     //     }
//     //     return false;
//     // }
//     //
//     // private float GetGroundDistance(out RaycastHit hit)
//     // {
//     //     Bounds b = controller.bounds;
//     //     Vector3 feet = new Vector3(b.center.x, b.min.y + controller.skinWidth + 0.01f, b.center.z);
//     //     float rayLen = GROUND_RAY_EXTRA + 0.01f;
//     //     if (Physics.Raycast(feet, Vector3.down, out hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
//     //         return hit.distance;
//     //     return -1f;
//     // }
//     //
//     // private void SnapFeetToGroundIfClose_Safe()
//     // {
//     //     if (controller.isGrounded) return;
//     //     SnapFeetToGroundIfClose();
//     // }
//     //
//     // private void SnapFeetToGroundIfClose()
//     // {
//     //     Bounds b = controller.bounds;
//     //     Vector3 feetRayOrigin = new Vector3(b.center.x, b.min.y + controller.skinWidth + 0.01f, b.center.z);
//     //     float rayLen = GROUND_SNAP_MAX_DISTANCE + 0.01f;
//     //
//     //     if (Physics.Raycast(feetRayOrigin, Vector3.down, out RaycastHit hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
//     //     {
//     //         float currentFeetY = b.min.y;
//     //         float desiredFeetY = hit.point.y + controller.skinWidth + 0.02f;
//     //         float upDelta = Mathf.Max(0f, desiredFeetY - currentFeetY);
//     //         if (upDelta > 0f)
//     //             controller.Move(Vector3.up * upDelta);
//     //     }
//     // }
//     //
//     // private void BeginIgnoreLadderCollisions(float seconds)
//     // {
//     //     if (_playerLayer < 0 || _ladderLayer < 0) return;
//     //     Physics.IgnoreLayerCollision(_playerLayer, _ladderLayer, true);
//     //     _ignoreLadderTimer = seconds;
//     //     _ladderCollisionIgnored = true;
//     // }
//     //
//     // private void EndIgnoreLadderCollisions()
//     // {
//     //     if (_playerLayer < 0 || _ladderLayer < 0) return;
//     //     Physics.IgnoreLayerCollision(_playerLayer, _ladderLayer, false);
//     //     _ladderCollisionIgnored = false;
//     // }
//     //
//     // private static int FirstLayerFromMask(LayerMask m)
//     // {
//     //     int v = m.value;
//     //     for (int i = 0; i < 32; i++)
//     //         if (((v >> i) & 1) != 0) return i;
//     //     return -1;
//     // }
// }