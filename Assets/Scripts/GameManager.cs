using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public enum NodeType
    {
        Empty,
        Snake,
        Food
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Transform _wallPrefab;
        [SerializeField] private PauseMenuManager _pauseMenuManager;

        [Header("Movement Settings")]
        [Tooltip("Time between movements in seconds")]
        [Range(minMoveInterval, maxMoveInterval)]
        public float moveInterval = .055f; // add [System.NonSerialized] to prevent serialisation

        [Header("Game Settings")]
        public bool isPaused = false;
        public bool isPathfinding = true;
        [Tooltip("Needs to be odd for wall to aligh with grid")]
        public int wallThickness = 1;
        public int intialSnakeSize = 4;

        private Camera _camera;

        public int gridWidth { get; private set; }
        public int gridHeight { get; private set; }
        public int score { get; private set; }
        public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

        public const float moveIntervalStep = .005f;
        public const float minMoveInterval = .01f;
        public const float maxMoveInterval = .1f;
        public const int minSpeed = 1;
        public const int maxSpeed = (int)((maxMoveInterval - minMoveInterval) / moveIntervalStep + minSpeed);

        private void Awake()
        {
            _camera = Camera.main;
            MoveWallsToCamera();
            InitialiseGrid();
            if (isPaused) Time.timeScale = 0;
        }

        void Update()
        {
            // toggle pathfinding on tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isPathfinding = !isPathfinding;
                _pauseMenuManager.UpdatePathfinding();
            }
            // slow down with J
            if (Input.GetKeyDown(KeyCode.J) && moveInterval < maxMoveInterval)
            {
                moveInterval += moveIntervalStep;
                _pauseMenuManager.UpdateGameSpeed();
            }
            // speed up with K
            else if (Input.GetKeyDown(KeyCode.K) && moveInterval > minMoveInterval)
            {
                moveInterval -= moveIntervalStep;
                _pauseMenuManager.UpdateGameSpeed();
            }
        }

        public void RestartGame()
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
        }

        public void IncrementScore()
        {
            score += 1;
            _pauseMenuManager.UpdateScoreText();
        }

        public void ResetScore()
        {
            score = 0;
            _pauseMenuManager.UpdateScoreText();
        }

        private void MoveWallsToCamera()
        {
            // Get camera bounds
            Vector2 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            Vector2 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));

            // Round the positions to the nearest integer
            bottomLeft = new Vector2(Mathf.Round(bottomLeft.x), Mathf.Round(bottomLeft.y));
            topRight = new Vector2(Mathf.Round(topRight.x), Mathf.Round(topRight.y));

            // Calculate view dimensions
            int width = (int)(topRight.x - bottomLeft.x);
            int height = (int)(topRight.y - bottomLeft.y);

            // create move and resize walls to camera view
            Transform wallLeft = Instantiate(_wallPrefab);
            Transform wallRight = Instantiate(_wallPrefab);
            Transform wallBottom = Instantiate(_wallPrefab);
            Transform wallTop = Instantiate(_wallPrefab);
            wallLeft.transform.position = new Vector2(bottomLeft.x, bottomLeft.y + height / 2);
            wallRight.transform.position = new Vector2(topRight.x, bottomLeft.y + height / 2);
            wallBottom.transform.position = new Vector2(bottomLeft.x + width / 2, bottomLeft.y);
            wallTop.transform.position = new Vector2(bottomLeft.x + width / 2, topRight.y);
            // add wallThickness to the length so that the walls fully overlap (half on each side)
            wallLeft.transform.localScale = new Vector2(wallThickness, height + wallThickness);
            wallRight.transform.localScale = new Vector2(wallThickness, height + wallThickness);
            wallBottom.transform.localScale = new Vector2(width + wallThickness, wallThickness);
            wallTop.transform.localScale = new Vector2(width + wallThickness, wallThickness);

            gridWidth = width - wallThickness;
            gridHeight = height - wallThickness;
        }

        private void InitialiseGrid()
        {
            int halfWidth = Mathf.RoundToInt(gridWidth / 2);
            int halfHeight = Mathf.RoundToInt(gridHeight / 2);
            grid.Clear();

            // when the grid has an odd width and/or height it needs an extra row and/or column
            for (int x = -halfWidth; x < halfWidth + (isEven(gridWidth) ? 0 : 1); x++)
            {
                for (int y = -halfHeight; y < halfHeight + (isEven(gridHeight) ? 0 : 1); y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    grid[gridPos] = new Node(gridPos);
                }
            }
            Debug.Log($"Grid initialized with size: {gridWidth}x{gridHeight}, centered at (0,0)");
        }

        public bool isEven(int num)
        {
            return num % 2 == 0;
        }

        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x),
                Mathf.RoundToInt(worldPosition.y)
            );
        }

        public Vector3 GridToWorld(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x, gridPosition.y, 0);
        }

        public void UpdateGridSnake(Vector2Int headPos, List<Transform> segments)
        {
            // Clear previous snake positions
            foreach (var node in grid.Values)
            {
                if (node.type == NodeType.Snake)
                {
                    node.type = NodeType.Empty;
                }
            }

            // Update snake position
            foreach (Transform segment in segments)
            {
                Vector2Int segmentPos = WorldToGrid(segment.position);
                if (grid.ContainsKey(segmentPos))
                {
                    grid[segmentPos].type = NodeType.Snake;
                }
                else if (segment.Equals(segments[0]))
                {

                    Debug.LogWarning("Snake head outside grid (did you crash into a wall?) " + segmentPos);
                }
                else Debug.LogError("Snake segment outside grid ! " + segmentPos);
            }
        }
    }

    // cells of grid
    public class Node
    {
        public Vector2Int position;
        public Node parent;
        public NodeType type;
        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }

        public Node(Vector2Int position, NodeType type = NodeType.Empty)
        {
            this.position = position;
            this.type = type;
        }
    }
}