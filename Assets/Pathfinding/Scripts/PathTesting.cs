using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTesting : MonoBehaviour
{
    private Pathfinding pathfinding;
    void Start()
    {
        Pathfinding pathfinding = new Pathfinding(20, 14);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                foreach (PathNode pathNode in path)
                {
                    Debug.DrawLine(
                        new Vector3(pathNode.x, pathNode.y) * 10f + new Vector3(5, 5),
                        new Vector3(pathNode.x, pathNode.y) * 10f + new Vector3(5, 5) + Vector3.up * 5f,
                        Color.green,
                        5f
                    );
                }
            }
        }
    }
}
