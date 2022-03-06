using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPos;
    public int gCost;
    public int hCost;
    public int xCellIndex;
    public int yCellIndex;
    public Node parent;

    public Node(bool isWalkable, Vector3 worldPos, int xCellIndex, int yCellIndex)
    {
        this.isWalkable = isWalkable;
        this.worldPos = worldPos;
        this.xCellIndex = xCellIndex;
        this.yCellIndex = yCellIndex;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
