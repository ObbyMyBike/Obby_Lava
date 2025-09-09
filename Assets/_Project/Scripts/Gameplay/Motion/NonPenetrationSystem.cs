using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NonPenetrationSystem : ITickable
{
    private const int MAX_SOLVER_ITERATIONS = 2;
    private const float SKIN_OFFSET = 0.001f;
    private const float PUSH_SCALE = 1.0f;
    private const float MIN_MOVE = 0.0001f;

    private readonly ActiveBots bots;
    private readonly SpawnedPlayerAccessor playerAccessor;

    public NonPenetrationSystem(ActiveBots bots, SpawnedPlayerAccessor playerAccessor)
    {
        this.bots = bots;
        this.playerAccessor = playerAccessor;
    }

    void ITickable.Tick()
    {
        List<CharacterController> controllers = CollectControllers();
        
        if (controllers.Count <= 1)
            return;

        for (int iter = 0; iter < MAX_SOLVER_ITERATIONS; iter++)
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                CharacterController characterController = controllers[i];
                
                if (characterController == null || !characterController.enabled)
                    continue;

                Capsule aCapsule = GetCapsule(characterController);
                Vector3 totalCorrection = Vector3.zero;

                for (int j = 0; j < controllers.Count; j++)
                {
                    if (i == j)
                        continue;
                    
                    CharacterController controller = controllers[j];
                    
                    if (controller == null || !controller.enabled)
                        continue;

                    Capsule bCapsule = GetCapsule(controller);

                    if (Physics.ComputePenetration(aCapsule.Collider, aCapsule.Position, aCapsule.Rotation, bCapsule.Collider,
                            bCapsule.Position, bCapsule.Rotation, out Vector3 direction, out float distance))
                    {
                        if (distance > 0f)
                        {
                            Vector3 correction = direction * (distance + SKIN_OFFSET) * 0.5f * PUSH_SCALE;
                            totalCorrection += correction;
                        }
                    }
                }

                if (totalCorrection.sqrMagnitude > (MIN_MOVE * MIN_MOVE))
                    characterController.Move(totalCorrection);
            }
        }
    }

    private List<CharacterController> CollectControllers()
    {
        List<CharacterController> list = new List<CharacterController>(32);

        if (bots.Active != null)
        {
            foreach (var bot in bots.Active)
                if (bot != null && bot.Controller != null)
                    list.Add(bot.Controller);
        }

        CharacterController player = playerAccessor.CharacterController;
        
        if (player != null)
            list.Add(player);

        return list;
    }
    
    private Capsule GetCapsule(CharacterController characterController)
    {
        CapsuleCollider proxy = CapsuleProxy.Get();
        proxy.radius = characterController.radius;
        proxy.height = characterController.height;
        proxy.direction = 1;
        proxy.transform.position = characterController.transform.position + characterController.center;
        proxy.transform.rotation = characterController.transform.rotation;
        proxy.enabled = true;

        return new Capsule
        {
            Collider = proxy,
            Position = proxy.transform.position,
            Rotation = proxy.transform.rotation
        };
    }
}