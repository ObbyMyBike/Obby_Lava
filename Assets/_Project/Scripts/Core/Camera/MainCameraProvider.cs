using UnityEngine;

public class MainCameraProvider : ICameraProvider
{
    private readonly Transform cameraTransform;

    public MainCameraProvider(Transform cameraTransform)
    {
        this.cameraTransform = cameraTransform;
    }

    Transform ICameraProvider.CameraTransform => cameraTransform;
}