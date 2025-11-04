using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject> { // Generic Grid class using Templates to allow any type of object to be stored in the grid
    /*
    * Varibles public to whole class
    * width: width/x of the grid
    * height: height/y of the grid
    * cellSize: size of each cell in the grid in world units
    * originPosition: world position of the bottom left corner of the grid
    * gridArray: 2D array to store the grid objects
    * debugTextArray: 2D array to store the debug text objects
    *
    * Class Methods
    * Grid: constructor to initialize the grid
    *      Parameters: width/x, height/y, cellSize, originPosition, createGridObject (function to create grid objects)
    * GetWorldPosition: converts grid coordinates to world position
    *      Parameters: x in grid coords, y in grid coords
    * GetXY: converts world position to grid coordinates
    *      Parameters: worldPosition, out x in grid coords, out y in grid coords
    * SetGridObject: sets the grid object at the specified grid coordinates
    *      Parameters: x in grid coords, y in grid coords, value to set
    * SetGridObject: sets the grid object at the specified world position
    *      Parameters: worldPosition, value to set
    * GetGridObject: gets the grid object at the specified grid coordinates
    *      Parameters: x in grid coords, y in grid coords
    * GetGridObject: gets the grid object at the specified world position
    *      Parameters: worldPosition
    * CreateWorldText: creates a TextMesh object for debugging purposes
    *      Parameters: text, parent transform, localPosition, fontSize, color, textAnchor, textAlignment
    *
    * Notes:
    */
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<TGridObject> createGridObject) {
        /*
        * Constructor to initialize the grid
        * Parameters: 
        *      width: cell width/x of the grid
        *      height: cell height/y of the grid
        *      cellSize: size of each cell in the grid in world units
        *      originPosition: world position of the bottom left corner of the grid
        *      createGridObject: function to create grid objects
        */
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x< gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++){
                gridArray[x, y] = createGridObject();
            }
        }

        bool showDebug = true; // Toggle debug visualization and rest of method is for debugging
        if (showDebug) {
            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 10f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 10f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 10f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 10f);
        }
    }

    private Vector3 GetWorldPosition(int x, int y) {
        /*
        * Converts grid coordinates to world position
        * Parameters:
        *      x: x in grid coords
        *      y: y in grid coords
        * Returns: world position as Vector3
        */
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y) {
        /*
        * Converts world position to grid coordinates
        * Parameters:
        *      worldPosition: world position as Vector3
        *      out x: x in grid coords
        *      out y: y in grid coords
        */
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        /*
        * Sets the grid object at the specified grid coordinates
        * Parameters:
        *      x: x in grid coords
        *      y: y in grid coords
        *      value: value to set
        */
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        /*
        * Sets the grid object at the specified world position
        * Parameters:
        *      worldPosition: world position as Vector3
        *      value: value to set
        */
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        /*
        * Gets the grid object at the specified grid coordinates
        * Parameters:
        *      x: x in grid coords
        *      y: y in grid coords
        * Returns: grid object at specified coordinates
        */
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        /*
        * Gets the grid object at the specified world position
        * Parameters:
        *      worldPosition: world position as Vector3
        * Returns: grid object at specified world position
        */
        int x , y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color color = default(Color), TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left) {
        /*
        * NOTE: This method is purley for debugging purposes and can be moved to a utility class if needed
        *
        * Creates a TextMesh object for debugging purposes
        * Parameters:
        *      text: text to display
        *      parent: parent transform
        *      localPosition: local position as Vector3
        *      fontSize: font size
        *      color: text color
        *      textAnchor: text anchor
        *      textAlignment: text alignment
        * Returns: TextMesh object
        */
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = 5;
        return textMesh;
    }



}
