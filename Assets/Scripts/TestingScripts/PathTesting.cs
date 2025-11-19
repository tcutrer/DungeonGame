using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTesting : MonoBehaviour
{
    public Camera mainCamera;
    public Vector3 mouseWorldPosition;
    private Pathfinding pathfinding;
    private List<PathNode> path;
    void Start()
    {
        pathfinding = new Pathfinding(20, 14);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (mainCamera != null) {
                Vector3 mouseScreenPosition = Input.mousePosition;
                mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
                mouseWorldPosition.z = 0f;
            }
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out x, out y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i=0; i<path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * 10f + new Vector3(-95f, -65f), new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * 10f + new Vector3(-95f, -65f), Color.green, 5f
                    );
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (mainCamera != null) {
                Vector3 mouseScreenPosition = Input.mousePosition;
                mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
                mouseWorldPosition.z = 0f;
            }
            int x, y;
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out x, out y);
            PathNode node = pathfinding.GetGrid().GetGridObject(x, y);
            if (node != null)
            {
                node.SetIsWalkable(!node.GetIsWalkable());
            }
        }
    }
}
