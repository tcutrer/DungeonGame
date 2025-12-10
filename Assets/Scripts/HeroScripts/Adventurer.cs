using System;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    public int health;
    public int speed;
    public int attackPower;
    public Vector2 position;
    private Pathfinding pathfinding;
    private UtilityFunctions UF;
    [SerializeField] private AdventurerData adventurerData;
    public string id { get; private set; }
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
        pathfinding = new Pathfinding(UF.getGridWidth(), UF.getGridHeight());
    }

    void SetupAdventurer(AdventurerData data)
    {
        health = data.health;
        speed = data.speed;
        attackPower = data.attackPower;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FollowPath()
    {
        Vector2 desiredPosition = FindDesiredPosition();
        Move(desiredPosition);
    }

    private void Move(Vector2 newPosition)
    {
        position = newPosition;
        // Additional movement logic here
    }

    private Vector2 FindDesiredPosition()
    {
        // Placeholder for pathfinding logic to determine desired position
        return new Vector2(5 , 5);
    }
}
