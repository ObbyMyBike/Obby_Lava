// using UnityEngine;
//
// [RequireComponent(typeof(Collider))]
// public class LadderVolume : MonoBehaviour
// {
//     private const float ENTER_INTENT_THR = 0.35f;
//     private const float MAX_PLANE_OFFSET = 0.45f;
//     
//     [SerializeField] private Transform _facing;
//
//     private Collider _collider;
//     
//     private Transform Facing => _facing != null ? _facing : transform;
//     
//     private void OnTriggerEnter(Collider other)
//     {
//         if (!other.TryGetComponent(out Player player))
//             return;
//
//         bool decision = (!player.IsClimbing && player.CanEnterLadderNow && ShouldAutoEnter(player));
//
//         if (decision)
//             player.BeginClimb(Facing);
//     }
//     
//     private void OnTriggerStay(Collider other)
//     {
//         if (!other.TryGetComponent(out Player player))
//             return;
//
//         bool decision = (!player.IsClimbing && player.CanEnterLadderNow && ShouldAutoEnter(player));
//
//         if (decision)
//             player.BeginClimb(Facing);
//     }
//
//     private void OnTriggerExit(Collider other)
//     {
//         if (other.TryGetComponent(out Player player))
//             player.EndClimb(Facing);
//     }
//     
//     private void Reset()
//     {
//         _collider = GetComponent<Collider>();
//         _collider.isTrigger = true;
//     }
//     
//     private bool ShouldAutoEnter(Player player)
//     {
//         Vector3 planarInput = Vector3.ProjectOnPlane(player.CurrentMoveDirectionWorld, Vector3.up);
//         
//         if (planarInput.sqrMagnitude < 0.0001f)
//             return false;
//
//         Vector3 localInput = Facing.InverseTransformDirection(planarInput).normalized;
//         float intoLadder = Mathf.Clamp01(-localInput.z);
//         
//         if (intoLadder < ENTER_INTENT_THR)
//             return false;
//         
//         CharacterController controller;
//         
//         if (player.TryGetComponent(out controller))
//         {
//             Vector3 ladderPlaneOrigin = Facing.position;
//             Vector3 ladderPlaneNormal = Facing.forward;
//             float planeOffset = Mathf.Abs(Vector3.Dot((player.transform.position - ladderPlaneOrigin), ladderPlaneNormal));
//             
//             if (planeOffset > MAX_PLANE_OFFSET + controller.radius)
//                 return false;
//         }
//
//         return true;
//     }
// }