using TMPro;
using UnityEngine;

public class FinishUI : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private TMP_Text _timeText;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        _view.SetActive(true);
    }

    public void Hide()
    {
        _view.SetActive(false);
    }

    public void SetTimeText(string text)
    {
        _timeText.text = text;
    }
}
