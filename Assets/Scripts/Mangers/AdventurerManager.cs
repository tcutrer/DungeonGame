using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class AdventurerManager : MonoBehaviour
{
    // All the Adventuruer prefabs
    [SerializeField] private GameObject farmerPrefab;
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject magePrefab;

    private Pathfinding pathfinding;
    private Grid<GameObject> grid;
    private List<PathNode> path;
    private UtilityFunctions UF;
    private Game_Manger gameManager;

    public int adventurerCountStillInMaze = 0;
    public int adventurerCountThisWave = 0;

    public static AdventurerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UF = new UtilityFunctions();
        pathfinding = PathfindingManager.Instance.GetPathfinding();
        gameManager = Game_Manger.instance;
    }


    public void incrementadventurercount_inMazeStillUP()
    {
        gameManager.adventurercount += 1f;
    }

    public void decrementadventurercount_inMazeStillUP()
    {
        gameManager.adventurercount -= 1f;
        adventurerCountStillInMaze -= 1;
    }

    public void SpawnAdventurer(Vector3 spawnPosition, int adventurerType)
    {
        
        switch (adventurerType)
        {
            case 0:
                Instantiate(farmerPrefab, spawnPosition, Quaternion.identity);
                break;
            case 1:
                Instantiate(warriorPrefab, spawnPosition, Quaternion.identity);
                break;
            case 2:
                Instantiate(magePrefab, spawnPosition, Quaternion.identity);
                break;
        }
        adventurerCountThisWave += 1;
        adventurerCountStillInMaze += 1;

    }
    private void update(){
        
        if (adventurerCountStillInMaze <= 0) {
            gameManager.areExplorersGone = true;
        }
        else {
            gameManager.areExplorersGone = false;
        }
    }

}