using System;
using System.Collections;
using UnityEngine;

public class PlayerDestructor : MonoBehaviour
{
    public event Action OnDestructed;

    [SerializeField] private PlayerDestructorConfig _config;

    public float DestructionTime => _config.DemonstrateTime;

    public void Destruct(SkinIdType skinType)
    {
        var prefab = _config.GetPrefabBySkin(skinType);
        var view = Instantiate(prefab, transform.position, transform.rotation);
        view.ActivateDestruction(_config.DemonstrateTime);
        StartCoroutine(WaitForStopDestruction());
    }

    private IEnumerator WaitForStopDestruction()
    {
        yield return new WaitForSeconds(_config.DemonstrateTime);
        OnDestructed?.Invoke();
    }
}