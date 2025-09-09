using UnityEngine;

public static class CapsuleProxy
{
    private static CapsuleCollider _proxy;

    public static CapsuleCollider Get()
    {
        if (_proxy == null)
        {
            GameObject target = new GameObject("__CapsulePenetrationProxy");
            Object.DontDestroyOnLoad(target);
                
            _proxy = target.AddComponent<CapsuleCollider>();
            _proxy.isTrigger = true;
            target.hideFlags = HideFlags.HideAndDontSave;
        }
        
        return _proxy;
    }
}