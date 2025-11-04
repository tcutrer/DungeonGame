using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    private Grid<bool> grid;
    public Camera mainCamera;
    public Vector3 mouseWorldPosition;

    // Start is called before the first frame update
    private void Start() {
        // Create test grid
        grid = new Grid<bool>(20, 14, 10f, new Vector3(-100, -70), () => false);
        // Get main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (mainCamera != null)
        {
            //Update mouse world position
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 0;
            mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        }
        
        // Left click to set true at mouse position, right click to get value at mouse position
        if (Input.GetMouseButtonDown(0)) {
            grid.SetGridObject(mouseWorldPosition, true);
        }

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log(grid.GetGridObject(mouseWorldPosition));
        }
    }
}
