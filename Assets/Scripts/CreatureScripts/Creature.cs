using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int health;
    public int speed;
    public int attackPower;
    public Vector2 position;
    private Pathfinding pathfinding;
    private UtilityFunctions UF;
    [SerializeField] private CreatureData creatureData;
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
        SetupCreature(creatureData);
        pathfinding = new Pathfinding(UF.getGridWidth(), UF.getGridHeight());
    }

    void SetupCreature(CreatureData data)
    {
        health = data.health;
        speed = data.speed*10;
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