using UnityEngine;

public class BotPool
{
    private readonly SimpleGameObjectPool pool;

    public BotPool(BotAgent prefab, Transform parent, int preload)
    {
        pool = new SimpleGameObjectPool(prefab.gameObject, parent, preload);
    }

    public GameObject Get() => pool.Get();
    
    public GameObject GetInactive() => pool.GetInactive();
    
    public void Release(GameObject instance) => pool.Release(instance);
}