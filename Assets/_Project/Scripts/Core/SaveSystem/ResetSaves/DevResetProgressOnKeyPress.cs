using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DevResetProgressOnKeyPress : ITickable
{
    private const KeyCode RESET_KEY = KeyCode.X;
    private const string LOG_RESET_DONE = "[DEV] Saves cleared and scene reloaded.";

    private readonly Yg2GameSaveResetter resetter;

    public DevResetProgressOnKeyPress(Yg2GameSaveResetter resetter)
    {
        this.resetter = resetter;
    }

    void ITickable.Tick()
    {
        if (Input.GetKeyDown(RESET_KEY))
        {
            resetter.ResetAllSaves();

            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex);

            Debug.Log(LOG_RESET_DONE);
        }
    }
}