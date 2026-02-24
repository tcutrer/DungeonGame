using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject playerUI;
    public bool shouldDisablePauseAbility = false;

    private Game_Manger gameManager;

    void Awake()
    {
        pauseMenuUI.SetActive(false);
        gameManager = Game_Manger.instance;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (shouldDisablePauseAbility) {
            return;
        }
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
        if (gameManager == null) {
            gameManager = Game_Manger.instance; // Try to find the game manager again
        }
        
        if (gameManager.isGameOver) {
            pauseMenuUI.SetActive(false);
            playerUI.SetActive(false);
            Time.timeScale = 0f; // Ensure the game is not paused if it's already over
            isPaused = true;
            // Don't allow pausing if the game is already over
        }
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void setIsPaused(bool value) {
        isPaused = value;
    }

}
