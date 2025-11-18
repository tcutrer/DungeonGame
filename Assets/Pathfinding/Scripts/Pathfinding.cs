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
    private BSTNode bstRoot;
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
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * 10f + new Vector3(-95f, -65f) );
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
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        bstRoot = null;
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        bstRoot = InsertBST(bstRoot, startNode);

        while (bstRoot != null)
        {
            PathNode currentNode = GetLowestFCostNode(); // Get min from BST
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            bstRoot = DeleteBST(bstRoot, currentNode); // Remove from BST
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) { continue; }
                if (!neighborNode.isWalkable) { 
                    closedList.Add(neighborNode);
                    continue; 
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    // Update in BST
                    bstRoot = DeleteBST(bstRoot, neighborNode);
                    bstRoot = InsertBST(bstRoot, neighborNode);
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

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
            // Left Down
            // if (currentNode.y - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y - 1));
            // // Left Up
            // if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
            // Right Down
            // if (currentNode.y - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y - 1));
            // // Right Up
            // if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));

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
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    private PathNode GetLowestFCostNode()
    {
        /*
        * Gets the node with the lowest F cost from a list
        * Parameters:
        *      pathNodeList: list of PathNodes
        * Returns: PathNode with lowest F cost
        */
        BSTNode current = bstRoot;
        while (current.left != null)
        {
            current = current.left;
        }
        return current.pathNode;;
    }

    private BSTNode InsertBST(BSTNode node, PathNode pathNode)
    {
        if (node == null)
        {
            return new BSTNode(pathNode);
        }

        if (pathNode.fCost < node.pathNode.fCost)
        {
            node.left = InsertBST(node.left, pathNode);
        }
        else
        {
            node.right = InsertBST(node.right, pathNode);
        }

        return node;
    }

    private BSTNode DeleteBST(BSTNode node, PathNode pathNode)
    {
        if (node == null) return null;

        if (pathNode.fCost < node.pathNode.fCost)
        {
            node.left = DeleteBST(node.left, pathNode);
        }
        else if (pathNode.fCost > node.pathNode.fCost)
        {
            node.right = DeleteBST(node.right, pathNode);
        }
        else if (node.pathNode == pathNode)
        {
            if (node.left == null) return node.right;
            if (node.right == null) return node.left;

            BSTNode minRight = node.right;
            while (minRight.left != null)
            {
                minRight = minRight.left;
            }
            node.pathNode = minRight.pathNode;
            node.right = DeleteBST(node.right, minRight.pathNode);
        }

        return node;
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
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
}

public class BSTNode
{
    public PathNode pathNode;
    public BSTNode left;
    public BSTNode right;

    public BSTNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
        this.left = null;
        this.right = null;
    }
}
