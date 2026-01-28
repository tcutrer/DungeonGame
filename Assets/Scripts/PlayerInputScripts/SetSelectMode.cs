using UnityEngine;
using UnityEngine.InputSystem;

public class SetSelectMode : MonoBehaviour
{
    private Camera _mainCamera;
    private bool wasSelectMode = false;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("SetSelectMode: Main camera not found!");
        }
    }
    public void press(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_mainCamera == null)
        {
            Debug.LogError("SetSelectMode: Main camera is null!");
            return;
        }

        if (Game_Manger.instance == null)
        {
            Debug.LogError("SetSelectMode: Game_Manger instance is null!");
            return;
        }
        if (PauseScript.isPaused) return;
        if (wasSelectMode == false)
        {
            setSelectModeTrue();
            wasSelectMode = true;
        }
        else
        {
            setSelectModeFalse();
            wasSelectMode = false;
        }

       
    }

    public void setSelectModeTrue()
    {
        if (Game_Manger.instance == null)
        {
            Debug.LogError("SetSelectMode: Game_Manger instance is null!");
            return;
        }

        Game_Manger.instance.setSelectModeTrue(true);
    }

    public void setSelectModeFalse()
    {
        if (Game_Manger.instance == null)
        {
            Debug.LogError("SetSelectMode: Game_Manger instance is null!");
            return;
        }

        Game_Manger.instance.setSelectModeTrue(false);
    }

}
