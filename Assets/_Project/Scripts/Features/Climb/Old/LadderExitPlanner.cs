// using UnityEngine;
//
// public class LadderExitPlanner
// {
//     private readonly CharacterController controller;
//     private readonly LadderPhysics physics;
//     private readonly LadderConfig config;
//     private readonly LayerMask groundMask;
//
//     public LadderExitPlanner(CharacterController controller, LadderPhysics physics, LayerMask groundMask, LadderConfig config)
//     {
//         this.controller = controller;
//         this.physics = physics;
//         this.config = config;
//         this.groundMask = groundMask;
//     }
//
//     public bool TrySoftTopExit(Transform ladderFacing)
//     {
//
//         if (ladderFacing == null)
//             return false;
//
//         Vector3 start = controller.bounds.center + Vector3.up * config.TopUpOffset;
//         Vector3 forwardToPlatform = -ladderFacing.forward;
//         Vector3 probe = start + forwardToPlatform * config.TopForwardProbeDist;
//
//         bool GotGround(Vector3 p, out RaycastHit hitInfo)
//         {
//             if (Physics.Raycast(p, Vector3.down, out hitInfo, config.TopDowncastDist, groundMask, QueryTriggerInteraction.Ignore))
//                 return true;
//
//             return false;
//         }
//
//         if (!GotGround(probe, out RaycastHit downHit))
//         {
//             float extraForward = Mathf.Max(controller.radius * 0.5f, 0.15f);
//             float extraUp = 0.05f;
//             Vector3 probe2 = start + Vector3.up * extraUp + forwardToPlatform * (config.TopForwardProbeDist + extraForward);
//
//             if (!GotGround(probe2, out downHit))
//             {
//                 float minForwardClear = controller.radius + config.ForwardClearEps;
//                 float forwardClear = Mathf.Max(minForwardClear, config.TopExitForwardNudgeFallback);
//                 Vector3 targetCenter = controller.bounds.center + forwardToPlatform * forwardClear;
//
//                 float capsuleHalf = (controller.height * 0.5f) - controller.radius;
//                 Vector3 capBottom = targetCenter + Vector3.down * capsuleHalf;
//                 Vector3 capTop = targetCenter + Vector3.up * capsuleHalf;
//                 float capRadius = controller.radius - config.WarpSkinPad;
//
//                 if (!Physics.CheckCapsule(capBottom, capTop, capRadius, groundMask, QueryTriggerInteraction.Ignore))
//                 {
//                     float upNudge = config.TopExitExtraUpNudge;
//                     controller.Move(Vector3.up * upNudge);
//                     controller.Move(forwardToPlatform * forwardClear);
//                     
//                     return true;
//                 }
//                 
//                 return false;
//             }
//         }
//         
//         float upDot = Vector3.Dot(downHit.normal, Vector3.up);
//         
//         if (upDot < config.TopMinUpDot)
//             return false;
//
//         float skin = controller.skinWidth;
//         Bounds bounds = controller.bounds;
//         float currentFeetY = bounds.min.y;
//         float desiredFeetY = downHit.point.y + skin + 0.02f;
//         float upDelta = Mathf.Max(desiredFeetY - currentFeetY + config.TopExitUpExtraClearance, config.TopExitExtraUpNudge);
//
//         controller.Move(Vector3.up * upDelta);
//
//         float minForwardClear2 = controller.radius + config.ForwardClearEps;
//         float forwardClear2 = Mathf.Max(minForwardClear2, config.TopExitForwardNudge);
//
//         Vector3 targetCenter2 = controller.bounds.center + forwardToPlatform * forwardClear2;
//
//         float capsuleHalf2 = (controller.height * 0.5f) - controller.radius;
//         Vector3 capBottom2 = targetCenter2 + Vector3.down * capsuleHalf2;
//         Vector3 capTop2 = targetCenter2 + Vector3.up * capsuleHalf2;
//         float capRadius2 = controller.radius - config.WarpSkinPad;
//
//         if (Physics.CheckCapsule(capBottom2, capTop2, capRadius2, groundMask, QueryTriggerInteraction.Ignore))
//         {
//             controller.Move(Vector3.down * Mathf.Min(upDelta, 0.06f));
//             
//             return false;
//         }
//
//         controller.Move(forwardToPlatform * forwardClear2);
//         
//         return true;
//     }
// }