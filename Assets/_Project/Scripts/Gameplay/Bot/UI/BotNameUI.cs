using TMPro;
using UnityEngine;
using Zenject;

public class BotNameUI : MonoBehaviour
{
    [SerializeField] private Canvas _botNameCanvas;
    [SerializeField] private TMP_Text _nameText;
    private MainCameraProvider _cameraProvider;

    public void SetMainCameraProvider(MainCameraProvider mainCameraProvider)
    {
        _cameraProvider = mainCameraProvider;
    }

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    private void Update()
    {
        OrientNameToMainCamera();
    }

    private void OrientNameToMainCamera()
    {
        if (_cameraProvider == null)
            return;
        Transform camTrans = _cameraProvider.CameraTransform;
        Vector3 dir = _botNameCanvas.transform.position - camTrans.position;
        if (dir.magnitude == 0)
            return;
        _botNameCanvas.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}