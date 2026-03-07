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
    public const int searchRange = 10;
    private int waitTime = 0;
    public bool foundDestination = false;
    public Vector2 HomeTile { get; private set; }
    private bool homeTileSetExternally = false; // FIX: track if SetHomeTile was called before Start()
    private Game_Manger gameManager;
    private bool isReturningToSpawn = false;

    // FIX: Decision cooldown to stop every-frame retry spam
    private float decisionCooldown = 0f;
    private const float DECISION_INTERVAL = 0.5f;

    private void Awake()
    {
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();

        if (UF == null)
            UF = new UtilityFunctions();

        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();
    }

    void Start()
    {
        SetupCreature(creatureData);

        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();
    }

    void SetupCreature(CreatureData data)
    {
        if (data == null)
        {
            Debug.LogWarning($"Creature {id}: CreatureData is not assigned! Using defaults.");
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

        if (UF == null) UF = new UtilityFunctions();

        // FIX: Only set HomeTile from transform position if SetHomeTile() was NOT already called
        // by the factory/manager before Start() ran. This prevents overwriting the correct value.
        if (!homeTileSetExternally)
        {
            HomeTile = UF.WorldToGridCoords(transform.position);
        }
        position = HomeTile;
    }

    public static Creature CreateCreature(GameObject prefab, Vector3 spawnPosition)
    {
        if (prefab == null)
        {
            Debug.LogError("Creature.CreateCreature: Prefab is null!");
            return null;
        }

        Vector3 spawnPos = spawnPosition;
        spawnPos.z = -0.9f;

        GameObject instance = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
        if (instance == null)
        {
            Debug.LogError("Creature.CreateCreature: Instantiate returned null.");
            return null;
        }

        instance.SetActive(true);
        Creature creature = instance.GetComponent<Creature>();
        if (creature == null)
        {
            Debug.LogError("Creature.CreateCreature: Prefab missing Creature component! Destroying instance.");
            Destroy(instance);
            return null;
        }

        var sr = instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            try { sr.sortingLayerName = "Characters"; } catch { }
            sr.sortingOrder = 50;
        }

        return creature;
    }

    // FIX: Mark that HomeTile was set externally so Start() does not overwrite it
    public void SetHomeTile(Vector3 worldPosition)
    {
        if (UF == null) UF = new UtilityFunctions();
        Vector2 grid = UF.WorldToGridCoords(worldPosition);
        HomeTile = new Vector2(Mathf.RoundToInt(grid.x), Mathf.RoundToInt(grid.y));
        homeTileSetExternally = true;
        Debug.Log($"Creature {id}: SetHomeTile world {worldPosition} -> grid {HomeTile}");
    }

    public void SetPosition(Vector2 gridPosition)
    {
        if (UF == null) UF = new UtilityFunctions();
        Vector3 worldPosition = UF.GridToWorldCoords(gridPosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, -0.9f);
        position = gridPosition;
        Debug.Log($"Creature {id}: SetPosition -> grid {gridPosition} world {transform.position}");
    }

    void Update()
    {
        if (health <= 0)
        {
            Death();
            return;
        }

        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();

        if (gameManager == null)
            gameManager = Game_Manger.instance;

        // shouldThink is true when:
        // 1. It is daytime (normal exploration/chasing)
        // 2. Time has run out (TimeToExplore) — even if isDay just flipped false,
        //    we still need makeDecision to fire ONCE to SET isReturningToSpawn=true.
        //    Without this, isDay=false and isReturningToSpawn=false simultaneously,
        //    so makeDecision never runs and the creature never walks home.
        // 3. Already mid-return (isReturningToSpawn=true) so we can keep retrying.
        bool shouldThink = gameManager != null &&
            (gameManager.isDay || gameManager.TimeToExplore() || isReturningToSpawn);
        if (shouldThink && !IsCoroutineRunning(movementCoroutine))
        {
            decisionCooldown -= Time.deltaTime;
            if (decisionCooldown <= 0f)
            {
                decisionCooldown = DECISION_INTERVAL;
                makeDecision();
            }
        }
    }

    public int Attack(int modifier)
    {
        int hitpoints = UnityEngine.Random.Range(0, attackPower);
        hitpoints += modifier;
        Debug.Log(name + " Attack " + hitpoints);
        return hitpoints;
    }

    public void TakeDamage(int hitpoints)
    {
        health -= hitpoints;
        Debug.Log(name + " Hit -" + hitpoints);
    }

    public void Death()
    {
        if (UF == null) UF = new UtilityFunctions();
        Vector2 curGrid = UF.WorldToGridCoords(transform.position);
        if (pathfinding != null)
            pathfinding.SetTileOccupied((int)curGrid.x, (int)curGrid.y, false);

        // FIX: Remove from adventurer tracking when destroyed
        if (!string.IsNullOrEmpty(id) && gameManager != null)
            gameManager.RemoveAdventurerPos(id);

        Destroy(gameObject);
        if (gameManager != null)
            gameManager.PlaceBlock(HomeTile);
    }

    public void Destroy()
    {
        if (UF == null) UF = new UtilityFunctions();
        Vector2 curGrid = UF.WorldToGridCoords(transform.position);
        if (pathfinding != null)
            pathfinding.SetTileOccupied((int)curGrid.x, (int)curGrid.y, false);

        Destroy(gameObject);
    }

    private void Move(Vector2 newPositionGrid)
    {
        if (UF == null) UF = new UtilityFunctions();
        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();

        Grid<PathNode> grid = pathfinding?.GetGrid();
        if (grid == null)
        {
            Debug.LogError($"Creature {id}: Move(): pathfinding grid is null!");
            return;
        }

        Vector2 startGridPosF = UF.WorldToGridCoords(transform.position);
        int startX = Mathf.RoundToInt(startGridPosF.x);
        int startY = Mathf.RoundToInt(startGridPosF.y);

        int endX = Mathf.RoundToInt(newPositionGrid.x);
        int endY = Mathf.RoundToInt(newPositionGrid.y);

        if (endX < 0 || endX >= grid.GetWidth() || endY < 0 || endY >= grid.GetHeight())
        {
            Debug.LogError($"Creature {id}: Move(): target out of bounds ({endX},{endY}) grid {grid.GetWidth()}x{grid.GetHeight()}");
            return;
        }

        if (startX == endX && startY == endY)
        {
            Debug.Log($"Creature {id}: Move(): already at target ({endX},{endY}).");
            foundDestination = true;
            return;
        }

        List<PathNode> path = pathfinding.FindPath(startX, startY, endX, endY);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"Creature {id}: Move(): FindPath returned null/empty from ({startX},{startY}) to ({endX},{endY}).");
            return;
        }

        pathfinding.SetTileOccupied(startX, startY, false);

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<PathNode> path)
    {
        int pathIndex = 0;
        int lastX = -1, lastY = -1;

        Debug.Log($"Creature {id}: MoveAlongPath started, nodes: {path.Count}");

        while (pathIndex < path.Count)
        {
            int nextX = path[pathIndex].GetX();
            int nextY = path[pathIndex].GetY();

            float distanceToSpawn = float.MaxValue;
            if (isReturningToSpawn)
            {
                Vector2 homeGridPos = HomeTile;
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                distanceToSpawn = Vector2.Distance(curGridPos, homeGridPos);
            }

            bool tileBlocked = pathfinding.IsTileOccupied(nextX, nextY);
            if (tileBlocked && !(isReturningToSpawn && distanceToSpawn < 3f))
            {
                int localWaitTime = 0;
                int maxWait = isReturningToSpawn ? 120 : 300;
                while (pathfinding.IsTileOccupied(nextX, nextY) && !(isReturningToSpawn && distanceToSpawn < 3f))
                {
                    localWaitTime++;
                    if (localWaitTime > maxWait)
                    {
                        Debug.LogWarning($"Creature {id}: MoveAlongPath: stuck waiting on tile ({nextX},{nextY}). Recalculating.");
                        FollowPath(isReturningToSpawn);
                        yield break;
                    }
                    yield return null;
                }
            }

            if (pathIndex > 0)
            {
                int prevX = path[pathIndex - 1].GetX();
                int prevY = path[pathIndex - 1].GetY();
                pathfinding.SetTileOccupied(prevX, prevY, false);
            }

            if (!(isReturningToSpawn && distanceToSpawn < 3f))
                pathfinding.SetTileOccupied(nextX, nextY, true);

            lastX = nextX; lastY = nextY;

            Vector3 targetWorld = UF.GridToWorldCoords(new Vector3(nextX, nextY, -1));
            targetWorld.z = -0.9f;
            currentDestination = targetWorld;

            while (Vector3.Distance(transform.position, targetWorld) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorld, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, transform.position.y, -0.9f);
                yield return null;
            }

            pathIndex++;
        }

        Debug.Log($"Creature {id}: MoveAlongPath finished at grid ({lastX},{lastY}).");

        if (lastX >= 0 && lastY >= 0 && pathfinding != null)
            pathfinding.SetTileOccupied(lastX, lastY, false);

        foundDestination = true;
        movementCoroutine = null;
    }

    private Vector2 FindDesiredPosition()
    {
        if (UF == null) UF = new UtilityFunctions();
        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();

        Grid<PathNode> grid = pathfinding?.GetGrid();
        if (grid == null)
            return UF.WorldToGridCoords(transform.position);

        Vector2 curGridPosF = UF.WorldToGridCoords(transform.position);
        int curX = Mathf.RoundToInt(curGridPosF.x);
        int curY = Mathf.RoundToInt(curGridPosF.y);

        List<Vector2> possibleDestinations = new List<Vector2>();

        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                int checkX = curX + x;
                int checkY = curY + y;
                if (checkX >= 0 && checkX < grid.GetWidth() && checkY >= 0 && checkY < grid.GetHeight())
                {
                // Prevent creature from selecting its spawn tile
                    if (checkX == Mathf.RoundToInt(HomeTile.x) && checkY == Mathf.RoundToInt(HomeTile.y))
                        continue;


                    possibleDestinations.Add(new Vector2(checkX, checkY));
                }
            }
        }

        if (possibleDestinations.Count > 0)
        {
            Vector2 chosen = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];
            if (chosen.x == curX && chosen.y == curY && possibleDestinations.Count > 1)
                chosen = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];

            finalDestination = new Vector2(Mathf.RoundToInt(chosen.x), Mathf.RoundToInt(chosen.y));
            foundDestination = false;
            return finalDestination;
        }
        return new Vector2(curX, curY);
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
                Debug.Log("Creature " + id + " starting return to home tile");
                FollowPath(true);
            }
            else
            {
                // FIX: Compare against HomeTile (grid coords) not world coords
                Vector2 curGridPos = UF.WorldToGridCoords(transform.position);
                float distanceToHome = Vector2.Distance(curGridPos, HomeTile);

                if (distanceToHome < 1f)
                {
                    Debug.Log("Creature " + id + " reached home tile (" + distanceToHome + " units away)");
                    AdventurerManager.Instance.decrementadventurercount_inMazeStillUP();
                    Destroy(gameObject);
                    return;
                }

                if (!IsCoroutineRunning(movementCoroutine))
                {
                    FollowPath(true);
                }
            }
        }
        else
        {
            if (isReturningToSpawn)
                isReturningToSpawn = false;

            // FIX: Chase adventurers if any are registered in the manager
            if (gameManager.AdventurerPos != null && gameManager.AdventurerPos.Count > 0)
            {
                if (!IsCoroutineRunning(movementCoroutine))
                    MoveTowardAdventurer();
                return;
            }

            // Otherwise explore randomly
            if (!IsCoroutineRunning(movementCoroutine))
                FollowPath(false);
        }
    }

    private void FollowPath(bool gotoSpawn = false)
    {
        if (pathfinding == null)
            pathfinding = PathfindingManager.Instance?.GetPathfinding();
        if (pathfinding == null)
        {
            Debug.LogError($"Creature {id}: FollowPath(): pathfinding null.");
            return;
        }

        foundDestination = false;
        if (gotoSpawn)
        {
            // FIX: Use HomeTile directly — it is already in grid coordinates
            Vector2 spawnPointGrid = new Vector2(Mathf.RoundToInt(HomeTile.x), Mathf.RoundToInt(HomeTile.y));
            isReturningToSpawn = true;
            Debug.Log($"Creature {id}: FollowPath(gotoSpawn) -> target grid {spawnPointGrid}");
            Move(spawnPointGrid);
        }
        else
        {
            isReturningToSpawn = false;
            Vector2 desiredPosition = FindDesiredPosition();
            desiredPosition = new Vector2(Mathf.RoundToInt(desiredPosition.x), Mathf.RoundToInt(desiredPosition.y));
            Debug.Log($"Creature {id}: FollowPath(explore) -> target grid {desiredPosition}");
            Move(desiredPosition);
        }
    }

    private bool IsCoroutineRunning(Coroutine coroutine)
    {
        return coroutine != null;
    }

    private Vector2 GetClosestAdventurer()
    {
        // FIX: Use the live AdventurerPos list from Game_Manger (populated each frame by adventurers)
        List<Vector2> adventurers = gameManager?.AdventurerPos;

        if (adventurers == null || adventurers.Count == 0)
            return new Vector2(-1, -1);

        Vector2 currentGrid = UF.WorldToGridCoords(transform.position);

        Vector2 closest = adventurers[0];
        float closestDist = Vector2.Distance(currentGrid, closest);

        for (int i = 1; i < adventurers.Count; i++)
        {
            float dist = Vector2.Distance(currentGrid, adventurers[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = adventurers[i];
            }
        }

        return closest;
    }

    private void MoveTowardAdventurer()
    {
        Vector2 target = GetClosestAdventurer();

        if (target.x == -1 && target.y == -1)
            return;

        Move(target);
    }
}