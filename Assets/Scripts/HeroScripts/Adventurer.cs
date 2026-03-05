using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    private Grid<GameObject> grid;

    public int hitpoints = 0;

    // FIX: Decision cooldown to stop every-frame retry spam causing clumping
    private float decisionCooldown = 0f;
    private const float DECISION_INTERVAL = 0.5f;

    void Awake()
    {
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        UF = new UtilityFunctions();
        SetupAdventurer(adventurerData);
        pathfinding = PathfindingManager.Instance.GetPathfinding();
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);

        if (pathfinding != null)
            pathfinding.SetTileOccupied((int)curGridPos.x, (int)curGridPos.y, true);

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
        if (prefab == null)
        {
            Debug.LogError("Prefab is null!");
            return null;
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.SetActive(true);
        Adventurer adventurer = instance.GetComponent<Adventurer>();
        if (adventurer != null)
        {
            spawnPosition.z = -1;
            adventurer.transform.position = spawnPosition;
        }
        return adventurer;
    }

    public int Attack(int modifier)
    {
        int hitpoints = UnityEngine.Random.Range(0, attackPower);
        hitpoints += modifier;
        return hitpoints;
    }

    public void TakeDamage(int hitpoints)
    {
        health -= hitpoints;
        Debug.Log(name + " Hit -" + hitpoints);
    }

    void Update()
    {
        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance.GetPathfinding();

        if (health <= 0)
        {
            Death(10);
            return;
        }

        // FIX: Register this adventurer's current grid position every frame so creatures can chase
        if (gameManager != null && UF != null)
        {
            Vector2 curGrid = UF.WorldToGridCoords(transform.position);
            gameManager.UpdateAdventurerPos(id, curGrid);
        }

        // FIX: Apply cooldown so we don't spam makeDecision every frame
        if (!IsCoroutineRunning(movementCoroutine))
        {
            decisionCooldown -= Time.deltaTime;
            if (decisionCooldown <= 0f)
            {
                decisionCooldown = DECISION_INTERVAL;
                makeDecision();
            }
        }
    }

    private bool IsCoroutineRunning(Coroutine coroutine)
    {
        return coroutine != null;
    }

    public void Death(int goldReward)
    {
        CurrencyManager currencyManager = CurrencyManager.Instance;
        if (currencyManager != null)
            currencyManager.AddGold(goldReward);

        // FIX: Unregister position on death so creatures don't chase a ghost
        if (gameManager != null)
            gameManager.RemoveAdventurerPos(id);

        AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
        Destroy(gameObject);
    }

    private void FollowPath(bool gotoSpawn = false)
    {
        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance.GetPathfinding();
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
                Debug.Log("Adventurer " + id + " attempting spawn path - spawn world: " + spawnPointWorld + " -> grid: " + spawnPointGrid);

            Move(spawnPointGrid);
        }
        else
        {
            Vector2 desiredPosition = FindDesiredPosition();
            Move(desiredPosition);
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

        Vector2 startGridPos = UF.WorldToGridCoords(transform.position);
        int startX = (int)startGridPos.x;
        int startY = (int)startGridPos.y;

        int endX = (int)newPosition.x;
        int endY = (int)newPosition.y;

        if (endX < 0 || endX >= grid.GetWidth() || endY < 0 || endY >= grid.GetHeight())
        {
            Debug.LogError("Adventurer " + id + ": Target out of bounds! (" + endX + "," + endY + ") grid is " + grid.GetWidth() + "x" + grid.GetHeight());
            if (isReturningToSpawn)
            {
                Debug.LogError("Adventurer " + id + ": SPAWN POINT IS OUT OF BOUNDS! This is a major problem.");
                AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                gameManager?.RemoveAdventurerPos(id);
                Destroy(gameObject);
            }
            return;
        }

        if (isReturningToSpawn)
            Debug.Log("Adventurer " + id + ": Pathfinding from (" + startX + "," + startY + ") to spawn at (" + endX + "," + endY + ")");

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
                    gameManager?.RemoveAdventurerPos(id);
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
            StopCoroutine(movementCoroutine);

        if (isReturningToSpawn)
            Debug.Log("Adventurer " + id + ": Starting movement home with path of length " + path.Count);

        movementCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<PathNode> path)
    {
        int pathIndex = 0;

        while (pathIndex < path.Count)
        {
            float distanceToSpawn = float.MaxValue;
            if (isReturningToSpawn)
            {
                Vector3 spawnPos = gameManager.getSpawnPoint();
                Vector2 spawnGridPos = UF.WorldToGridCoords(spawnPos);
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                distanceToSpawn = Vector2.Distance(curGridPos, spawnGridPos);
            }

            bool tileBlocked = pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY());

            if (tileBlocked && !(isReturningToSpawn && distanceToSpawn < 3f))
            {
                int localWaitTime = 0;
                int maxWait = isReturningToSpawn ? 120 : 300;

                while (pathfinding.IsTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY()) && !(isReturningToSpawn && distanceToSpawn < 3f))
                {
                    localWaitTime++;
                    if (localWaitTime > maxWait)
                    {
                        Debug.Log("Adventurer " + id + (isReturningToSpawn ? " is stuck returning" : " is stuck exploring") + ", recalculating path!");
                        FollowPath(isReturningToSpawn);
                        yield break;
                    }
                    yield return null;
                }
            }

            waitTime = 0;

            if (pathIndex > 0)
                pathfinding.SetTileOccupied(path[pathIndex - 1].GetX(), path[pathIndex - 1].GetY(), false);

            if (!(isReturningToSpawn && distanceToSpawn < 3f))
                pathfinding.SetTileOccupied(path[pathIndex].GetX(), path[pathIndex].GetY(), true);

            Vector3 targetPosition = UF.GridToWorldCoords(new Vector3(path[pathIndex].GetX(), path[pathIndex].GetY(), -1));
            targetPosition.z = -1;
            currentDestination = targetPosition;

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
        Vector2 finalGridPos = UF.WorldToGridCoords(transform.position);
        int tilevalue = Game_Manger.instance.tileValues[(int)finalGridPos.x, (int)finalGridPos.y];
        int tileindex = Game_Manger.instance.grid.GetGridObject((int)finalGridPos.x, (int)finalGridPos.y).GetComponent<SpriteChanger>().GetCurrentSpriteIndex();
        if (tilevalue == 2 && tileindex == 3)
            gameManager.isGameOver = true;

        movementCoroutine = null;
    }

    private Vector2 FindDesiredPosition()
    {
        Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
        int curX = (int)curGridPos.x;
        int curY = (int)curGridPos.y;
        List<Vector2> possibleDestinations = new List<Vector2>();
        int[,] tileValues = Game_Manger.instance.tileValues;

        // Primary: look for target tiles (value 2)
        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                int checkX = x + curX;
                int checkY = y + curY;

                if (checkX >= 0 && checkX < tileValues.GetLength(0) &&
                    checkY >= 0 && checkY < tileValues.GetLength(1))
                {
                    if (tileValues[checkX, checkY] == 2 &&
                        !pathfinding.IsTileOccupied(checkX, checkY) &&
                        !(curX == checkX && curY == checkY))
                    {
                        possibleDestinations.Add(new Vector2(checkX, checkY));
                    }
                }
            }
        }

        // FIX: Fall back to any walkable tile if no target tiles are in range.
        // This spreads adventurers out instead of clumping them all on the same few tiles.
        if (possibleDestinations.Count == 0)
        {
            for (int x = -searchRange; x <= searchRange; x++)
            {
                for (int y = -searchRange; y <= searchRange; y++)
                {
                    int checkX = x + curX;
                    int checkY = y + curY;

                    if (checkX >= 0 && checkX < tileValues.GetLength(0) &&
                        checkY >= 0 && checkY < tileValues.GetLength(1) &&
                        tileValues[checkX, checkY] != 1 && // not a wall
                        !pathfinding.IsTileOccupied(checkX, checkY) &&
                        !(curX == checkX && curY == checkY))
                    {
                        possibleDestinations.Add(new Vector2(checkX, checkY));
                    }
                }
            }
        }

        if (possibleDestinations.Count != 0)
        {
            finalDestination = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];
            foundDestination = false;
        }
        else
        {
            finalDestination = curGridPos;
        }

        return finalDestination;
    }

    private void makeDecision()
    {
        if (gameManager == null)
        {
            gameManager = Game_Manger.instance;
            if (gameManager == null) return;
        }

        bool timeIsUp = gameManager.TimeToExplore();

        if (timeIsUp)
        {
            if (!isReturningToSpawn)
            {
                isReturningToSpawn = true;
                returnAttempts = 0;
                waitTimeForSpawn = 0;
                Debug.Log("Adventurer " + id + " starting return to spawn");
                FollowPath(true);
            }
            else
            {
                waitTimeForSpawn++;

                Vector3 spawnPos = gameManager.getSpawnPoint();
                Vector2 spawnGridPos = UF.WorldToGridCoords(spawnPos);
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);

                float distanceToSpawn = Vector2.Distance(curGridPos, spawnGridPos);

                if (distanceToSpawn < 1f)
                {
                    Debug.Log("Adventurer " + id + " reached spawn (" + distanceToSpawn + " units away)");
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    gameManager.RemoveAdventurerPos(id);
                    Destroy(gameObject);
                    return;
                }

                if (!IsCoroutineRunning(movementCoroutine))
                {
                    returnAttempts++;
                    Debug.Log("Adventurer " + id + " no active movement, retry attempt #" + returnAttempts + ", distance to spawn: " + distanceToSpawn);
                    FollowPath(true);
                }

                if (waitTimeForSpawn > 1800 || returnAttempts > 10)
                {
                    Debug.LogWarning("Adventurer " + id + " giving up return (waited " + waitTimeForSpawn + " frames, attempts " + returnAttempts + "), destroying at distance " + distanceToSpawn);
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    gameManager.RemoveAdventurerPos(id);
                    Destroy(gameObject);
                    return;
                }
            }
        }
        else
        {
            if (isReturningToSpawn)
            {
                isReturningToSpawn = false;
                returnAttempts = 0;
                waitTimeForSpawn = 0;
            }

            if (!IsCoroutineRunning(movementCoroutine))
                FollowPath(false);
        }
    }
}