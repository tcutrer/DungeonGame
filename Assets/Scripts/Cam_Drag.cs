using UnityEngine;
using UnityEngine.InputSystem;

public class Cam_Drag : MonoBehaviour
{
    private Vector3 _origin;
    private Vector3 _difference;

    private Camera _mainCamera;

    private bool _isDragging;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        if (context.started) _origin = GetMousePosition;
        _isDragging = context.started || context.performed;

    }

    private void LateUpdate()
    {
        if (!_isDragging) return;

        _difference = GetMousePosition - transform.position;
        transform.position = _origin - _difference;
    }

    private Vector3 GetMousePosition => _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

}
