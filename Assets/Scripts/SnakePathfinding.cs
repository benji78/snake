using System.Collections.Generic;
using UnityEngine;

public class SnakePathfinding : MonoBehaviour
{
    public Transform snakeHead;
    public Transform food;
    public Snake snakeScript;
    public GameMaster gameMaster;

    private Dictionary<Vector2Int, Node> grid;

    void Start()
    {
        InitializeGrid();
        List<Vector2Int> path = FindPathToFood();
        LogPath(path, Vector2Int.RoundToInt(snakeHead.position));
    }

    void FixedUpdate()
    {
        // if (Input.GetKeyDown(KeyCode.Tab)) // For testing
        // {
        // List<Vector2> path = FindPath(snakeHead.position, food.position);
        // if (path != null)
        // {
        //     Debug.Log("Path found!");
        //     // Move snake along path
        // }
        // else
        // {
        //     Debug.Log("No path found!");
        // }
        // }
    }

    void InitializeGrid()
    {
        int halfWidth = Mathf.RoundToInt(gameMaster.gridWidth / 2);
        int halfHeight = Mathf.RoundToInt(gameMaster.gridHeight / 2);
        grid = new Dictionary<Vector2Int, Node>();

        // when the grid has an odd width and/or height it needs an extra row and/or column
        for (int x = -halfWidth; x < halfWidth + isOddInt(gameMaster.gridWidth); x++)
        {
            for (int y = -halfHeight; y < halfHeight + isOddInt(gameMaster.gridHeight); y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                grid[gridPos] = new Node(gridPos);
            }
        }
        Debug.Log($"Grid initialized with size: {gameMaster.gridWidth}x{gameMaster.gridHeight}, centered at (0,0)");
    }

    int isOddInt(int num)
    {
        return num % 2 == 0 ? 0 : 1;
    }

    public Vector2Int? AStar()
    {
        List<Vector2Int> pathToFood = FindPathToFood();
        if (pathToFood == null)
        {
            // Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

            // return directions[Random.Range(0, 3)];
            Debug.LogWarning("No Path found");
            return null;
        }
        if (pathToFood.Count == 0)
        {
            Debug.LogError("ON TOP OF FOOD!");
            return null;
        }
        Vector2Int nextPosition = pathToFood[0];
        Vector2Int direction = nextPosition - Vector2Int.RoundToInt(snakeHead.position);

        // ##########
        Debug.Log($"Snake: {Vector2Int.RoundToInt(snakeHead.position)} ; Direction: {direction}");

        // Update snake's direction based on the path
        if (direction.x != 0)
        {
            return new Vector2Int(direction.x, 0);
        }
        else if (direction.y != 0)
        {
            return new Vector2Int(0, direction.y);
        }
        Debug.LogError("Invalid direction");
        return null;
    }

    List<Vector2Int> FindPathToFood()
    {
        // Debug.Log(snakeHead.position);
        return FindPath(Vector2Int.RoundToInt(snakeHead.position), Vector2Int.RoundToInt(food.position));
    }

    List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        if (!grid.TryGetValue(startPos, out Node startNode) || !grid.TryGetValue(targetPos, out Node targetNode))
        {
            Debug.LogWarning("Start or target position is outside the grid.");
            return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor) || IsObstacle(neighbor.position))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = node.position + dir;
            if (grid.TryGetValue(checkPos, out Node neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        // much more complicated
        // for (int x = -1; x <= 1; x++)
        // {
        //     for (int y = -1; y <= 1; y++)
        //     {
        //         if (x == 0 && y == 0)
        //             continue;

        //         int checkX = Mathf.RoundToInt(node.position.x + x);
        //         int checkY = Mathf.RoundToInt(node.position.y + y);

        //         if (checkX >= 0 && checkX < gameMaster.gridWidth && checkY >= 0 && checkY < gameMaster.gridHeight)
        //         {
        //             neighbors.Add(grid[checkX, checkY]);
        //         }
        //     }
        // }
        return neighbors;
    }

    bool IsObstacle(Vector2Int position)
    {
        // Check if the position is occupied by a snake segment
        foreach (Transform segment in snakeScript.segments)
        {
            if (Vector2Int.RoundToInt(segment.position) == position)
                return true;
        }
        return false;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        Vector2Int dist = nodeA.position - nodeB.position;
        return Mathf.Abs(dist.x) + Mathf.Abs(dist.y);
    }

    List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    // ########## LOG PATH ##########
    public void LogPath(List<Vector2Int> path, Vector2Int start)
    {
        if (path == null || path.Count == 0)
        {
            Debug.Log("Path is null or empty");
            return;
        }

        string pathString = $"Path: ({path[0].x}, {path[0].y}) ";
        string pathDirString = "Path directions: ";

        Vector2Int direction = path[0] - start;
        pathDirString += GetDirectionString(direction);

        for (int i = 1; i < path.Count; i++)
        {
            pathString += $"({path[i].x}, {path[i].y}) ";
            direction = path[i] - path[i - 1];
            pathDirString += GetDirectionString(direction);
        }
        Debug.Log(pathString);
        Debug.Log(pathDirString);
    }

    private string GetDirectionString(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return "Up ";
        if (direction == Vector2Int.down) return "Down ";
        if (direction == Vector2Int.left) return "Left ";
        if (direction == Vector2Int.right) return "Right ";
        return "Unknown";
    }
}

// cell in grid
public class Node
{
    public Vector2Int position;
    public Node parent;
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public Node(Vector2Int pos)
    {
        position = pos;
    }

}
