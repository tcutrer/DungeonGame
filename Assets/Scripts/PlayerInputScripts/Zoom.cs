using UnityEngine;
using UnityEngine.InputSystem;

public class Zoom : MonoBehaviour
{
    private Camera _mainCamera;
    private float min_zoom;
    private float max_zoom;
    private float zoom_sensitivity;

    private void Awake()
    {
        _mainCamera = Camera.main;
        min_zoom = 5f;
        max_zoom = 100f;
        zoom_sensitivity = 1f;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (PauseScript.isPaused) return;
        Vector2 value = context.ReadValue<Vector2>();
        float scrollValue = value.y;
        float newSize = _mainCamera.orthographicSize - scrollValue * zoom_sensitivity;
        _mainCamera.orthographicSize = Mathf.Clamp(newSize, min_zoom, max_zoom);
    }
}
