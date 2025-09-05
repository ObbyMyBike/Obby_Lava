using System.Collections.Generic;
using UnityEngine;

public class SimpleGameObjectPool
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Stack<GameObject> stack = new Stack<GameObject>();

    public SimpleGameObjectPool(GameObject prefab, Transform parent, int preload)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < preload; i++)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            instance.SetActive(false);
            stack.Push(instance);
        }
    }

    public GameObject Get()
    {
        GameObject instance = stack.Count > 0 ? stack.Pop() : Object.Instantiate(prefab, parent);
        instance.SetActive(true);
        
        return instance;
    }

    public void Release(GameObject instance)
    {
        if (instance == null)
            return;
        
        instance.SetActive(false);
        instance.transform.SetParent(parent, false);
        stack.Push(instance);
    }
}