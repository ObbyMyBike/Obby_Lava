using UnityEngine;
using Zenject;

public class LavaContactHealthDrain : ITickable
{
    private const float EPSILON = 0.0001f;
    
    private readonly SpawnedPlayerAccessor accessor;
    private readonly ILavaSurface lavaSurface;
    private readonly PlayerHealth health;
    private readonly float secondsToDieInLava;
    private readonly float regenPerSecondOutsideLava;
    private readonly float damagePerSecond;

    public LavaContactHealthDrain(SpawnedPlayerAccessor accessor, ILavaSurface lavaSurface, PlayerHealth health, float secondsToDieInLava, float regenPerSecondOutsideLava)
    {
        this.accessor = accessor;
        this.lavaSurface = lavaSurface;
        this.health = health;
        this.secondsToDieInLava = Mathf.Max(EPSILON, secondsToDieInLava);
        this.regenPerSecondOutsideLava = Mathf.Max(0f, regenPerSecondOutsideLava);

        damagePerSecond = health.MaxHealth / this.secondsToDieInLava;
    }

    void ITickable.Tick()
    {
        if (health.IsDead)
            return;

        CharacterController controller = accessor.CharacterController;
        
        if (controller == null)
            return;
        
        float deltaTime = Time.deltaTime;
        
        if (IsPlayerSubmerged(controller))
            health.TryApplyDamage(damagePerSecond * deltaTime);
        else if (regenPerSecondOutsideLava > 0f)
            health.Heal(regenPerSecondOutsideLava * deltaTime);
    }

    private bool IsPlayerSubmerged(CharacterController playerController)
    {
        var feetY = playerController.bounds.min.y;
        var (topY, isOk) = GetLavaTopWorldY();
        
        if (!isOk)
            return false;
        
        return feetY <= topY;
    }
    
    private (float topY, bool isOk) GetLavaTopWorldY()
    {
        LavaSurfaceView view = lavaSurface as LavaSurfaceView;
        
        if (view == null)
            return (0f, false);

        Transform viewTransform = view.transform;
        float halfWorldHeight = viewTransform.lossyScale.y * 0.5f;
        float topY = viewTransform.position.y + halfWorldHeight;
        
        return (topY, true);
    }
}