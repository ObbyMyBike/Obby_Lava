using UnityEngine;

public class MainCameraProvider
{
    private readonly Transform cameraTransform;

    public MainCameraProvider(Transform cameraTransform)
    {
        this.cameraTransform = cameraTransform;
    }

    public Transform CameraTransform => cameraTransform;
}