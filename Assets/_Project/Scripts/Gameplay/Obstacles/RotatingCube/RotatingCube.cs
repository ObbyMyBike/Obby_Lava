using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("Настройки вращения")]
    public Vector3 rotationAxis = Vector3.up; 
    public float rotationSpeed = 50f;
    public bool clockwise = true;

    void Update()
    {
        float angle = rotationSpeed * Time.deltaTime;
        if (!clockwise) angle = -angle;

        transform.Rotate(rotationAxis, angle);
    }
}
