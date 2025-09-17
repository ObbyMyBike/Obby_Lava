using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishView : MonoBehaviour
{
    public event Action OnPlayerFinished;

    [SerializeField] private List<ParticleSystem> _finishConfetti;
    [SerializeField] private FinishUI _finishUI;

    private void Awake()
    {
        _finishUI.Hide();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnPlayerFinished?.Invoke();
        }
    }

    public void ActivateFinishing(float finishTime)
    {
        _finishConfetti.ForEach(ps => ps.Play());
        StartCoroutine(ShowUIAfterTime(finishTime));
    }

    private IEnumerator ShowUIAfterTime(float finishTime)
    {
        yield return new WaitForSeconds(2f);


        int totalSecondsInt = Mathf.FloorToInt(finishTime);
        int minutesInt = totalSecondsInt / 60;
        int secondsInt = totalSecondsInt % 60;

        string timeText = $"Time: {minutesInt:00}:{secondsInt:00}";

        _finishUI.SetTimeText(timeText);
        _finishUI.Show();
        Time.timeScale = 0f;
    }
}
