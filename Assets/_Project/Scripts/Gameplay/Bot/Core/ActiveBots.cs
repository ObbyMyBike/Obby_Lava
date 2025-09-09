using System.Collections.Generic;

public class ActiveBots
{
    private readonly HashSet<BotAgent> active = new HashSet<BotAgent>();

    public IReadOnlyCollection<BotAgent> Active => active;

    public void Register(BotAgent bot)
    {
        if (bot != null)
            active.Add(bot);
    }

    public void Unregister(BotAgent bot)
    {
        if (bot != null && active.Contains(bot))
            active.Remove(bot);
    }
}