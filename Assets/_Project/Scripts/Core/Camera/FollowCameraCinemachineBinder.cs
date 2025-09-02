using UnityEngine;
using Cinemachine;
using Zenject;

public class FollowCameraCinemachineBinder : MonoBehaviour, IMainCamera
{
    private CinemachineVirtualCamera _virtualCamera;

    [Inject]
    public void Construct(Player player, CinemachineVirtualCamera virtualCamera)
    {
        _virtualCamera = virtualCamera;
        _virtualCamera.Follow = player.transform;
        _virtualCamera.LookAt = player.transform;
        
        CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        
        if (transposer != null)
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
    }

    public Vector3 ForwardDirection => transform.forward;
    public Vector3 RightDirection => transform.right;
}