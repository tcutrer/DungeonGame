using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid;
    private BinarySearchTree openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 10f, new Vector3(-100f , -70f), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y), true);
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        /*
        * Finds a path from start to end using A* algorithm
        * Parameters:
        *      startWorldPosition: starting position in world coords
        *      endWorldPosition: ending position in world coords
        * Returns: list of Vector3 positions representing the path
        */
        int startX, startY;
        int endX, endY;
        grid.GetXY(startWorldPosition, out startX, out startY);
        grid.GetXY(endWorldPosition, out endX, out endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.GetX(), pathNode.GetY()) * 10f + new Vector3(-95f, -65f) );
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        
        /*
        * Finds a path from start to end using A* algorithm
        * Parameters:
        *      startX: starting x in grid coords
        *      startY: starting y in grid coords
        *      endX: ending x in grid coords
        *      endY: ending y in grid coords
        * Returns: list of PathNodes representing the path
        */
        if (grid == null)
        {
            Debug.LogError("Grid is null in Pathfinding.FindPath()!");
            return null;
        }

        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null)
        {
            Debug.LogError("Start node is null in Pathfinding.FindPath()!");
            return null;
        }

        if (endNode == null)
        {
            Debug.LogError("End node is null in Pathfinding.FindPath()!");
            return null;
        }
        
        openList = new BinarySearchTree();
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.SetGCost(int.MaxValue);
                pathNode.CalculateFCost();
                pathNode.SetCameFromNode(null);
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistanceCost(startNode, endNode));
        startNode.CalculateFCost();

        openList.Insert(startNode);

        while (!openList.IsEmpty())
        {
            PathNode currentNode = openList.GetLowestFCostNode();
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Delete(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) { continue; }
                if (!neighborNode.GetIsWalkable()) { 
                    closedList.Add(neighborNode);
                    continue; 
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistanceCost(currentNode, neighborNode);
                if (tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistanceCost(neighborNode, endNode));
                    neighborNode.CalculateFCost();

                    openList.Delete(neighborNode);
                    openList.Insert(neighborNode);
                }
            }
        }

        // Out of nodes on the open list
        return null;
    }

    public Grid<PathNode> GetGrid()
    {
        /*
        * Gets the grid used for pathfinding
        * Returns: Grid of PathNodes
        */
        return grid;
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        /*
        * Gets the list of neighbor nodes for a given node
        * Parameters:
        *      currentNode: current PathNode
        * Returns: list of neighboring PathNodes
        */
        List<PathNode> neighborList = new List<PathNode>();

        if (currentNode.GetX() - 1 >= 0)
        {
            // Left
            neighborList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY()));
            // Left Down
            // if (currentNode.GetY() - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY() - 1));
            // // Left Up
            // if (currentNode.GetY() + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY() + 1));
        }
        if (currentNode.GetX() + 1 < grid.GetWidth())
        {
            // Right
            neighborList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY()));
            // Right Down
            // if (currentNode.GetY() - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY() - 1));
            // // Right Up
            // if (currentNode.GetY() + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY() + 1));
        }
        // Down
        if (currentNode.GetY() - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.GetX(), currentNode.GetY() - 1));
        // Up
        if (currentNode.GetY() + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.GetX(), currentNode.GetY() + 1));

        return neighborList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        /*
        * Calculates the path from start to end node
        * Parameters:
        *      endNode: ending PathNode
        * Returns: list of PathNodes representing the path
        */
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currentNode = endNode;
        while (currentNode.GetCameFromNode() != null)
        {
            path.Add(currentNode.GetCameFromNode());
            currentNode = currentNode.GetCameFromNode();
        }

        path.Reverse();
        return path;
    }
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        /*
        * Calculates the distance cost between two nodes
        * Parameters:
        *      a: first PathNode
        *      b: second PathNode
        * Returns: distance cost as int
        */
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public void SetWalkables(int[,] walkableArray)
    {
        /*
        * Sets the walkability of nodes based on a 2D array
        * Parameters:
        *      walkableArray: 2D int array where 1 = walkable, 0 = not walkable
        */
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                if (walkableArray[x, y] == 1 || walkableArray[x, y] == 2)
                {
                    pathNode.SetIsWalkable(true);
                }
                else
                {
                    pathNode.SetIsWalkable(false);
                }
            }
        }
    }
}