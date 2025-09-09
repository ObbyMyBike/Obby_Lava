using UnityEngine;
using Random = System.Random;

public class FailureDice
{
    private readonly Random random;
    private readonly BotDataConfig config;
    private readonly MotionSolver motion;

    public FailureDice(int seed, BotDataConfig config, MotionSolver motion)
    {
        random = new Random(seed);
        
        this.config = config;
        this.motion = motion;
    }

    public bool Roll(float p01) => p01 > 0f && random.NextDouble() <= Mathf.Clamp01(p01);

    public void ApplyFatalKick(BotAgent avatar)
    {
        Vector3 direction = avatar.Controller.transform.forward + UnityEngine.Random.insideUnitSphere * 0.15f;
        direction.y = 0f;
        direction.Normalize();
        
        motion.BeginAdditiveNudge(direction * config.RandomFailKickForce, 0.2f);
        motion.AddVerticalVelocity(config.FailFallDownVelocity);
    }
}