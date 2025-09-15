using System.Collections;
using UnityEngine;

public class PlayerDestructorView : MonoBehaviour
{
    [SerializeField] private Collider[] _colliderParts;
    [SerializeField] private Rigidbody[] _rigidbodyParts;

    private void Awake()
    {
        foreach (var rigidbody in _rigidbodyParts)
        {
            rigidbody.isKinematic = true;
        }
    }

    public void ActivateDestruction(float demonstrateTime)
    {
        foreach (var rigidbody in _rigidbodyParts)
        {
            rigidbody.isKinematic = false;
        }
        StartCoroutine(WaitAndDestroy(demonstrateTime));
    }

    private IEnumerator WaitAndDestroy(float demonstrateTime)
    {
        yield return new WaitForSeconds(demonstrateTime);
        Destroy(gameObject);
    }
}