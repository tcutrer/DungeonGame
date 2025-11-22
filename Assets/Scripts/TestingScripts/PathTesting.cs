using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTesting : MonoBehaviour
{
    public Camera mainCamera;
    public Vector3 mouseWorld;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private UtilityFunctions utilityFunctions;
    void Start()
    {
        utilityFunctions = new UtilityFunctions();
        pathfinding = new Pathfinding(utilityFunctions.getGridWidth(), utilityFunctions.getGridHeight());
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        mouseWorld = utilityFunctions.worldMousePosition();
        if (Input.GetMouseButtonDown(1))
        {
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorld, out x, out y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i=0; i<path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * utilityFunctions.getCellSize() +
                    new Vector3(-utilityFunctions.getGridOffset().x + utilityFunctions.getWhyOffset(), -utilityFunctions.getGridOffset().y + utilityFunctions.getWhyOffset()),
                    new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * utilityFunctions.getCellSize() + new Vector3(-utilityFunctions.getGridOffset().x + utilityFunctions.getWhyOffset(), -utilityFunctions.getGridOffset().y + utilityFunctions.getWhyOffset()), Color.green, 5f
                    );
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorld, out x, out y);
            PathNode node = pathfinding.GetGrid().GetGridObject(x, y);
            if (node != null)
            {
                node.SetIsWalkable(!node.GetIsWalkable());
            }
        }
    }
}
