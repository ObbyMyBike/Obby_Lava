using System.Collections;
using UniRx.Triggers;
using UnityEngine;

public class PushableRigidbodyAdapter : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CharacterController _characterController;
    //[SerializeField] private BotAgent _botAgent;

    private void Awake()
    {
        _rigidbody.isKinematic = true;
        _characterController.enabled = true;
    }

    public void TryPush(Vector3 force)
    {
        _characterController.enabled = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = false;
      //  _botAgent.SetPushed(true);
        _rigidbody.AddForce(force, ForceMode.Impulse);
        StartCoroutine(WaitForRigidbodyTurnOff());
    }

    private IEnumerator WaitForRigidbodyTurnOff()
    {
        yield return new WaitForSeconds(2f);
        _characterController.enabled = true;
        _rigidbody.isKinematic = true;
        //_botAgent.SetPushed(false);
    }
}