using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Game_Manger : MonoBehaviour
{
    // Private Variables
    public bool select_mode = true;
    private bool Is_play = false;
    private float Cycle_time = 0f;
    private bool isnight = true;
    private float time_to_explore = 0f;
    private float setTimeToExplore = 30f; 
    private bool areExplorersGone = true;
    private bool isDay = false;
    private int Unit_nums = 0;
    private int Phase = 0;
    private Grid<GameObject> grid;
    public GameObject PlayerUI;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;
    private Vector3 mouseWorld;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private UtilityFunctions UF;
    [SerializeField] private GameObject farmerPrefab;
    [SerializeField] private GameObject mushlingPrefab;
    private List<int> creatureCostumes = new List<int> {5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
    private Adventurer adventurer;
    private Creature creature;
    public float adventurercount = 0f;
    public int[,] tileValues { get; private set; }
    
    private int selectedSpriteIndex = 0;

    
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
        grid = new Grid<GameObject>(UF.getGridWidth() * UF.amountOfRooms[0], UF.getGridHeight() * UF.amountOfRooms[1], UF.getCellSize(), UF.getGridOffset(), testSprite.CreateSprite);
        
        // Position all grid objects correctly
        for (int x = 0; x < UF.getGridWidth() * UF.amountOfRooms[0]; x++)
        {
            for (int y = 0; y < UF.getGridHeight() * UF.amountOfRooms[1]; y++)
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
                if ((x % UF.getGridWidth() == 0 || x % UF.getGridWidth() == (UF.getGridWidth() - 1)) || (y % UF.getGridHeight() == 0 || y % UF.getGridHeight() == (UF.getGridHeight() - 1)))
                {
                    GameObject tile = grid.GetGridObject(position);
                    if (tile != null)
                    {
                        SpriteChanger spriteChanger = tile.GetComponent<SpriteChanger>();
                        if (spriteChanger != null)
                        {
                            spriteChanger.ChangeSprite(1); // Set wall sprite
                        }
                    }
                }
                if (x == UF.getGridWidth() / 2 && y == 0)
                {
                    GameObject tile = grid.GetGridObject(position);
                    if (tile != null)
                    {
                        SpriteChanger spriteChanger = tile.GetComponent<SpriteChanger>();
                        if (spriteChanger != null)
                        {
                            spriteChanger.ChangeSprite(2); // Set door sprite
                        }
                    }
                }
            }
        }
    }
    
    public void PlaceBlock(Vector3 position)
    {
        if (grid == null)
        {
            Debug.LogError("Grid is null in PlaceBlock!");
            return;
        }

        GameObject obj = grid.GetGridObject(mouseWorld);
        if (isDay == true) return;
        if (select_mode == true) return;
        if (obj != null) {
            SpriteChanger spriteChanger = obj.GetComponent<SpriteChanger>();
            if (spriteChanger != null) {
                // Check if selected sprite is a creature costume
                for (int i = 0; i < creatureCostumes.Count; i++) {
                    if (selectedSpriteIndex == creatureCostumes[i]) {
                        Vector3 snappedPosition = new Vector3(
                            Mathf.Floor((mouseWorld.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                            Mathf.Floor((mouseWorld.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                            UF.getZPlane()
                        );
                        if (i == 0)
                        {
                            creature = Creature.CreateCreature(mushlingPrefab, snappedPosition);
                        }
                        //creature = Creature.CreateCreature(mushlingPrefab, snappedPosition);
                    }
                }
                spriteChanger.ChangeSprite(selectedSpriteIndex);
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

        if (isDay == true && time_to_explore < setTimeToExplore)
        {
            time_to_explore += Time.deltaTime;
        }
        
        if (adventurercount <= 0f)
        {
            areExplorersGone = true;
        }
        if (time_to_explore >= setTimeToExplore && areExplorersGone == true)
        {
            isDay = false;
            time_to_explore = 0f;
            setNight();
            isnight = true;
            
        }

    }

    public void SelectSprite(int spriteIndex)
    {
        selectedSpriteIndex = spriteIndex;
    }

    public void UnselectBlock()
    {
        selectedSpriteIndex = 0; // Reset to default or no selection
    }
    public void ReadyForDay()
    {
        if (isnight == true && isDay == false)
        {
            setDay();
            isDay = true;
            isnight = false;
        }
    }
    private void setNight()
    {
        isDay = false;
        isnight = true;
        UnselectBlock();
        PlayerUI.SetActive(true);
    }
    private void setDay()
    {
        isnight = false;
        isDay = true;
        UnselectBlock();
        PlayerUI.SetActive(false);
    }
    public void incrementadventurercount(float value)
    {
        adventurercount += value;
    }
    public void setSelectModeTrue(bool value)
    {
        select_mode = value;
    }
}