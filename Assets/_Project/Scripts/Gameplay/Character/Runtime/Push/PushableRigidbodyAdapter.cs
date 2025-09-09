using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableRigidbodyAdapter : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void TryPush(Vector3 force)
    {
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }
}