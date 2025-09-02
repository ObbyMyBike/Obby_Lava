// using UnityEngine;
//
// public class LadderState
// {
//     public Transform LadderFacing { get; set; }
//     public bool IsClimbing { get; set; }
//     public float TimeSinceEnter { get; set; }
//     public float ExitIntentTimer { get; set; }
//     public float ReenterBlockTimer { get; set; }
//     public float AheadMissTimer { get; set; }
//     public float LandingSlopeBoostTimer { get; set; }
//     public float PostExitWatchdogTimer { get; set; }
//
//     public float CachedStepOffset { get; set; }
//     public float CachedSlopeLimit { get; set; }
//
//     public int PlayerLayer { get; set; } = -1;
//     public int LadderLayer { get; set; } = -1;
//     public bool LadderCollisionIgnored { get; set; }
//     public float IgnoreLadderTimer { get; set; }
//
//     public void ResetOnEnter()
//     {
//         IsClimbing = true;
//         TimeSinceEnter = 0f;
//         ExitIntentTimer = 0f;
//         AheadMissTimer = 0f;
//     }
//
//     public void ResetOnExit()
//     {
//         IsClimbing = false;
//         LadderFacing = null;
//         ExitIntentTimer = 0f;
//         TimeSinceEnter = 0f;
//         AheadMissTimer = 0f;
//     }
// }