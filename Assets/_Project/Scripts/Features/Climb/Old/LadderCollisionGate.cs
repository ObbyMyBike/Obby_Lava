// using UnityEngine;
//
// public class LadderCollisionGate
// {
//     private readonly CharacterController controller;
//     private readonly Transform player;
//     private readonly LayerMask ladderMask;
//     private readonly LadderConfig config;
//
//     public LadderCollisionGate(CharacterController controller, Transform player, LayerMask ladderMask, LadderConfig config)
//     {
//         this.controller = controller;
//         this.player = player;
//         this.ladderMask = ladderMask;
//         this.config = config;
//     }
//
//     public void SetupLayers(LadderState state)
//     {
//         state.PlayerLayer = player.gameObject.layer;
//         state.LadderLayer = FirstLayerFromMask(ladderMask);
//     }
//
//     public void BeginIgnore(LadderState state, float seconds)
//     {
//         if (state.PlayerLayer < 0 || state.LadderLayer < 0) return;
//
//         if (seconds <= 0f)
//         {
//             EnsureNotIgnoring(state);
//             return;
//         }
//
//         Physics.IgnoreLayerCollision(state.PlayerLayer, state.LadderLayer, true);
//         state.IgnoreLadderTimer = seconds;
//         state.LadderCollisionIgnored = true;
//     }
//
//     public void EnsureNotIgnoring(LadderState state)
//     {
//         if (state.PlayerLayer < 0 || state.LadderLayer < 0) return;
//
//         if (state.LadderCollisionIgnored)
//         {
//             Physics.IgnoreLayerCollision(state.PlayerLayer, state.LadderLayer, false);
//             state.LadderCollisionIgnored = false;
//             state.IgnoreLadderTimer = 0f;
//         }
//     }
//
//     public void TickIgnore(LadderState state, float delta)
//     {
//         if (!state.LadderCollisionIgnored) return;
//         
//         state.IgnoreLadderTimer = Mathf.Max(0f, state.IgnoreLadderTimer - delta);
//
//         if (state.IgnoreLadderTimer <= 0f)
//             EndIgnore(state);
//     }
//
//     private void EndIgnore(LadderState state)
//     {
//         if (state.PlayerLayer < 0 || state.LadderLayer < 0) return;
//
//         Physics.IgnoreLayerCollision(state.PlayerLayer, state.LadderLayer, false);
//         state.LadderCollisionIgnored = false;
//     }
//
//     private int FirstLayerFromMask(LayerMask mask)
//     {
//         int value = mask.value;
//         for (int i = 0; i < 32; i++)
//             if (((value >> i) & 1) != 0) return i;
//         return -1;
//     }
// }