using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PendulumKinematic : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public float maxAngle = 45f;
    public float speed = 2f;
    public float startPhase = 0f;

    private Quaternion startRot;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // сами управляем движением
        startRot = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin((Time.time + startPhase) * speed) * maxAngle;
        rb.MoveRotation(startRot * Quaternion.Euler(angle, 0f, 0f));
    }
}
