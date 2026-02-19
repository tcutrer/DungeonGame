using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    public int health;
    public float speed;
    public int attackPower;
    public string adventurerName;
    public Vector2 position;
    private Pathfinding pathfinding;
    private UtilityFunctions UF;
    [SerializeField] private AdventurerData adventurerData;
    public string id { get; private set; }
    private List<Vector3> currentPath;
    private Coroutine movementCoroutine;
    public Vector2 finalDestination { get; private set; }
    public Vector2 currentDestination { get; private set; }

    public const int searchRange = 15;
    private int waitTime = 0;
    private bool foundDestination = false;
    private bool isReturningToSpawn = false;
    private int waitTimeForSpawn = 0;
    private int returnAttempts = 0;
    private List<string> possibleNames = new List<string> {"Arin", "Bryn", "Cai", "Dara", "Eryn", "Finn", "Gwen", "Hale", "Ira", "Joss"};

    private Game_Manger gameManager;

    void Awake()
    {
        if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
    }

    void Start()
    {
        UF = new UtilityFunctions();
        SetupAdventurer(adventurerData);
        pathfinding = PathfindingManager.Instance.GetPathfinding();
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);


        if (pathfinding != null)
        {
            pathfinding.SetTileOccupied((int)curGridPos.x, (int)curGridPos.y, true);
        }
        gameManager = Game_Manger.instance;
    }

    void SetupAdventurer(AdventurerData data)
    {
        if (data == null)
        {
            Debug.LogWarning("AdventurerData is not assigned!");
            health = 100;
            speed = 5f * 10;
            attackPower = 10;
            adventurerName = possibleNames[UnityEngine.Random.Range(0, possibleNames.Count)];
        }
        else
        {
            health = data.health;
            speed = data.speed * 10;
            attackPower = data.attackPower;
            adventurerName = data.adventurerName;
        }
        position = new Vector3(-35, -35, -1);
    }

    public static Adventurer CreateAdventurer(GameObject prefab, Vector3 spawnPosition)
    {
        if (prefab == null) {
            Debug.LogError("Prefab is null!");
            return null;
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.SetActive(true);
        Adventurer adventurer = instance.GetComponent<Adventurer>();
        if (adventurer != null) {
            spawnPosition.z = -1;
            adventurer.transform.position = spawnPosition;
        }
        return adventurer;
    }

    // Update is called once per frame
    void Update()
    {
        if (pathfinding == null)
        {
            pathfinding = PathfindingManager.Instance.GetPathfinding();
        }

        if (health <= 0)
        {
            Death(10); // Example gold reward
        }
        if (!IsCoroutineRunning(movementCoroutine))
        {
            makeDecision();
        }
        // if (gameManager.TimeToExplore())
        // {
        //     makeDecision();
        // }
        // if (gameManager.TimeToExplore() == false)
        // {
        //     if (courage < 10) {
        //         // Return to spawn point
        //         Vector3 spawnPoint = gameManager.getSpawnPoint();
        //         Move(spawnPoint);
        //     }

        //}
        
    }

    private bool IsCoroutineRunning(Coroutine coroutine)
    {
        return coroutine != null;
    }

    public void Death(int goldReward)
    {
        CurrencyManager currencyManager = CurrencyManager.Instance;
        if (currencyManager != null)
        {
            currencyManager.AddGold(goldReward);
        }
        AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
        Destroy(gameObject);
    }

    private void FollowPath(bool gotoSpawn = false)
    {
        if (pathfinding == null)
        {
            pathfinding = PathfindingManager.Instance.GetPathfinding();
        }
        if (pathfinding == null)
        {
            Debug.LogError("Pathfinding is null! PathfindingManager may not be initialized.");
            return;
        }
        foundDestination = false;
        if (gotoSpawn)
        {
            Vector3 spawnPointWorld = gameManager.getSpawnPoint();
            Vector2 spawnPointGrid = UF.WorldToGridCoords(spawnPointWorld);
            
            if (isReturningToSpawn)
            {
                Debug.Log("Adventurer " + id + " attempting spawn path - spawn world: " + spawnPointWorld + " -> grid: " + spawnPointGrid);
            }
            Move(spawnPointGrid);
        }
        else
        {
            Vector2 desiredPosition = FindDesiredPosition();
            if (desiredPosition != null)
            {
                Move(desiredPosition);
            }
        }
    }

    private void Move(Vector2 newPosition)
    {
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
        pathfinding.SetTileOccupied((int)curGridPos.x, (int)curGridPos.y, false);
        Grid<PathNode> grid = pathfinding.GetGrid();
        if (grid == null)
        {
            Debug.LogError("Adventurer " + id + ": Grid is null in Move()!");
            return;
        }
        // Get current position as grid coordinates
        Vector2 startGridPos = UF.WorldToGridCoords(transform.position);
        int startX = (int)startGridPos.x;
        int startY = (int)startGridPos.y;
        
        // newPosition is already in grid coordinates

    // Snap to grid to ensure valid coordinates
        int endX = Mathf.RoundToInt(newPosition.x);
        int endY = Mathf.RoundToInt(newPosition.y);

        // Validate coordinates are in bounds
        if (endX < 0 || endX >= grid.GetWidth() || endY < 0 || endY >= grid.GetHeight())
        {
            Debug.LogError("Adventurer " + id + ": Target out of bounds! (" + endX + "," + endY + ") grid is " + grid.GetWidth() + "x" + grid.GetHeight());
            if (isReturningToSpawn)
            {
                Debug.LogError("Adventurer " + id + ": SPAWN POINT IS OUT OF BOUNDS! This is a major problem.");
                // Give up on return
                AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                Destroy(gameObject);
            }
            return;
        }
        
        if (isReturningToSpawn)
        {
            Debug.Log("Adventurer " + id + ": Pathfinding from (" + startX + "," + startY + ") to spawn at (" + endX + "," + endY + ")");
        }
        
        List<PathNode> path = pathfinding.FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            Debug.LogError("Adventurer " + id + ": FindPath returned null from (" + startX + "," + startY + ") to (" + endX + "," + endY + ")");
            if (isReturningToSpawn)
            {
                returnAttempts++;
                Debug.LogError("Adventurer " + id + ": Spawn is unreachable! Attempt " + returnAttempts);
                if (returnAttempts > 5)
                {
                    Debug.LogWarning("Adventurer " + id + ": Giving up, spawn is unreachable after " + returnAttempts + " attempts");
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    Destroy(gameObject);
                }
            }
            return;
        }
        if (path.Count == 0)
        {
            Debug.LogError("Adventurer " + id + ": FindPath returned empty path from (" + startX + "," + startY + ") to (" + endX + "," + endY + ")");
            return;
        }
        
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        if (isReturningToSpawn)
        {
            Debug.Log("Adventurer " + id + ": Starting movement home with path of length " + path.Count);
        }
        movementCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<PathNode> path)
    {
        int pathIndex = 0;

         while (pathIndex < path.Count)
        {
            // Get spawn distance for return logic
            float distanceToSpawn = float.MaxValue;
            if (isReturningToSpawn)
            {
                Vector3 spawnPos = gameManager.getSpawnPoint();
                Vector2 spawnGridPos = UF.WorldToGridCoords(spawnPos);
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                distanceToSpawn = Vector2.Distance(curGridPos, spawnGridPos);
            }
            
            // Check if the next tile is occupied (but ignore blocking if very close to spawn during return)
            bool tileBlocked = pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY());
            
            if (tileBlocked && !(isReturningToSpawn && distanceToSpawn < 3f))
            {
                    int localWaitTime = 0;
                    int maxWait = isReturningToSpawn ? 120 : 300; // Wait 2 seconds for returns, 5 for exploration
                    
                    while (pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY()) && !(isReturningToSpawn && distanceToSpawn < 3f))
                    {
                        localWaitTime++;
                        if (localWaitTime > maxWait)
                        {
                            if (isReturningToSpawn)
                            {
                                Debug.Log("Adventurer " + id + " is stuck returning, recalculating path!");
                            }
                            else
                            {
                                Debug.Log("Adventurer " + id + " is stuck exploring, recalculating path!");
                            }
                            // Recalculate path from current position - preserve the return flag
                            FollowPath(isReturningToSpawn);
                            yield break;
                        }
                        yield return null;
                    }
            }

            waitTime = 0;
            
            // Clear previous tile as unoccupied
            if (pathIndex > 0)
            {
                pathfinding.SetTileOccupied(path[pathIndex - 1].GetX(), path[pathIndex - 1].GetY(), false);
            }

            // Don't mark spawn area tiles as occupied during return to allow overlap
            if (!(isReturningToSpawn && distanceToSpawn < 3f))
            {
                pathfinding.SetTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY(), true);
            }
            
            Vector3 targetPosition = UF.GridToWorldCoords(new Vector3(path[pathIndex].GetX(), path[pathIndex].GetY(), -1));
            targetPosition.z = -1;
            currentDestination = targetPosition;
            
            // Move to target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                newPosition.z = -1;
                transform.position = newPosition;
                yield return null;
            }
        

            pathIndex++;
        }

        // Clear the last tile when path is complete
        foundDestination = true;
        gameManager.DestroyTileAt((int)currentDestination.x, (int)currentDestination.y);
        
        // Destroy the tile at destination
        if (gameManager != null && gameManager.tileValues != null && (position.x >= 0 && position.y >= 0))
        {
            int destX = (int)position.x;
            int destY = (int)position.y;
            
            // Clamp to bounds just in case
            destX = Mathf.Clamp(destX, 0, gameManager.tileValues.GetLength(0) - 1);
            destY = Mathf.Clamp(destY, 0, gameManager.tileValues.GetLength(1) - 1);

            // Destroy tiles that are targets (door tile = 2, orange tile = 3, desired tile = 4)
            gameManager.DestroyTileAt(destX, destY);
        }
        
        movementCoroutine = null;
    }

    private Vector2 FindDesiredPosition()
    {   
        // Initialize necessary variables
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
        int curX = (int)curGridPos.x;
        int curY = (int)curGridPos.y;
        List<Vector2> possibleDestinations = new List<Vector2>();
        
        // Get tileValues from game manager
        if (gameManager == null || gameManager.tileValues == null)
        {
            Debug.LogWarning("Adventurer " + id + ": tileValues is null, cannot find destination");
            finalDestination = curGridPos;
            return finalDestination;
        }
        
        int[,] tileValues = gameManager.tileValues;

        // Search within the defined range for desired tiles
        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                int checkX = x + curX;
                int checkY = y + curY;
                
                // Bounds check before accessing the array
                if (checkX >= 0 && checkX < tileValues.GetLength(0) && 
                    checkY >= 0 && checkY < tileValues.GetLength(1))
                {
                    if ((tileValues[checkX, checkY] == 2 || tileValues[checkX, checkY] == 3 || tileValues[checkX, checkY] == 4) && !pathfinding.IsTileOccupied(checkX, checkY) && !(curX == checkX && curY == checkY)) // Check for door (2), orange (3) or desired (4) tiles
                    {
                        possibleDestinations.Add(new Vector2(checkX, checkY));
                    }
                }
            }
        }

        // Select a random destination from the possible ones
        if (possibleDestinations.Count != 0)
        {
            // Store the exact grid coordinates for tile checking later
            Vector2 selectedTile = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];
            // Add offset of ±1 (the space between tiles) to prevent multiple adventurers from targeting the same tile
            finalDestination = selectedTile;
            foundDestination = false;
            Debug.Log("Adventurer " + id + " found destination at (" + selectedTile.x + "," + selectedTile.y + "), moving to (" + finalDestination.x + "," + finalDestination.y + ")");
        }
        else
        {
            Debug.Log("Adventurer " + id + ": No valid destinations found at (" + curX + "," + curY + "), staying in place");
            finalDestination = curGridPos; // No valid destination found, stay in place
        }
        return finalDestination;
    }

    private void makeDecision()
    {
        // If gameManager is not initialized, get it
        if (gameManager == null)
        {
            gameManager = Game_Manger.instance;
            if (gameManager == null) return;
        }

        // Check if time to explore is over
        bool timeIsUp = gameManager.TimeToExplore();

        if (timeIsUp)
        {
            // TIME IS UP - MUST RETURN TO SPAWN
            if (!isReturningToSpawn)
            {
                // Just started returning
                isReturningToSpawn = true;
                returnAttempts = 0;
                waitTimeForSpawn = 0;
                Debug.Log("Adventurer " + id + " starting return to spawn");
                FollowPath(true); // Go home!
            }
            else
            {
                // Already in return mode - check status
                waitTimeForSpawn++;
                
                // Check if reached spawn
                Vector3 spawnPos = gameManager.getSpawnPoint();
                Vector2 spawnGridPos = UF.WorldToGridCoords(spawnPos);
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                
                float distanceToSpawn = Vector2.Distance(curGridPos, spawnGridPos);
                
                // Check if at spawn tile
                if (distanceToSpawn < 1f)
                {
                    Debug.Log("Adventurer " + id + " reached spawn (" + distanceToSpawn + " units away)");
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    Destroy(gameObject);
                    return;
                }
                
                // If no movement coroutine, try pathfinding again
                if (!IsCoroutineRunning(movementCoroutine))
                {
                    returnAttempts++;
                    Debug.Log("Adventurer " + id + " no active movement, retry attempt #" + returnAttempts + ", distance to spawn: " + distanceToSpawn);
                    FollowPath(true);
                }
                
                // Force destroy if waited way too long (30 seconds) or too many attempts
                if (waitTimeForSpawn > 1800 || returnAttempts > 10)
                {
                    Debug.LogWarning("Adventurer " + id + " giving up return (waited " + waitTimeForSpawn + " frames, attempts " + returnAttempts + "), destroying at distance " + distanceToSpawn);
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    Destroy(gameObject);
                    return;
                }
            }
        }
        else
        {
            // STILL TIME TO EXPLORE
            if (isReturningToSpawn)
            {
                // Was returning but somehow exploration resumed? Reset
                isReturningToSpawn = false;
                returnAttempts = 0;
                waitTimeForSpawn = 0;
            }
            
            // Explore normally - only call FollowPath if not moving
            if (!IsCoroutineRunning(movementCoroutine))
            {
                FollowPath(false);
            }
        }
    }
    
}
