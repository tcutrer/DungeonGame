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
        transform.position = new Vector3(position.x, position.y, 0);
    }

    void SetupAdventurer(AdventurerData data)
    {
        health = data.health;
        speed = data.speed;
        attackPower = data.attackPower;
        position = new Vector2(-35, -35);
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
        Vector2 desiredPosition = FindDesiredPosition();
        Move(desiredPosition);
    }

    private void Move(Vector2 newPosition)
    {
        List<Vector3> path = pathfinding.FindPath(transform.position, newPosition);
        if (path == null || path.Count == 0)
            return;
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<Vector3> path)
    {
        int pathIndex = 0;

        while (pathIndex < path.Count)
        {
            Vector3 targetPosition = path[pathIndex];
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
        // Placeholder for pathfinding logic to determine desired position
        return new Vector2(5 , 5);
    }
}
