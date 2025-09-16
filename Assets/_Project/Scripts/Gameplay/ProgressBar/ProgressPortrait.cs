using UnityEngine;
using UnityEngine.UI;

public class ProgressPortrait : MonoBehaviour
{
    [SerializeField] private Image _portrait;

    private RectTransform _rectTransform;

    private float _containerHeight;
    private float _currentProgress;
    private bool _isPlayer;

    public RectTransform RectTransform => _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

    }

    public void Initialize(Sprite portrait, bool isPlayer)
    {
        _portrait.sprite = portrait;
        _isPlayer = isPlayer;
        if (_isPlayer)
        {
            transform.localScale *= 1.1f;
        }
    }
}
