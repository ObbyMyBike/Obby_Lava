// public struct LadderConfig
// {
//     public readonly float TopForwardProbeDist;
//     public readonly float TopUpOffset;
//     public readonly float TopDowncastDist;
//     public readonly float TopMinUpDot;
//     public readonly float TopExitForwardNudge;
//     public readonly float TopExitExtraUpNudge;
//     public readonly float TopExitUpExtraClearance;
//     public readonly float TopReenterBlockSeconds;
//     public readonly float TopExitIgnoreSeconds;
//     public readonly float ForwardClearEps;
//     public readonly float WarpSkinPad;
//     
//     public readonly float ClimbStickPosThreshold;
//     public readonly float MinInputMagnitude;
//     public readonly float ExitSideDotThreshold;
//     public readonly float ExitSideHardThreshold;
//     public readonly float ExitDebounceSeconds;
//     public readonly float GraceAfterEnterSeconds;
//     
//     public readonly float BottomGroundThreshold;
//     public readonly float GroundRayExtra;
//     public readonly float GroundSnapMaxDistance;
//     public readonly float LandingSlopeLimit;
//     public readonly float LandingSlopeBoostSeconds;
//     
//     public readonly float AheadCheckUpOffset;
//     public readonly float AheadCheckRadiusScale;
//     public readonly float AheadHitDistanceOk;
//     public readonly float AheadMissDebounce;
//     public readonly float TopExitForwardNudgeFallback;
//     
//     public readonly float DesiredZOffset;
//     public readonly float SnapEpsilon;
//     public readonly float MaxSnapPerTick;
//     
//     public readonly bool EnablePostExitWatchdog;
//     public readonly float PostExitWatchdogSeconds;
//     public readonly float WatchdogExtraDowncast;
//     public readonly float WatchdogUpNudge;
//     public readonly float WatchdogForwardNudge;
//
//     private LadderConfig(float topForwardProbeDist = 0.90f, float topUpOffset = 1.05f, float topDowncastDist = 2.10f, float topMinUpDot = 0.85f,
//         float topExitForwardNudge = 0.30f, float topExitExtraUpNudge = 0.12f, float topExitUpExtraClearance = 0.10f, float topReenterBlockSeconds = 0.90f,
//         float topExitIgnoreSeconds = 0.55f, float forwardClearEps = 0.08f, float warpSkinPad = 0.02f, float climbStickPosThreshold = 0.25f,
//         float minInputMagnitude = 0.05f, float exitSideDotThreshold = 0.70f, float exitSideHardThreshold = 0.95f, float exitDebounceSeconds = 0.12f,
//         float graceAfterEnterSeconds = 0.25f, float bottomGroundThreshold = 0.14f, float groundRayExtra = 0.30f, float groundSnapMaxDistance = 0.45f,
//         float landingSlopeLimit = 80f, float landingSlopeBoostSeconds = 0.35f, float aheadCheckUpOffset = 1.0f, float aheadCheckRadiusScale = 0.45f,
//         float aheadHitDistanceOk = 0.04f, float aheadMissDebounce = 0.10f, float topExitForwardNudgeFallback = 0.34f, float desiredZOffset = 0.36f,
//         float snapEpsilon = 0.0025f, float maxSnapPerTick = 0.06f, bool enablePostExitWatchdog = true, float postExitWatchdogSeconds = 0.35f,
//         float watchdogExtraDowncast = 0.75f, float watchdogUpNudge = 0.10f, float watchdogForwardNudge = 0.10f)
//     {
//         TopForwardProbeDist = topForwardProbeDist;
//         TopUpOffset = topUpOffset;
//         TopDowncastDist = topDowncastDist;
//         TopMinUpDot = topMinUpDot;
//         TopExitForwardNudge = topExitForwardNudge;
//         TopExitExtraUpNudge = topExitExtraUpNudge;
//         TopExitUpExtraClearance = topExitUpExtraClearance;
//         TopReenterBlockSeconds = topReenterBlockSeconds;
//         TopExitIgnoreSeconds = topExitIgnoreSeconds;
//         ForwardClearEps = forwardClearEps;
//         WarpSkinPad = warpSkinPad;
//
//         ClimbStickPosThreshold = climbStickPosThreshold;
//         MinInputMagnitude = minInputMagnitude;
//         ExitSideDotThreshold = exitSideDotThreshold;
//         ExitSideHardThreshold = exitSideHardThreshold;
//         ExitDebounceSeconds = exitDebounceSeconds;
//         GraceAfterEnterSeconds = graceAfterEnterSeconds;
//
//         BottomGroundThreshold = bottomGroundThreshold;
//         GroundRayExtra = groundRayExtra;
//         GroundSnapMaxDistance = groundSnapMaxDistance;
//         LandingSlopeLimit = landingSlopeLimit;
//         LandingSlopeBoostSeconds = landingSlopeBoostSeconds;
//
//         AheadCheckUpOffset = aheadCheckUpOffset;
//         AheadCheckRadiusScale = aheadCheckRadiusScale;
//         AheadHitDistanceOk = aheadHitDistanceOk;
//         AheadMissDebounce = aheadMissDebounce;
//         TopExitForwardNudgeFallback = topExitForwardNudgeFallback;
//
//         DesiredZOffset = desiredZOffset;
//         SnapEpsilon = snapEpsilon;
//         MaxSnapPerTick = maxSnapPerTick;
//
//         EnablePostExitWatchdog = enablePostExitWatchdog;
//         PostExitWatchdogSeconds = postExitWatchdogSeconds;
//         WatchdogExtraDowncast = watchdogExtraDowncast;
//         WatchdogUpNudge = watchdogUpNudge;
//         WatchdogForwardNudge = watchdogForwardNudge;
//     }
//     
//     public LadderConfig CreateDefault() => new LadderConfig(
//             topForwardProbeDist: 0.90f, topUpOffset: 1.05f, topDowncastDist: 2.10f, topMinUpDot: 0.85f,
//             topExitForwardNudge: 0.30f, topExitExtraUpNudge: 0.12f, topExitUpExtraClearance: 0.10f, topReenterBlockSeconds: 0.90f,
//             topExitIgnoreSeconds: 0.55f, forwardClearEps: 0.08f, warpSkinPad: 0.02f, climbStickPosThreshold: 0.25f,
//             minInputMagnitude: 0.05f, exitSideDotThreshold: 0.70f, exitSideHardThreshold: 0.95f, exitDebounceSeconds: 0.12f,
//             graceAfterEnterSeconds: 0.25f, bottomGroundThreshold: 0.14f, groundRayExtra: 0.30f, groundSnapMaxDistance: 0.45f,
//             landingSlopeLimit: 80f, landingSlopeBoostSeconds: 0.35f, aheadCheckUpOffset: 1.0f, aheadCheckRadiusScale: 0.45f,
//             aheadHitDistanceOk: 0.04f, aheadMissDebounce: 0.10f, topExitForwardNudgeFallback: 0.34f, desiredZOffset: 0.36f,
//             snapEpsilon: 0.0025f, maxSnapPerTick: 0.06f, enablePostExitWatchdog: true, postExitWatchdogSeconds: 0.35f,
//             watchdogExtraDowncast: 0.75f, watchdogUpNudge: 0.10f, watchdogForwardNudge: 0.10f
//         );
//     
//     public bool LooksUninitialized() => ExitSideDotThreshold == 0f && ExitSideHardThreshold == 0f && GraceAfterEnterSeconds == 0f && DesiredZOffset == 0f;
// }