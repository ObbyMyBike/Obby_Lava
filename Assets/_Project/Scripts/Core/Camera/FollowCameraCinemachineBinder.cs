using UnityEngine;
using Cinemachine;
using Zenject;

public class FollowCameraCinemachineBinder : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    [Inject]
    public void Construct(CinemachineVirtualCamera virtualCamera, [InjectOptional] PlayerPrefabSpawn spawner, SpawnedPlayerAccessor accessor)
    {
        _virtualCamera = virtualCamera;
        
        if (accessor.Player != null)
        {
            Setup(accessor.Player.transform);
        }
        else if (spawner != null)
        {
            spawner.OnPlayerSpawned += OnPlayerSpawned;
        }
    }

    public Vector3 ForwardDirection => transform.forward;
    public Vector3 RightDirection => transform.right;
    
    private void OnPlayerSpawned(Transform t)
    {
        Setup(t);
    }

    private void Setup(Transform player)
    {
        _virtualCamera.Follow = player;
        _virtualCamera.LookAt = player;

        CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        
        if (transposer != null)
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
    }
}