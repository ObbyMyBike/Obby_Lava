using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectTimerUI : MonoBehaviour
{
    private const string TIME_FORMAT = "{0:00} : {1:00}";
 
    public event OnExpired OnExpired;
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _timerText;

    private ItemType _currentType;
    private Coroutine _countdownCoroutine;
    private float _remainingSeconds;
    
    private void OnDestroy()
    {
        StopCountdownIfRunning();
    }
    
    public void TrySetupIcon(Sprite icon)
    {
        SetIcon(icon);
    }

    public void SetIcon(Sprite icon)
    {
        _iconImage.sprite = icon;
    }
    
    public void SetType(ItemType type)
    {
        _currentType = type;
    }
    
    public void StartCountdown(float durationSeconds)
    {
        StopCountdownIfRunning();
        
        _remainingSeconds = Mathf.Max(0f, durationSeconds);
        _countdownCoroutine = StartCoroutine(Countdown());
    }
    
    public void RestartCountdown(float durationSeconds)
    {
        StartCountdown(durationSeconds);
    }

    private void StopCountdownIfRunning()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            
            _countdownCoroutine = null;
        }
    }
    
    private void UpdateTimeText(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        
        _timerText.text = string.Format(TIME_FORMAT, minutes, secs);
    }
    
    private IEnumerator Countdown()
    {
        UpdateTimeText(_remainingSeconds);

        while (_remainingSeconds > 0f)
        {
            _remainingSeconds -= Time.deltaTime;
            
            if (_remainingSeconds < 0f)
                _remainingSeconds = 0f;

            UpdateTimeText(_remainingSeconds);
            
            yield return null;
        }
        
        OnExpired?.Invoke(_currentType);
        
        Destroy(gameObject);
    }
}