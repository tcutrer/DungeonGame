using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions {
    private const float Z_PLANE = 0f;

    public static Camera mainCamera = Camera.main;
    private static Vector3 mouseWorldPosition;

    public Vector3 worldMousePosition() {
        if (mainCamera != null) {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = Z_PLANE;
        }
        return mouseWorldPosition;
    }
}