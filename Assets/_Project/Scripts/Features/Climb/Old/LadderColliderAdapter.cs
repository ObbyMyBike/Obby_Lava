// using UnityEngine;
//
// public class LadderColliderAdapter
// {
//     private const float CONTROLLER_HEIGHT = 2.60f;
//     private const float NORMAL_COLLIDER_CENTER_Y = 1.31f;
//     private const float CLIMB_COLLIDER_CENTER_Y = 2.20f;
//     private const float CLIMB_STEP_OFFSET = 0.70f;
//     
//     private readonly CharacterController controller;
//     private readonly LadderConfig config;
//
//     public LadderColliderAdapter(CharacterController controller, LadderConfig config)
//     {
//         this.controller = controller;
//         this.config = config;
//     }
//
//     public void ApplyClimbCollider(LadderState state)
//     {
//         state.CachedStepOffset = controller.stepOffset;
//         state.CachedSlopeLimit = controller.slopeLimit;
//
//         controller.stepOffset = CLIMB_STEP_OFFSET;
//
//         controller.height = CONTROLLER_HEIGHT;
//         Vector3 center = controller.center;
//         center.y = CLIMB_COLLIDER_CENTER_Y;
//         controller.center = center;
//     }
//
//     public void RestoreOriginalCollider(LadderState state)
//     {
//         controller.stepOffset = state.CachedStepOffset;
//
//         controller.height = CONTROLLER_HEIGHT;
//         Vector3 center = controller.center;
//         center.y = NORMAL_COLLIDER_CENTER_Y;
//         controller.center = center;
//     }
//
//     public void BoostLandingStability(LadderState state)
//     {
//         controller.slopeLimit = config.LandingSlopeLimit;
//         state.LandingSlopeBoostTimer = config.LandingSlopeBoostSeconds;
//     }
//
//     public void TickLandingBoost(LadderState state, float delta)
//     {
//         if (state.LandingSlopeBoostTimer > 0f)
//         {
//             state.LandingSlopeBoostTimer = Mathf.Max(0f, state.LandingSlopeBoostTimer - delta);
//             
//             if (state.LandingSlopeBoostTimer <= 0f)
//                 controller.slopeLimit = state.CachedSlopeLimit;
//         }
//     }
// }