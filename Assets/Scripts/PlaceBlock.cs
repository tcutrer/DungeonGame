using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlock : MonoBehaviour
{

    private Camera _mainCamera;

    public void place(InputAction.CallbackContext context)
    {
        if (PauseScript.isPaused) return;
        if (context.started)
        {
            Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0f; // Assuming a 2D game in the XY plane

            //Call place logic
        }
    }

}
