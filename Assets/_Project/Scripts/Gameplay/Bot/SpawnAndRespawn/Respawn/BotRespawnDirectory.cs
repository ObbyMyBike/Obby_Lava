using System.Collections.Generic;

public class BotRespawnDirectory
{
    private readonly Dictionary<BotAgent, BotRespawn> map = new Dictionary<BotAgent, BotRespawn>();

    public bool TryGet(BotAgent bot, out BotRespawn respawn)
    {
        if (bot == null)
        {
            respawn = null;
            
            return false;
        }
        
        return map.TryGetValue(bot, out respawn);
    }

    public void Register(BotAgent bot, BotRespawn respawn)
    {
        if (bot == null || respawn == null)
            return;

        map[bot] = respawn;
    }

    public void Unregister(BotAgent bot)
    {
        if (bot == null)
            return;

        map.Remove(bot); 
    }
}