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
        // transform.position = new Vector3(position.x, position.y, -2);
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
            adventurer.transform.position = spawnPosition;
        }
        return adventurer;
    }

    // Update is called once per frame
    void Update()
    {
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
        Vector2 desiredPosition = FindDesiredPosition();
        Move(desiredPosition);
    }

    private void Move(Vector2 newPosition)
    {
        // Get current position as grid coordinates
        int startX = Mathf.RoundToInt(transform.position.x / 10f);
        int startY = Mathf.RoundToInt(transform.position.y / 10f);
        
        // Convert newPosition to grid coordinates
        int endX = Mathf.RoundToInt(newPosition.x / 10f);
        int endY = Mathf.RoundToInt(newPosition.y / 10f);
        
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
            Vector3 targetPosition = new Vector3(path[pathIndex].GetX() * 10f, path[pathIndex].GetY() * 10f, -1f);
            currentDestination = targetPosition;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
            pathIndex++;
        }
    }

    private Vector2 FindDesiredPosition()
    {
        int[,] tileValues = Game_Manger.instance.tileValues;
        // Placeholder for pathfinding logic to determine desired position
        // finalDestination = new Vector2(5, 5);
        return finalDestination;
    }

    
}
