public class JumpDecision
{
    private readonly MotionSolver motion;
    private readonly BotDataConfig config;
    private readonly GroundContactCheck ground;
    
    private float _cooldown;

    public JumpDecision(MotionSolver motion, BotDataConfig config, GroundContactCheck ground)
    {
        this.motion = motion;
        this.config = config;
        this.ground = ground;
        _cooldown = 0f;
    }

    public void TickCooldown(float deltaTime)
    {
        if (_cooldown > 0f)
        {
            _cooldown -= deltaTime;
            
            if (_cooldown < 0f)
                _cooldown = 0f;
        }
    }

    public bool TryJumpNow(AnimationPlayback animation)
    {
        bool characterControllerGrounded = motion.IsGrounded;
        bool maskGrounded = ground.Probe(out _);

        if (_cooldown > 0f || (!characterControllerGrounded && !maskGrounded))
            return false;

        motion.TryJump();
        animation?.PlayJump();
        
        _cooldown = config.JumpMinCooldown;
        
        return true;
    }
    
    public bool ForceJumpIfGrounded(AnimationPlayback animation)
    {
        if (!motion.IsGrounded)
            return false;

        motion.TryJump();
        animation?.PlayJump();
        
        _cooldown = config.JumpMinCooldown;
        
        return true;
    }
}