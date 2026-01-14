using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions {
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 14;
    private const float CELL_SIZE = 10f;
    private const float GRID_OFFSET_X = -100f;
    private const float GRID_OFFSET_Y = -70f;
    private const float DISPLAY_SPRITE_X = -125f;
    private const float DISPLAY_SPRITE_Y = 0f;
    private const float Z_PLANE = 0f;
    private const float WHY_OFFSET = CELL_SIZE / 2f;
    private static Camera mainCamera = Camera.main;
    private static Vector3 mouseWorldPosition;

    public Vector3 worldMousePosition() {
        if (mainCamera != null) {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = Z_PLANE;
        }
        return mouseWorldPosition;
    }

    public int getGridWidth() {
        return GRID_WIDTH;
    }

    public int getGridHeight() {
        return GRID_HEIGHT;
    }

    public float getCellSize() {
        return CELL_SIZE;
    }

    public Vector3 getGridOffset() {
        return new Vector3(GRID_OFFSET_X, GRID_OFFSET_Y);
    }

    public Vector3 getDisplaySpritePosition() {
        return new Vector3(DISPLAY_SPRITE_X, DISPLAY_SPRITE_Y, Z_PLANE);
    }

    public float getZPlane() {
        return Z_PLANE;
    }

    public float getWhyOffset() {
        return WHY_OFFSET;
    }

    public int[,] GetGridArrayTileValues(Grid<GameObject> grid) {
        int[,] intArray = new int[grid.width, grid.height];
            for (int x = 0; x < grid.width; x++) {
                for (int y = 0; y < grid.height; y++) {
                    SpriteChanger spriteChanger = grid.gridArray[x, y].GetComponent<SpriteChanger>();
                    if (spriteChanger != null) {
                        int tile = spriteChanger.GetCurrentSpriteIndex();
                        switch (tile) {
                            case 0:
                                intArray[x, y] = 1; // Walkable tile
                                break;
                                case 1:
                                intArray[x, y] = 2; // Desired tile
                                break;
                            default:
                                intArray[x, y] = 0; // Unknown tile
                                break;
                        }
                    }
                }
            }
            return intArray;
        
    }

    public void setBlockedTile(GameObject obj, Grid<GameObject> grid)
    {
        grid.SetGridObject(obj.transform.position, obj);
    }
}