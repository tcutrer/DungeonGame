using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlock : MonoBehaviour
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

    public void place()
    {
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

        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Game_Manger.instance.PlaceBlock(mouseWorld);
    }
}



