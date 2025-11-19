using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    private int gCost;
    private int hCost;
    private int fCost;
    private bool isWalkable;
    private PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public int CalculateFCost()
    {
        return fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }
    public int GetGCost()
    {
        return gCost;
    }
    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }
    public int GetHCost()
    {
        return hCost;
    }
    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }
    public bool GetIsWalkable()
    {
        return isWalkable;
    }
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
    public PathNode GetCameFromNode()
    {
        return cameFromNode;
    }
    public void SetCameFromNode(PathNode cameFromNode)
    {
        this.cameFromNode = cameFromNode;
    }
    
}
