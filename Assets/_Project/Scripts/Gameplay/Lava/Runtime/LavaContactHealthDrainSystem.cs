using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LavaContactHealthDrainSystem : ITickable
{
    private const float EPSILON = 0.0001f;
    private const int MAX_ACTIVE_BOTS = 64; 
    
    private readonly ILavaSurface lavaSurface;
    
    private readonly SpawnedPlayerAccessor playerAccessor;
    private readonly PlayerHealth playerHealth;
    
    private readonly ActiveBots activeBots;
    private readonly List<BotAgent> botsSnapshot = new List<BotAgent>(MAX_ACTIVE_BOTS);

    private readonly float secondsToDieInLava;
    private readonly float regenPerSecondOutsideLava;

    public LavaContactHealthDrainSystem(ILavaSurface lavaSurface, [InjectOptional] SpawnedPlayerAccessor playerAccessor,
        [InjectOptional] PlayerHealth playerHealth, [InjectOptional] ActiveBots activeBots, float secondsToDieInLava,
        float regenPerSecondOutsideLava)
    {
        this.lavaSurface = lavaSurface;
        this.playerAccessor = playerAccessor;
        this.playerHealth = playerHealth;
        this.activeBots = activeBots;

        this.secondsToDieInLava = Mathf.Max(EPSILON, secondsToDieInLava);
        this.regenPerSecondOutsideLava = Mathf.Max(0f, regenPerSecondOutsideLava);
    }

    void ITickable.Tick()
    {
        if (!(lavaSurface is LavaSurfaceView view))
            return;

        Transform viewTransform = view.transform;
        float topY = viewTransform.position.y + viewTransform.lossyScale.y * 0.5f;
        float deltaTime = Time.deltaTime;
        
        if (playerAccessor != null && playerHealth != null && !playerHealth.IsDead)
        {
            CharacterController controller = playerAccessor.CharacterController;
            
            if (controller != null)
            {
                float deathPerSeconds = playerHealth.MaxHealth / secondsToDieInLava;
                bool submerged = controller.bounds.min.y <= topY;

                if (submerged)
                    playerHealth.TryApplyDamage(deathPerSeconds * deltaTime);
                else if (regenPerSecondOutsideLava > 0f)
                    playerHealth.Heal(regenPerSecondOutsideLava * deltaTime);
            }
        }
        
        if (activeBots?.Active != null)
        {
            botsSnapshot.Clear();
            
            foreach (BotAgent bot in activeBots.Active)
                if (bot != null)
                    botsSnapshot.Add(bot);

            for (int i = 0; i < botsSnapshot.Count; i++)
            {
                BotAgent bot = botsSnapshot[i];
                
                if (bot == null)
                    continue;

                CharacterController controller = bot.Controller;
                BotHealth health = bot.Health;

                if (controller == null || health == null || health.IsDead)
                    continue;

                float feetY = controller.bounds.min.y;
                float deathPerSeconds = health.MaxHealth / secondsToDieInLava;

                if (feetY <= topY)
                {
                    health.TryApplyDamage(deathPerSeconds * deltaTime);
                    
                    if (health.IsDead)
                        bot.NotifyDiedByEnvironment();
                }
                else if (regenPerSecondOutsideLava > 0f)
                {
                    health.Heal(regenPerSecondOutsideLava * deltaTime);
                }
            }
        }
    }
}