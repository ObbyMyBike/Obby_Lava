// using UnityEngine;
//
// public class LadderPhysics
// {
//     private readonly CharacterController controller;
//     private readonly Transform player;
//     private readonly LayerMask ladderMask;
//     private readonly LayerMask groundMask;
//     private readonly LadderConfig config;
//
//     public LadderPhysics(CharacterController controller, Transform player, LayerMask ladderMask, LayerMask groundMask, LadderConfig config)
//     {
//         this.controller = controller;
//         this.player = player;
//         this.ladderMask = ladderMask;
//         this.groundMask = groundMask;
//         this.config = config;
//     }
//
//     public bool HasLadderAhead(Transform ladderFacing, out float hitDistance)
//     {
//         hitDistance = -1f;
//         
//         if (ladderFacing == null)
//             return false;
//
//         float radius = controller.radius * config.AheadCheckRadiusScale;
//         Vector3 origin = player.position + Vector3.up * config.AheadCheckUpOffset;
//
//         Vector3 toLadder = (ladderFacing.position - origin);
//         float distToLadder = toLadder.magnitude;
//         
//         if (distToLadder < 1e-4f)
//             return true;
//
//         Vector3 direction = toLadder / distToLadder;
//         float castDistance = Mathf.Max(distToLadder + 0.15f, config.DesiredZOffset + 0.50f);
//         bool hitSmth = Physics.SphereCast(origin, radius, direction, out RaycastHit hit, castDistance, ladderMask, QueryTriggerInteraction.Collide);
//
//         if (hitSmth)
//         {
//             bool same = hit.transform == ladderFacing || hit.transform.IsChildOf(ladderFacing) || ladderFacing.IsChildOf(hit.transform);
//             
//             if (same)
//             {
//                 hitDistance = hit.distance;
//                 bool ok = hit.distance <= (distToLadder + config.AheadHitDistanceOk);
//                 
//                 return ok;
//             }
//         }
//         
//         return false;
//     }
//
//     public float GetGroundDistance(out RaycastHit hit)
//     {
//         Bounds bounds = controller.bounds;
//         Vector3 feet = new Vector3(bounds.center.x, bounds.min.y + controller.skinWidth + 0.01f, bounds.center.z);
//         float rayLen = config.GroundRayExtra + 0.01f;
//         
//         if (Physics.Raycast(feet, Vector3.down, out hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
//             return hit.distance;
//         
//         return -1f;
//     }
//
//     public void SnapFeetToGroundIfCloseSafe()
//     {
//         if (controller.isGrounded)
//             return;
//         
//         SnapFeetToGroundIfClose();
//     }
//
//     public void TickPostExitWatchdog(LadderState state)
//     {
//         if (!config.EnablePostExitWatchdog || state.PostExitWatchdogTimer <= 0f)
//             return;
//
//         state.PostExitWatchdogTimer = Mathf.Max(0f, state.PostExitWatchdogTimer - Time.deltaTime);
//         
//         if (controller.isGrounded)
//             return;
//
//         Bounds bounds = controller.bounds;
//         Vector3 feet = new Vector3(bounds.center.x, bounds.min.y + controller.skinWidth + 0.01f, bounds.center.z);
//
//         if (Physics.Raycast(feet, Vector3.down, out RaycastHit hit, config.WatchdogExtraDowncast, groundMask, QueryTriggerInteraction.Ignore))
//         {
//             float desiredFeetY = hit.point.y + controller.skinWidth + 0.02f;
//             float currentFeetY = bounds.min.y;
//             float upDelta = Mathf.Max(0f, desiredFeetY - currentFeetY);
//
//             if (upDelta > 0f)
//             {
//                 Vector3 forward = (state.LadderFacing == null ? player.forward : -state.LadderFacing.forward);
//                 Vector3 nudge = (Vector3.up * Mathf.Min(upDelta, config.WatchdogUpNudge)) + (forward * config.WatchdogForwardNudge);
//                 controller.Move(nudge);
//             }
//         }
//     }
//
//     private void SnapFeetToGroundIfClose()
//     {
//         Bounds bounds = controller.bounds;
//         Vector3 feetRayOrigin = new Vector3(bounds.center.x, bounds.min.y + controller.skinWidth + 0.01f, bounds.center.z);
//         float rayLen = config.GroundSnapMaxDistance + 0.01f;
//
//         if (Physics.Raycast(feetRayOrigin, Vector3.down, out RaycastHit hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
//         {
//             float currentFeetY = bounds.min.y;
//             float desiredFeetY = hit.point.y + controller.skinWidth + 0.02f;
//             float upDelta = Mathf.Max(0f, desiredFeetY - currentFeetY);
//             
//             if (upDelta > 0f)
//             {
//                 controller.Move(Vector3.up * upDelta);
//             }
//         }
//     }
//     
//     public void ArmPostExitWatchdog(LadderState state)
//     {
//         if (config.EnablePostExitWatchdog && !controller.isGrounded)
//             state.PostExitWatchdogTimer = config.PostExitWatchdogSeconds;
//     }
// }