public class BotDeathToRespawnLink
{
    private readonly DeathToRespawnQueue deathQueue;

    public BotDeathToRespawnLink(DeathToRespawnQueue deathQueue) => this.deathQueue = deathQueue;

    public void Attach(BotAgent bot)
    {
        if (bot == null)
            return;
        
        bot.OnDied += _ => deathQueue.OnBotDied(bot);
    }
}