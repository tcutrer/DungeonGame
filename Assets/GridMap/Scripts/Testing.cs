using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour {

    private Grid<GameObject> grid;
    public Camera mainCamera;
    public Vector3 mouseWorldPosition;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;

    // Start is called before the first frame update
    private void Start() {
        testSprite = new Test_Sprite(); // Proper instantiation
        grid = new Grid<GameObject>(20, 14, 10f, new Vector3(-100, -70), testSprite.CreateSprite);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 14; y++)
            {
                Vector3 position = new Vector3(x * 10f - 95f, y * 10f - 65f, 0f);
                GameObject obj = grid.GetGridObject(position);
                if (obj != null)
                {
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
        testSpriteObject = testSprite.CreateSprite();
        testSpriteObject.transform.position = new Vector3(-125, 0, 0);
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
                    SpriteChanger displaySprite = testSpriteObject.GetComponent<SpriteChanger>();
                    if (displaySprite != null) {
                        int costumeIndex = displaySprite.GetCurrentSpriteIndex();
                        spriteChanger.ChangeSprite(costumeIndex);
                    }
                }
        }
    }

    if (Mouse.current != null)
        {
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();

            if (scrollDelta.y > 0f)
            {
                SpriteChanger spriteChanger = testSpriteObject.GetComponent<SpriteChanger>();
                if (spriteChanger != null) {
                    spriteChanger.ChangeSprite();
                }
            }
        }
    }
}
