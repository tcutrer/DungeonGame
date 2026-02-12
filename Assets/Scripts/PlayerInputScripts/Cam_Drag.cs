using UnityEngine;
using UnityEngine.InputSystem;

public class Cam_Drag : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;

    private Camera _mainCamera;

    private bool _isDragging;

    [SerializeField] private float minCameraX = -50f;
    [SerializeField] private float maxCameraX = 50f;
    [SerializeField] private float minCameraY = -30f;
    [SerializeField] private float maxCameraY = 30f;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        if (PauseScript.isPaused || UITextManager.isRoomMenuOpen) return;
        if (context.started) _origin = GetMousePosition;
        _isDragging = context.started || context.performed;

    }

    private void LateUpdate()
    {
        if (PauseScript.isPaused || UITextManager.isRoomMenuOpen) return;
        if (!_isDragging) return;

        _difference = GetMousePosition - transform.position;
        Vector3 newPosition = _origin - _difference;
        
        // Clamp the camera position within bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minCameraX, maxCameraX);
        newPosition.y = Mathf.Clamp(newPosition.y, minCameraY, maxCameraY);
        newPosition.z = transform.position.z; // Keep z unchanged
        
        transform.position = newPosition;
    }

    private Vector3 GetMousePosition => _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

}
