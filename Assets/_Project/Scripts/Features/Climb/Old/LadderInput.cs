// using UnityEngine;
//
// public class LadderInput
// {
//     public bool TryRead(Transform ladderFacing, Vector3 moveDirectionWorld, float minInputMag, out Vector3 local, out Result result)
//     {
//         Vector3 planar = Vector3.ProjectOnPlane(moveDirectionWorld, Vector3.up);
//
//         if (planar.sqrMagnitude < minInputMag * minInputMag)
//         {
//             local = Vector3.zero;
//             result = default;
//             
//             return false;
//         }
//
//         local = ladderFacing.InverseTransformDirection(planar).normalized;
//
//         float into = Mathf.Clamp01(-local.z);
//         float away = Mathf.Clamp01(+local.z);
//         float sideAbs = Mathf.Abs(Mathf.Clamp(local.x, -1f, 1f));
//         float climbSigned = Mathf.Clamp(into - away, -1f, 1f);
//
//         result = new Result
//         {
//             ClimbSigned = climbSigned,
//             SideAbs = sideAbs,
//             MovingUp = (climbSigned > +0.10f),
//             MovingDown = (climbSigned < -0.10f),
//         };
//         
//         return true;
//     }
//
//     public bool SideExitIntent(ref LadderState state, LadderConfig config, Vector3 local, float into)
//     {
//         bool grace = state.TimeSinceEnter < Mathf.Max(0.01f, config.GraceAfterEnterSeconds);
//         float hard = Mathf.Max(0.01f, config.ExitSideHardThreshold);
//         float soft = Mathf.Max(0.01f, config.ExitSideDotThreshold);
//         float thr = (into > config.ClimbStickPosThreshold) ? hard : soft;
//
//         bool wantExit = !grace && (Mathf.Abs(local.x) >= thr);
//
//         if (wantExit)
//         {
//             state.ExitIntentTimer += Time.deltaTime;
//             
//             if (state.ExitIntentTimer >= config.ExitDebounceSeconds)
//                 return true;
//         }
//         else
//         {
//             state.ExitIntentTimer = 0f;
//         }
//         
//         return false;
//     }
// }