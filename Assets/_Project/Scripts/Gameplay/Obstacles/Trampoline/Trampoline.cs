using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trampoline : MonoBehaviour
{
    public float bounceForce = 30f;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Movement.Bounce(bounceForce);
        }
    }
}
