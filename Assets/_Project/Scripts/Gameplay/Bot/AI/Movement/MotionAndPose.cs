using UnityEngine;

public class MotionAndPose
{
    private readonly BotAgent botAgent;
    private readonly MotionSolver motion;
    private readonly BotDataConfig config;

    public MotionAndPose(BotAgent botAgent, MotionSolver motion, BotDataConfig config)
    {
        this.botAgent = botAgent;
        this.motion = motion;
        this.config = config;
    }

    public void MoveAndAnimate(Vector3 moveDirection, float delta)
    {
        motion.UpdateMovement(moveDirection, delta);
        botAgent.Animation.UpdateAnimation(motion.CurrentVelocity, motion.IsGrounded);
    }

    public void IdlePose(float deltaTime) => MoveAndAnimate(Vector3.zero, deltaTime);

    public bool IsGrounded => motion.IsGrounded;

    public void ResetVertical() => motion.ResetVerticalVelocity();
}