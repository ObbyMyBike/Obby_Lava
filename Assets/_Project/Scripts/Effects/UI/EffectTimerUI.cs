using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectTimerUI : MonoBehaviour
{
    private const string TIME_FORMAT = "{0:00} : {1:00}";
    
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _timerText;

    public void Setup(Sprite icon, float duration)
    {
        _icon.sprite = icon;
        
        StartCoroutine(UpdateTimer(duration));
    }

    private IEnumerator UpdateTimer(float remaining)
    {
        while (remaining > 0f)
        {
            _timerText.text = Format(remaining);
            remaining -= Time.deltaTime;
            
            yield return null;
        }
        
        Destroy(gameObject);
    }

    private string Format(float second)
    {
        int minutes = Mathf.FloorToInt(second / 60f);
        int seconds = Mathf.FloorToInt(second % 60f);

        return string.Format(TIME_FORMAT, minutes, seconds);
    }
}