using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class SnakePathfinding : MonoBehaviour
    {
        public Transform food;
        public Snake snake;

        private Transform _snakeHead;
        private Dictionary<Vector2Int, Node> _grid;
        private GameManager _gameManager;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _grid = _gameManager.grid;
            _snakeHead = snake.transform;

            List<Vector2Int> path = FindPathToFood();
            LogPath(path, Vector2Int.RoundToInt(_snakeHead.position));
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
            Vector2Int direction = nextPosition - Vector2Int.RoundToInt(_snakeHead.position);

            // ########## LOG ##########
            // Debug.Log($"Snake: {Vector2Int.RoundToInt(_snakeHead.position)} ; Direction: {direction}");

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
            // Debug.Log(_snakeHead.position);
            return FindPath(Vector2Int.RoundToInt(_snakeHead.position), Vector2Int.RoundToInt(food.position));
        }

        List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
        {
            if (!_grid.TryGetValue(startPos, out Node startNode) || !_grid.TryGetValue(targetPos, out Node targetNode))
            {
                Debug.LogWarning("Start or target position is outside of the grid.");
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
                    if (closedSet.Contains(neighbor) || neighbor.type == NodeType.Snake)
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
                if (_grid.TryGetValue(checkPos, out Node neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
            return neighbors;
        }

        // bool IsObstacle(Vector2Int position)
        // {
        //     // Check if the position is occupied by a snake segment
        //     foreach (Transform segment in snake.segments)
        //     {
        //         if (Vector2Int.RoundToInt(segment.position) == position)
        //             return true;
        //     }
        //     return false;
        // }

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
                Debug.Log("Path is null or empty"); return;
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
}