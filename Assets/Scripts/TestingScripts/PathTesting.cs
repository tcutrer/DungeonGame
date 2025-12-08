using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTesting : MonoBehaviour
{
    public Vector3 mouseWorld;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private UtilityFunctions UF;
    void Start()
    {
        UF = new UtilityFunctions();
        pathfinding = new Pathfinding(UF.getGridWidth(), UF.getGridHeight());
    }

    private void Update()
    {
        mouseWorld = UF.worldMousePosition();
        if (Input.GetMouseButtonDown(1))
        {
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorld, out x, out y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i=0; i<path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * UF.getCellSize() +
                    new Vector3(-UF.getGridOffset().x + UF.getWhyOffset(), -UF.getGridOffset().y + UF.getWhyOffset()),
                    new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * UF.getCellSize() + new Vector3(-UF.getGridOffset().x + UF.getWhyOffset(), -UF.getGridOffset().y + UF.getWhyOffset()), Color.green, 5f
                    );
                }
            }
        }
    }
}
