using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Game_Manger : MonoBehaviour
{
    // Private Variables
    private bool Is_play = false;
    private float Cycle_time = 0f;
    private int Unit_nums = 0;
    private int Phase = 0;
    private Grid<GameObject> grid;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;
    private Vector3 mouseWorld;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private UtilityFunctions UF;
    [SerializeField] private GameObject farmerPrefab;
    private List<int> creatureCostumes = new List<int> {5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
    private Adventurer adventurer;
    public int[,] tileValues { get; private set; }
    


    public static Game_Manger instance { get; private set; }

    // Getters
    public bool Get_Is_play()
    {
        return Is_play;
    }
    public float Get_Cycle_time()
    {
        return Cycle_time;
    }
    public int Get_Unit_nums()
    {
        return Unit_nums;
    }
    public int Get_Phase()
    {
        return Phase;
    }

    // Setters
    public void Set_Is_play(bool value)
    {
        Is_play = value;
    }
    public void Set_Cycle_time(float value)
    {
        Cycle_time = value;
    }
    public void Set_Unit_nums(int value)
    {
        Unit_nums = value;
    }
    public void Set_Phase(int value)
    {
        Phase = value;
    }

    // Other Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        // Initialize utilities and sprites
        UF = new UtilityFunctions();
        testSprite = new Test_Sprite();
        pathfinding = PathfindingManager.Instance.GetPathfinding();
        
        // Initialize grid
        grid = new Grid<GameObject>(UF.getGridWidth(), UF.getGridHeight(), UF.getCellSize(), UF.getGridOffset(), testSprite.CreateSprite);
        
        // Position all grid objects correctly
        for (int x = 0; x < UF.getGridWidth(); x++)
        {
            for (int y = 0; y < UF.getGridHeight(); y++)
            {
                Vector3 position = new Vector3(x * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(), y * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(), UF.getZPlane());
                GameObject obj = grid.GetGridObject(position);
                if (obj != null)
                {
                    // Snap the position to the grid
                    Vector3 snappedPosition = new Vector3(
                        Mathf.Floor((position.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                        Mathf.Floor((position.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                        UF.getZPlane()
                    );
                    obj.transform.position = snappedPosition;
                }
            }
        }
        
        // Create and position the display sprite
        testSpriteObject = testSprite.CreateSprite();
        testSpriteObject.transform.position = new Vector3(UF.getDisplaySpritePosition().x, UF.getDisplaySpritePosition().y, UF.getZPlane());
    }
    
    public void PlaceBlock(Vector3 position)
    {
        if (grid == null)
        {
            Debug.LogError("Grid is null in PlaceBlock!");
            return;
        }

        if (testSpriteObject == null)
        {
            Debug.LogError("testSpriteObject is null in PlaceBlock!");
            return;
        }

       GameObject obj = grid.GetGridObject(mouseWorld);
            if (obj != null) {
                SpriteChanger spriteChanger = obj.GetComponent<SpriteChanger>();
                if (spriteChanger != null) {
                    SpriteChanger displaySprite = testSpriteObject.GetComponent<SpriteChanger>();
                    if (displaySprite != null) {
                        int costumeIndex = displaySprite.GetCurrentSpriteIndex();
                        for (int i = 0; i < creatureCostumes.Count; i++) {
                            if (costumeIndex == creatureCostumes[i]) {
                                Vector3 snappedPosition = new Vector3(
                                    Mathf.Floor((mouseWorld.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                                    Mathf.Floor((mouseWorld.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                                    UF.getZPlane()
                                );
                                adventurer = Adventurer.CreateAdventurer(farmerPrefab, snappedPosition);
                                costumeIndex = 0;
                            }
                        }
                        spriteChanger.ChangeSprite(costumeIndex);
                    }
                }
            }
    }

    private void Update()
    {
        // Update mouse world position
        mouseWorld = UF.worldMousePosition();

        // Set walkables for pathfinding
        if (pathfinding == null)
        {
            pathfinding = PathfindingManager.Instance.GetPathfinding();
        }
        tileValues = UF.GetGridArrayTileValues(grid);
        pathfinding.SetWalkables(tileValues);

        // Handle right click for pathfinding
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click detected");
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorld, out x, out y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                Debug.Log("Path found with length: " + path.Count);
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.Log("Drawing line from (" + path[i].GetX() + ", " + path[i].GetY() + ") to (" + path[i + 1].GetX() + ", " + path[i + 1].GetY() + ")");
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * UF.getCellSize() +
                    new Vector3(-UF.getGridOffset().x + UF.getWhyOffset(), -UF.getGridOffset().y + UF.getWhyOffset()),
                    new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * UF.getCellSize() + new Vector3(-UF.getGridOffset().x + UF.getWhyOffset(), -UF.getGridOffset().y + UF.getWhyOffset()), Color.green, 5f
                    );
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
                if (spriteChanger != null)
                {
                    // Scroll up to go to next sprite
                    if (scrollDelta.y > 0f)
                    {
                        spriteChanger.ChangeSprite();
                    }
                    // Scroll down to go to previous sprite
                    else if (scrollDelta.y < 0f)
                    {
                        int newIndex = spriteChanger.GetCurrentSpriteIndex() - 1;
                        if (newIndex < 0)
                        {
                            newIndex = spriteChanger.GetTotalSprites() - 1;
                        }
                        spriteChanger.ChangeSprite(newIndex);
                    }
                }
            }
        }
    }
    }