using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour {

    // Grid configuration constants
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 14;
    private const float CELL_SIZE = 10f;
    private const float GRID_OFFSET_X = -100f;
    private const float GRID_OFFSET_Y = -70f;
    private const float DISPLAY_SPRITE_X = -125f;
    private const float DISPLAY_SPRITE_Y = 0f;
    private const float Z_PLANE = 0f;
    private const float WHY_OFFSET = CELL_SIZE / 2f;
    private Vector3 mouseWorld;

    private Grid<GameObject> grid;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;
    private UtilityFunctions utilityFunctions;

    // Start is called before the first frame update
    private void Start() {
        testSprite = new Test_Sprite(); // Proper instantiation
        grid = new Grid<GameObject>(GRID_WIDTH, GRID_HEIGHT, CELL_SIZE, new Vector3(GRID_OFFSET_X, GRID_OFFSET_Y), testSprite.CreateSprite);
        // Position all grid objects correctly
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                Vector3 position = new Vector3(x * CELL_SIZE + GRID_OFFSET_X + CELL_SIZE / 2f, y * CELL_SIZE + GRID_OFFSET_Y + CELL_SIZE / 2f, Z_PLANE);
                GameObject obj = grid.GetGridObject(position);
                if (obj != null)
                {
                    // Snap the position to the grid
                    Vector3 snappedPosition = new Vector3(
                        Mathf.Floor((position.x - GRID_OFFSET_X) / CELL_SIZE) * CELL_SIZE + GRID_OFFSET_X + WHY_OFFSET,
                        Mathf.Floor((position.y - GRID_OFFSET_Y) / CELL_SIZE) * CELL_SIZE + GRID_OFFSET_Y + WHY_OFFSET,
                        Z_PLANE
                    );
                    obj.transform.position = snappedPosition;
                }
            }
        }
        // Create and position the display sprite for easy to see editing
        testSpriteObject = testSprite.CreateSprite();
        testSpriteObject.transform.position = new Vector3(DISPLAY_SPRITE_X, DISPLAY_SPRITE_Y, Z_PLANE);
        utilityFunctions = new UtilityFunctions();
    }

    // Update is called once per frame
    private void Update() {
        // Update mouse world position
        mouseWorld = utilityFunctions.worldMousePosition();

        // Handle mouse click to change sprite at mouse position
        if (Input.GetMouseButtonDown(0)) {
            GameObject obj = grid.GetGridObject(mouseWorld);
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

        // Handle mouse scroll to change display sprite
        if (Mouse.current != null)
        {
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();

            if (scrollDelta.y != 0f && testSpriteObject != null)
            {
                SpriteChanger spriteChanger = testSpriteObject.GetComponent<SpriteChanger>();
                if (spriteChanger != null) {
                    // Scroll up to go to next sprite
                    if (scrollDelta.y > 0f)
                    {
                        spriteChanger.ChangeSprite();
                    }
                    // Scroll down to go to previous sprite
                    else if (scrollDelta.y < 0f)
                    {
                        int newIndex = spriteChanger.GetCurrentSpriteIndex() - 1;
                        if (newIndex < 0) {
                            newIndex = spriteChanger.GetTotalSprites() - 1;
                        }
                        spriteChanger.ChangeSprite(newIndex);
                    }
                }
            }
        }
    }
}
