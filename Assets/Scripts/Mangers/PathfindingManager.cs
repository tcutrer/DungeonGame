using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }
    private Pathfinding pathfinding;
    private UtilityFunctions UF;

    private void Awake()
    {
        // Singleton pattern: destroy duplicate instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene loads
    }

    private void Start()
    {
        UF = new UtilityFunctions();
        pathfinding = new Pathfinding(UF.getGridWidth(), UF.getGridHeight());
    }

    public Pathfinding GetPathfinding()
    {
        return pathfinding;
    }
}
