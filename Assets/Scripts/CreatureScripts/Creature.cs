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
        SetupCreature(creatureData);
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
        
        if (health <= 0)
        {
            Death(); 
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

}