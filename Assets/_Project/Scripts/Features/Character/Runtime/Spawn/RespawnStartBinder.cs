using UnityEngine;
using Zenject;

public class RespawnStartBinder : IInitializable
{
    private readonly PlayerRespawn respawn;
    private readonly Transform startSpawn;

    [Inject]
    public RespawnStartBinder(PlayerRespawn respawn, Transform startSpawn)
    {
        this.respawn = respawn;
        this.startSpawn = startSpawn;
    }

    void IInitializable.Initialize()
    {
        if (startSpawn != null)
            respawn.SetStartPoint(startSpawn);
    }
}