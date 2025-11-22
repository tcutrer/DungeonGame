using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour {
    private Vector3 mouseWorld;

    private Grid<GameObject> grid;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;
    private UtilityFunctions UF;

    // Start is called before the first frame update
    private void Start() {
        UF = new UtilityFunctions();
        testSprite = new Test_Sprite();
        grid = new Grid<GameObject>(UF.getGridWidth(), UF.getGridHeight(), UF.getCellSize(), UF.getGridOffset(), testSprite.CreateSprite);
        // Position all grid objects correctly
        for (int x = 0; x < UF.getGridWidth(); x++)
        {
            for (int y = 0; y < UF.getGridHeight(); y++)
            {
                // Vector3 position = new Vector3(x * CELL_SIZE + GRID_OFFSET_X + CELL_SIZE / 2f, y * CELL_SIZE + GRID_OFFSET_Y + CELL_SIZE / 2f, Z_PLANE);
                Vector3 position = new Vector3(x * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(), y * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(), UF.getZPlane());
                GameObject obj = grid.GetGridObject(position);
                if (obj != null)
                {
                    // Snap the position to the grid
                    Vector3 snappedPosition = new Vector3(
                        // Mathf.Floor((position.x - GRID_OFFSET_X) / CELL_SIZE) * CELL_SIZE + GRID_OFFSET_X + WHY_OFFSET,
                        // Mathf.Floor((position.y - GRID_OFFSET_Y) / CELL_SIZE) * CELL_SIZE + GRID_OFFSET_Y + WHY_OFFSET,
                        Mathf.Floor((position.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                        Mathf.Floor((position.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                        UF.getZPlane()
                    );
                    obj.transform.position = snappedPosition;
                }
            }
        }
        // Create and position the display sprite for easy to see editing
        testSpriteObject = testSprite.CreateSprite();
        testSpriteObject.transform.position = new Vector3(UF.getDisplaySpritePosition().x, UF.getDisplaySpritePosition().y, UF.getZPlane());
    }

    // Update is called once per frame
    private void Update() {
        // Update mouse world position
        mouseWorld = UF.worldMousePosition();

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
