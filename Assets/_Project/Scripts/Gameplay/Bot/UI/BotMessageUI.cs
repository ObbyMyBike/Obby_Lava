using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;

public class BotMessageUI : MonoBehaviour
{
    [SerializeField] private float _showTime = 3f;
    [SerializeField] private float _minWaitTimeBeforeMessage = 10f;
    [SerializeField] private float _maxWaitTimeBeforeMessage = 120f;
    [SerializeField] private GameObject _view;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private GameObject _nameView;

    private IEnumerator Start()
    {
        _view.SetActive(false);
        while (true)
        {
            yield return StartCoroutine(WaitForMessage());
            yield return StartCoroutine(ShowMessageCoroutine(RobloxChatProvider.GetNextPhrase()));
        }
    }

    private IEnumerator WaitForMessage()
    {
        float randTime = Random.Range(_minWaitTimeBeforeMessage, _maxWaitTimeBeforeMessage);
        yield return new WaitForSeconds(randTime);
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        _messageText.text = message;
        _view.SetActive(true);
        _nameView.SetActive(false);
        yield return new WaitForSeconds(_showTime);
        _view.SetActive(false);
        _nameView.SetActive(true);
    }
}