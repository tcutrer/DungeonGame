using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Game_Manger : MonoBehaviour
{
    // Private Variables
    public bool select_mode = true;
    private int wavecount = 0;
    private List<int> adventurercountPerWave = new List<int> {3, 5, 7, 10, 12, 15};
    private bool Is_play = false;
    private float Cycle_time = 0f;
    private bool isnight = true;
    private float time_to_explore = 0f;
    private float setTimeToExplore = 5f; 
    public bool areExplorersGone = true;
    private bool isDay = false;
    private int Unit_nums = 0;
    private int Phase = 0;
    public bool isGameOver = false;
    public Grid<GameObject> grid;
    public GameObject PlayerUI;
    public GameObject GameOverScreen;
    public GameObject pauseManager;
    private Test_Sprite testSprite;
    private GameObject testSpriteObject;
    private Vector3 mouseWorld;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private UtilityFunctions UF;
    //Prefabs
    [SerializeField] private GameObject farmerPrefab;
    [SerializeField] private GameObject mushlingPrefab;
    [SerializeField] private GameObject batpirePrefab;

    private List<int> creatureCostumes = new List<int> {5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};
    private Adventurer adventurer;
    private Creature creature;
    public float adventurercount = 0f;
    public int[,] tileValues { get; private set; }
    public bool[,] roomAvailable;
    public Vector3 spawnPoint = new Vector3(-35, -35, -1);
    
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
        UF = new UtilityFunctions();
        testSprite = new Test_Sprite();
        pathfinding = PathfindingManager.Instance.GetPathfinding();
        setStartingOwnedRooms();
        setupGrid();
        GameOverScreen.SetActive(false);
    }

    private void setStartingOwnedRooms() 
    {
        roomAvailable = new bool[UF.amountOfRooms[0], UF.amountOfRooms[1]];
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                roomAvailable[x, y] = true;
            }
        }
    }

    private void setupGrid()
    {
        grid = new Grid<GameObject>(UF.getGridWidth() * UF.amountOfRooms[0], UF.getGridHeight() * UF.amountOfRooms[1], UF.getCellSize(), UF.getGridOffset(), testSprite.CreateSprite);
        
        // Position all grid objects correctly
        for (int x = 0; x < UF.getGridWidth() * UF.amountOfRooms[0]; x++)
        {
            for (int y = 0; y < UF.getGridHeight() * UF.amountOfRooms[1]; y++)
            {
                Vector3 position = new Vector3(x * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(), y * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(), UF.getZPlane());
                GameObject obj = grid.GetGridObject(position);
                if (obj == null) continue;
                Vector3 snappedPosition = new Vector3(
                    Mathf.Floor((position.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                    Mathf.Floor((position.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                    UF.getZPlane()
                );
                obj.transform.position = snappedPosition;
                setWallsUp(grid, x, y, position);
                setUpMainDoor(grid, x, y, position);
            }
        }
    }

    private void setWallsUp(Grid<GameObject> grid, int x, int y, Vector3 position) 
    {
        if ((x % UF.getGridWidth() == 0 || x % UF.getGridWidth() == (UF.getGridWidth() - 1)) || (y % UF.getGridHeight() == 0 || y % UF.getGridHeight() == (UF.getGridHeight() - 1)))
            {
                GameObject tile = grid.GetGridObject(position);
                if (tile == null) return;
                SpriteChanger spriteChanger = tile.GetComponent<SpriteChanger>();
                if (spriteChanger == null) return;
                spriteChanger.ChangeSprite(1); // Set wall sprite
            }
        }

    private void setUpMainDoor(Grid<GameObject> grid, int x, int y, Vector3 position) 
    {
        if (x == UF.getGridWidth() / 2 && y == 0)
            {
                GameObject tile = grid.GetGridObject(position);
                if (tile == null) return;
                SpriteChanger spriteChanger = tile.GetComponent<SpriteChanger>();
                if (spriteChanger == null) return;
                spriteChanger.ChangeSprite(2); // Set door sprite
            }
        }

    
    public void PlaceBlock(Vector3 position)
    {
        if (grid == null) return;

        if (CurrencyManager.Instance == null) return;

        if (!roomAvailable[(int)(UF.WorldToGridCoords(position).x / UF.getGridWidth()), (int)(UF.WorldToGridCoords(position).y / UF.getGridHeight())])
        {
            Debug.Log("PlaceBlock: Attempted to place block outside of availible bounds!");
            UITextManager.Instance.ShowRoomPurchaseText(50, position);
            return;
        }
        Debug.Log((UF.WorldToGridCoords(position).x / UF.getGridWidth()) + ", " + (UF.WorldToGridCoords(position).y / UF.getGridHeight()));

        GameObject obj = grid.GetGridObject(mouseWorld);
        if (isDay == true) return;
        if (select_mode == true) return;
        if (obj == null) return;

        SpriteChanger spriteChanger = obj.GetComponent<SpriteChanger>();
        if (spriteChanger == null) return;

        if (CurrencyManager.Instance.SpendGold(spriteChanger.GetCost(selectedSpriteIndex)) == false) return;
        // Check if selected sprite is a creature costume
        for (int i = 0; i < creatureCostumes.Count; i++) {
            if (selectedSpriteIndex != creatureCostumes[i]) continue;
            Vector3 snappedPosition = new Vector3(
                Mathf.Floor((mouseWorld.x - UF.getGridOffset().x) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().x + UF.getWhyOffset(),
                Mathf.Floor((mouseWorld.y - UF.getGridOffset().y) / UF.getCellSize()) * UF.getCellSize() + UF.getGridOffset().y + UF.getWhyOffset(),
                UF.getZPlane()
            );
            switch(i) {
                case 0:
                    creature = Creature.CreateCreature(mushlingPrefab, snappedPosition);
                    break;
                case 1:
                    creature = Creature.CreateCreature(batpirePrefab, snappedPosition);
                    break;
                case 2:
                    adventurer = Adventurer.CreateAdventurer(farmerPrefab, snappedPosition);
                    break;
                default:
                    creature = Creature.CreateCreature(mushlingPrefab, snappedPosition);
                    break;
            }
        }
        spriteChanger.ChangeSprite(selectedSpriteIndex);
    }

    public void unlockRoom(Vector3 position)
    {
        int roomX = (int)(UF.WorldToGridCoords(position).x / UF.getGridWidth());
        int roomY = (int)(UF.WorldToGridCoords(position).y / UF.getGridHeight());
        if (roomX < 0 || roomX >= UF.amountOfRooms[0] || roomY < 0 || roomY >= UF.amountOfRooms[1])
        {
            Debug.LogError("UnlockRoom: Attempted to unlock room outside of bounds!");
            return;
        }
        roomAvailable[roomX, roomY] = true;
        Debug.Log("Room at " + roomX + ", " + roomY + " unlocked!");
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
        

        if (time_to_explore >= setTimeToExplore && areExplorersGone == true)
        {
            isDay = false;
            setNight();
            isnight = true;
            
        }

        if (isGameOver == true) {
            GameOverScreen.SetActive(true);
            FadeObjectInOnObject fadeScript = GameOverScreen.GetComponent<FadeObjectInOnObject>();

            if (fadeScript != null)
            {
                fadeScript.startFadeIn();
            }
            pauseManager.GetComponent<PauseScript>().shouldDisablePauseAbility = true;
            pauseManager.GetComponent<PauseScript>().OnPause(new InputAction.CallbackContext());
            pauseManager.GetComponent<PauseScript>().setIsPaused(true); // Force pause the game when game over
            Time.timeScale = 0f; // Pause the game
            // You can add additional game over logic here, such as showing a game over screen or restarting the game.
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
        List<Vector2> tilesToCheckPathfind = new List<Vector2> {};
        Vector2 mainDoorCords = new Vector2(-1, -1);
        Vector2 hordeCords = new Vector2(-1, -1);
        bool goodGrid = true;
        for (int x = 0; x < UF.getGridWidth() * UF.amountOfRooms[0]; x++)
        {
            for (int y = 0; y < UF.getGridHeight() * UF.amountOfRooms[1]; y++)
            {
                int tileValue = grid.GetGridObject(x, y).GetComponent<SpriteChanger>().GetCurrentSpriteIndex();
                switch (tileValue)
                {
                    case 2:
                        if (mainDoorCords.x == -1 && mainDoorCords.y == -1)
                        {
                            mainDoorCords = new Vector2(x, y);
                        }
                        else
                        {
                            goodGrid = false;
                        }
                        break;
                    case 3: 
                        if (hordeCords.x == -1 && hordeCords.y == -1)
                        {
                            hordeCords = new Vector2(x, y);
                        }
                        else
                        {
                            goodGrid = false;
                        }
                        break;
                    case 4:
                        tilesToCheckPathfind.Add(new Vector2(x, y));
                        break;
                    default:
                        if ((x == 0 || x == (UF.getGridWidth() * UF.amountOfRooms[0] - 1) || y == 0 || y == (UF.getGridHeight() * UF.amountOfRooms[1] - 1)) && tileValue != 1)
                        {
                            Debug.Log("ReadyForDay: Outer wall tile at " + x + ", " + y + " is not set as a wall! Please fix your grid!");
                            goodGrid = false;
                        }
                        break;
                }
            }
        }
        if (goodGrid == false || mainDoorCords.x == -1 || mainDoorCords.y == -1 || hordeCords.x == -1 || hordeCords.y == -1)
        {
            return;
        }
        for (int i = 0; i < tilesToCheckPathfind.Count; i++)
        {
            path = pathfinding.FindPath((int)mainDoorCords.x, (int)mainDoorCords.y, (int)tilesToCheckPathfind[i].x, (int)tilesToCheckPathfind[i].y);
            if (path == null)
            {
                Debug.Log("ReadyForDay: No path found from main door to tile at " + tilesToCheckPathfind[i] + "! Please fix your grid!");
                return;
            }
        }
        if (isnight == true && isDay == false)
        {
            setSpawnPoint();
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
        time_to_explore = 0f;
    }
    private void setDay()
    {
        isnight = false;
        isDay = true;
        UnselectBlock();
        PlayerUI.SetActive(false);
        setSpawnPoint();
        SpawnAdventurersForWave(adventurercountPerWave[wavecount]);
        areExplorersGone = false;
        
    }
    public void incrementadventurercount(float value)
    {
        adventurercount += value;
    }
    public void setSelectModeTrue(bool value)
    {
        select_mode = value;
    }
    private void SpawnAdventurersForWave(int adventurerCountThisWave)
    {
        for (int i = 0; i < adventurerCountThisWave; i++)
        {
            
            Vector3 spawnPosition = spawnPoint;
            
            
            StartCoroutine(SpawnAdventurerCoroutine(spawnPosition, Random.Range(0, 3), i * 1.0f));
        }
    }
    public bool TimeToExplore()
    {
        if (time_to_explore >= setTimeToExplore)
        {
            return true;
        }
        return false;
    }

    IEnumerator SpawnAdventurerCoroutine(Vector3 spawnPosition, int adventurerType, float delay)
    {
        yield return new WaitForSeconds(delay);
        AdventurerManager.Instance.SpawnAdventurer(spawnPosition, adventurerType);
    }
    private void setSpawnPoint()
    {
        for (int x = 0; x < UF.getGridWidth() * UF.amountOfRooms[0]; x++)
        {
            for (int y = 0; y < UF.getGridHeight() * UF.amountOfRooms[1]; y++)
            {
                int tileValue = grid.GetGridObject(x, y).GetComponent<SpriteChanger>().GetCurrentSpriteIndex();
                if (tileValue == 2)
                {
                    spawnPoint = UF.GridToWorldCoords(new Vector2(x, y));
                    spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, -1);
                    return;
                }
            }
        }
    }
    public Vector3 getSpawnPoint() 
    {
        return spawnPoint;
    }
}