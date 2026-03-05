using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int health;
    public float speed;
    public int attackPower;
    public Vector2 position;
    private Pathfinding pathfinding;
    private UtilityFunctions UF;
    [SerializeField] private CreatureData creatureData;
    public string id { get; private set; }
    private List<Vector3> currentPath;
    private Coroutine movementCoroutine;
    public Vector2 finalDestination { get; private set; }
    public Vector2 currentDestination { get; private set; }
    public const int searchRange = 15;
    private int waitTime = 0;
    public bool foundDestination = false;
    public Vector2 HomeTile { get; private set; }
    private Game_Manger gameManager;
    private bool isReturningToSpawn = false;

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
        SetupCreature(creatureData);
        pathfinding = PathfindingManager.Instance.GetPathfinding();
    }

    // Sets up defalt creature or certen creature given data
    void SetupCreature(CreatureData data)
    {
        if (data == null)
        {
            Debug.LogWarning("CreatureData is not assigned!");
            health = 100;
            speed = 5f * 10;
            attackPower = 10;
        }
        else
        {
            health = data.health;
            speed = data.speed * 10;
            attackPower = data.attackPower;
        }
        
        position = new Vector3(-35, -35, -1);
        HomeTile = UF.GridToWorldCoords(position);
    }

    // Creates a creature 
    public static Creature CreateCreature(GameObject prefab, Vector3 spawnPosition)
    {
        if (prefab == null) {
            Debug.LogError("Prefab is null!");
            return null;
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.SetActive(true);
        Creature creature = instance.GetComponent<Creature>();
        if (creature != null) {
            spawnPosition.z = -1;
            creature.transform.position = spawnPosition;
        }
        creature.SetHomeTile(spawnPosition);
        creature.SetPosition(spawnPosition);
        return creature;

    }

    //Sets hometile
    public void SetHomeTile(Vector3 worldPosition)
    {
        UtilityFunctions UF = new UtilityFunctions();
        HomeTile = UF.GridToWorldCoords(worldPosition);
        Debug.Log("Home tile set to: " + HomeTile);
    }

    public void SetPosition(Vector2 gridPosition)
    {
        UtilityFunctions UF = new UtilityFunctions();
        Vector3 worldPosition = UF.GridToWorldCoords(gridPosition);
        Debug.Log("Home tile set to: " + HomeTile);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (health <= 0)
        {
            Death(); 
        }
        if (pathfinding == null)
        {
            pathfinding = PathfindingManager.Instance.GetPathfinding();
            if (pathfinding == null)
            {
                Debug.LogError("Creature " + id + ": Pathfinding component not found in scene!");
            }
        }
        gameManager = Game_Manger.instance;
        if (gameManager.isDay == true && !IsCoroutineRunning(movementCoroutine))
        {
            makeDecision();
        }
    }

    public int Attack(int modifier)
    {
        int hitpoints = UnityEngine.Random.Range(0, attackPower);
        hitpoints += modifier;
        Debug.Log(name+" Attack "+hitpoints);
        return hitpoints;
    }
    public void TakeDamage(int hitpoints)
    {
        health -= hitpoints;
        Debug.Log(name+" Hit -"+hitpoints);
    }

    // Destroys Creature
    public void Death()
    {
        Destroy(gameObject);
        gameManager.PlaceBlock(HomeTile);
    }

    public void Distroy()
    {
        Destroy(gameObject);
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
        int endX = (int)newPosition.x;
        int endY = (int)newPosition.y;
        
        // Validate coordinates are in bounds
        if (endX < 0 || endX >= grid.GetWidth() || endY < 0 || endY >= grid.GetHeight())
        {
            Debug.LogError("Adventurer " + id + ": Target out of bounds! (" + endX + "," + endY + ") grid is " + grid.GetWidth() + "x" + grid.GetHeight());
        }
        
        List<PathNode> path = pathfinding.FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            Debug.LogError("Adventurer " + id + ": FindPath returned null from (" + startX + "," + startY + ") to (" + endX + "," + endY + ")");
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
                Vector2 homeTile = HomeTile;
                Vector2 homeGridPos = HomeTile;
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                distanceToSpawn = Vector2.Distance(curGridPos, homeGridPos);
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
        movementCoroutine = null;
    }
     private Vector2 FindDesiredPosition()
    {   
        // Initialize necessary variables
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
        int curX = (int)curGridPos.x;
        int curY = (int)curGridPos.y;
        List<Vector2> possibleDestinations = new List<Vector2>();
    

        // Search within the defined range for desired tiles
        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                int checkX = x + curX;
                int checkY = y + curY;
                

                possibleDestinations.Add(new Vector2(checkX, checkY));
    
            }
        }

        // Select a random destination from the possible ones
        if (possibleDestinations.Count != 0)
        {
            finalDestination = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];
            //finalDestination += new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
            foundDestination = false;
        }
        else
        {
            finalDestination = curGridPos; // No valid destination found, stay in place(May need better handling)
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
                Debug.Log("Adventurer " + id + " starting return to spawn");
                FollowPath(true); // Go home!
            }
            else
            {
                // Already in return mode - check status
                
                // Check if reached spaw                
                Vector2 spawnGridPos = HomeTile;
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
                    FollowPath(true);
                }
                
                // Force destroy if waited way too long (30 seconds) or too many attempts

            }
        }
        else
        {
            // STILL TIME TO EXPLORE
            if (isReturningToSpawn)
            {
                // Was returning but somehow exploration resumed? Reset
                isReturningToSpawn = false;
            }
            
            // Explore normally - only call FollowPath if not moving
            if (!IsCoroutineRunning(movementCoroutine))
            {
                FollowPath(false);
            }
        }
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
            Vector2 spawnPointWorld = UF.GridToWorldCoords(HomeTile); // Assuming HomeTile is set to the grid position of the spawn point
            Vector2 spawnPointGrid = HomeTile; // HomeTile should already be in grid coordinates based on SetHomeTile
            
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

    private bool IsCoroutineRunning(Coroutine coroutine)
    {
        return coroutine != null;
    }
}

