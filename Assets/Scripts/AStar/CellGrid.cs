using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    public Transform player;
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;
    [SerializeField] LayerMask unwalkableMask;

    Node[,] _grid;
    float _nodeDiameter;
    int _xCellNum;
    int _yCellNum;

    void Start()
    {
        _nodeDiameter = nodeRadius * 2;
        _xCellNum = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _yCellNum = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

        CreateGrid();
    }

    void Update()
    {
    }

    void CreateGrid()
    {
        _grid = new Node[_xCellNum, _yCellNum];
        Vector2 worldBottomLeft = transform.position - Vector3.up * gridWorldSize.y / 2 - Vector3.right * gridWorldSize.x / 2;

        for (int i = 0; i < _xCellNum; i++)
        {
            for (int j = 0; j < _yCellNum; j++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (i * _nodeDiameter + nodeRadius) + Vector2.up * (j * _nodeDiameter + nodeRadius);
                bool isWalkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);

                _grid[i, j] = new Node(isWalkable, worldPoint, i, j);
            }
        }
    }

    public Node GetNodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_xCellNum - 1) * percentX);
        int y = Mathf.RoundToInt((_yCellNum - 1) * percentY);

        return _grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Iterate all 8 neighbours
        // TODO: For our game, remember to ignore corner
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int checkX = node.xCellIndex + i;
                int checkY = node.yCellIndex + j;

                if (checkX < 0 || checkX >= _xCellNum || checkY < 0 || checkY >= _yCellNum)
                {
                    continue;
                }

                neighbours.Add(_grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        if (_grid != null)
        {
            foreach (Node node in _grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                Gizmos.DrawCube(node.worldPos, Vector3.one * (_nodeDiameter - .1f));
            }
        }
    }
}
