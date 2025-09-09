using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DevReloadSceneOnKeyPress : ITickable
{
    private const KeyCode RELOAD_KEY = KeyCode.R;

    void ITickable.Tick()
    {
        if (Input.GetKeyDown(RELOAD_KEY))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex);
        }
    }
}