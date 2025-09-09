public class BotAiContext
{
    public readonly BotAgent Agent;
    public readonly BotRespawn Respawn;
    public readonly BotDataConfig Config;
    public readonly ActiveBots Registry;

    public readonly WaypointTrack Track;
    public readonly MotionAndPose Motion;
    public readonly ProximitySteering Steering = new ProximitySteering();
    public readonly JumpDecision Jump;
    public readonly ClimbActivator Climb;
    public readonly FailureDice FailureDice;
    public readonly BotFailureDecider Failure;

    public BotAiContext(BotAgent agent, MotionSolver motion, BotRespawn respawn, WaypointPath path,
        BotDataConfig config, ActiveBots registry, int seed, BotLadderBridge ladder)
    {
        Agent = agent;
        Respawn = respawn;
        Config = config;
        Registry = registry;

        Track = new WaypointTrack(path);
        GroundContactCheck ground = new GroundContactCheck(agent.Controller, config);
        Jump = new JumpDecision(motion, config, ground);
        Climb = new ClimbActivator(ladder);
        FailureDice = new FailureDice(seed, config, motion);
        Motion = new MotionAndPose(agent, motion, config);
        Failure = new BotFailureDecider(FailureDice);
    }
}