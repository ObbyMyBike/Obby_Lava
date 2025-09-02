using UnityEngine;

public class PlayerRotator
{
    private const float MIN_SQR_MAGNITUDE = 0.01f;
    
    private readonly Transform transform;
    private readonly float rotationSpeed;
    
    public PlayerRotator(Transform transform, float rotationSpeed)
    {
        this.transform = transform;
        this.rotationSpeed = rotationSpeed;
    }
    
    public void UpdateRotation(Vector3 moveDirection, float deltaTime)
    {
        Vector3 horizontal = new Vector3(moveDirection.x, 0f, moveDirection.z);
        
        if (horizontal.sqrMagnitude < MIN_SQR_MAGNITUDE)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontal);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * deltaTime);
    }
}