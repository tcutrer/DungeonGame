using System;
using System.Collections;
using System.Collections.Generic;
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
        return creature;
    }

    //Sets hometile
    public void SetHomeTile(Vector3 worldPosition)
    {
        UtilityFunctions uf = new UtilityFunctions();
        HomeTile = uf.WorldToGridCoords(worldPosition);
        Debug.Log("Home tile set to: " + HomeTile);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (pathfinding == null)
        {
            pathfinding = PathfindingManager.Instance.GetPathfinding();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FollowPath();
        }
        if (movementCoroutine != null && !IsCoroutineRunning(movementCoroutine) && !foundDestination)
        {
            FollowPath();
        }
        */
    }

    private bool IsCoroutineRunning(Coroutine coroutine)
    {
        return coroutine != null;
    }

    // Destroys Creature
    public void Death(int goldReward)
    {
        Destroy(gameObject);
    }

    public void search()
    {
        
    }
    /*
    /// Change this to have creatures path find to neerest Adventurer
    private void FollowPath()
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
        Vector2 desiredPosition = FindDesiredPosition();
        Move(desiredPosition);
    }

    private void Move(Vector2 newPosition)
    {
        Grid<PathNode> grid = pathfinding.GetGrid();
        if (grid == null)
        {
            Debug.LogError("Grid is null in Move()!");
            return;
        }
        // Get current position as grid coordinates
        grid.GetXY(transform.position, out int startX, out int startY);
        
        // newPosition is already in grid coordinates
        int endX = (int)newPosition.x;
        int endY = (int)newPosition.y;
        
        List<PathNode> path = pathfinding.FindPath(startX, startY, endX, endY);
        if (path == null || path.Count == 0)
            return;
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<PathNode> path)
    {
        int pathIndex = 0;

         while (pathIndex < path.Count)
        {
            // Check if the next tile is occupied
            if (pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY()))
            {
                    while (pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY()))
                    {
                    waitTime++;
                    if (waitTime > 300) // Wait up to 5 seconds (300 frames at 60fps)
                    {
                        Debug.Log("Adventurer " + id + " is stuck and cannot move!");
                        if (pathIndex > 0)
                        {
                            pathfinding.SetTileOccupied(path[pathIndex - 1].GetX(), path[pathIndex - 1].GetY(), false);
                        }

                        yield break;
                    }
                    yield return null;
                    continue; // Skip the rest and check again next frame
                }
            }

            waitTime = 0;
            
            // Clear previous tile as unoccupied
            if (pathIndex > 0)
            {
                pathfinding.SetTileOccupied(path[pathIndex - 1].GetX(), path[pathIndex - 1].GetY(), false);
            }

            // Mark current tile as occupied
            pathfinding.SetTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY(), true);
            
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
        foundDestination = true;
    }

    private Vector2 FindDesiredPosition()
    {   
        // Initialize necessary variables
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
        int curX = (int)curGridPos.x;
        int curY = (int)curGridPos.y;
        List<Vector2> possibleDestinations = new List<Vector2>();
        int[,] tileValues = Game_Manger.instance.tileValues;

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
                    if (tileValues[checkX, checkY] == 2) // Assuming 2 represents a target tile
                    {
                        possibleDestinations.Add(new Vector2(checkX, checkY));
                    }
                }
            }
        }

        // Select a random destination from the possible ones
        if (possibleDestinations.Count != 0)
        {
            finalDestination = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];
        }
        else
        {
            finalDestination = curGridPos; // No valid destination found, stay in place(May need better handling)
        }
        return finalDestination;
    }
    */
    
}