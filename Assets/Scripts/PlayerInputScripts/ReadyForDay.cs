using UnityEngine;
using UnityEngine.InputSystem;

public class ReadyForDay : MonoBehaviour
{
    private Camera _mainCamera;
    private void Awake()
    {
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("ReadyForDay: Main camera not found!");
        }
    }
    public void Ready(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_mainCamera == null)
        {
            Debug.LogError("ReadyForDay: Main camera is null!");
            return;
        }

        if (Game_Manger.instance == null)
        {
            Debug.LogError("ReadyForDay: Game_Manger instance is null!");
            return;
        }
        if (PauseScript.isPaused) return;
        Game_Manger.instance.ReadyForDay();
    }
}
