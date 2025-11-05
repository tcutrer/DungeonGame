using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    private Grid<GameObject> grid;
    public Camera mainCamera;
    public Vector3 mouseWorldPosition;

    // Start is called before the first frame update
    private void Start() {
        Test_Sprite spriteCreator = new Test_Sprite();
        grid = new Grid<GameObject>(20, 14, 10f, new Vector3(-100, -70), spriteCreator.CreateSprite);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        for (int x = 0; x < 20; x++) {
            for (int y = 0; y < 14; y++) {
                Vector3 position = new Vector3(x * 10f - 95f, y * 10f - 65f, 0f);
                GameObject obj = grid.GetGridObject(position);
                if (obj != null) {
                    obj.SetActive(true);
                    // Snap the position to the grid
                    Vector3 snappedPosition = new Vector3(
                        Mathf.Floor((position.x + 100) / 10f) * 10f - 95f,
                        Mathf.Floor((position.y + 70) / 10f) * 10f - 65f,
                        0f
                    );
                    obj.transform.position = snappedPosition;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update() {
        if (mainCamera != null) {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // Keep sprites on the same z-plane
    }
    
    if (Input.GetMouseButtonDown(0)) {
        GameObject obj = grid.GetGridObject(mouseWorldPosition);
        if (obj != null) {
            SpriteChanger spriteChanger = obj.GetComponent<SpriteChanger>();
                if (spriteChanger != null) {
                    spriteChanger.ChangeSprite();
                }
        }
    }

        if (Input.GetMouseButtonDown(1)) {
            GameObject obj = grid.GetGridObject(mouseWorldPosition);
            if (obj != null) {
                obj.SetActive(false);
            }
        }
    }
}
