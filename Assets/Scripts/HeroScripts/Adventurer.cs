using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    public int health;
    public float speed;
    public int attackPower;
    public Vector2 position;
    private Pathfinding pathfinding;
    private UtilityFunctions UF;
    [SerializeField] private AdventurerData adventurerData;
    public string id { get; private set; }
    private List<Vector3> currentPath;
    private Coroutine movementCoroutine;
    public Vector2 finalDestination { get; private set; }
    public Vector2 currentDestination { get; private set; }
    public const int searchRange = 10;

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
    }

    void SetupAdventurer(AdventurerData data)
    {
        if (data == null)
        {
            Debug.LogWarning("AdventurerData is not assigned!");
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FollowPath();
        }
    }

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
            // Convert grid coordinates back to world position properly
            // Grid origin is at (-100, -70) with cell size of 10
            Vector3 targetPosition = UF.GridToWorldCoords(new Vector3(path[pathIndex].GetX(), path[pathIndex].GetY(), -1));
            targetPosition.z = -1;
            currentDestination = targetPosition;
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                 Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                newPosition.z = -1; // Preserve z = -1 after movement
                transform.position = newPosition;
                yield return null;
            }
            pathIndex++;
        }
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

    
}
