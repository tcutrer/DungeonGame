using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject playerUI;

    void Awake()
    {
        pauseMenuUI.SetActive(false);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;

    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

    }

}
