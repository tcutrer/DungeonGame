using UnityEngine;
using UnityEngine.InputSystem;

public class UnselectBlock : MonoBehaviour
{

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("PlaceBlock: Main camera not found!");
        }
    }

    public void Unselect(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_mainCamera == null)
        {
            Debug.LogError("PlaceBlock: Main camera is null!");
            return;
        }

        if (Game_Manger.instance == null)
        {
            Debug.LogError("PlaceBlock: Game_Manger instance is null!");
            return;
        }
        if (PauseScript.isPaused) return;
        Game_Manger.instance.UnselectBlock();
    }
}
