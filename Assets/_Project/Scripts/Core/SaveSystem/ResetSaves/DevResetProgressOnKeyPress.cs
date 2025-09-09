using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DevResetProgressOnKeyPress : ITickable
{
    private const KeyCode RESET_KEY = KeyCode.X;

    private readonly YGGameSaveResetter _resetter;

    public DevResetProgressOnKeyPress(YGGameSaveResetter resetter)
    {
        _resetter = resetter;
    }

    void ITickable.Tick()
    {
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if (isShiftHeld && Input.GetKeyDown(RESET_KEY))
        {
            _resetter.ResetAllSaves();

            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex);
        }
    }
}