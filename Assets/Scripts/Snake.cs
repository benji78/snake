using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class Snake : MonoBehaviour
    {
        private Vector2Int _direction = Vector2Int.right;
        private Vector2Int _nextDirection = Vector2Int.right;
        public List<Transform> segments = new List<Transform>();
        public Transform segmentPrefab;
        public SnakePathfinding snakePathfinding;
        private GameManager _gameManager;

        // Start is called before the first frame update
        private void Start()
        {
            _gameManager = GameManager.Instance;
            // if (_gameManager == null)
            // {
            //     Debug.LogError("GameManager not found!"); return;
            // }

            SetState();
        }

        // Update is called once per frame
        private void Update()
        {
            // set direction and prevent instant u-turn suicide
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _direction != Vector2Int.down)
            {
                _nextDirection = Vector2Int.up;
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && _direction != Vector2Int.up)
            {
                _nextDirection = Vector2Int.down;
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && _direction != Vector2Int.right)
            {
                _nextDirection = Vector2Int.left;
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && _direction != Vector2Int.left)
            {
                _nextDirection = Vector2Int.right;
            }
        }

        private void FixedUpdate()
        {
            if (_gameManager.isPathfinding) Pathfind();

            Move();

            // to make sure the snake won't u-turn when the user preses multiple keys between each FixedUpdate
            _direction = _nextDirection;
        }

        private void Pathfind()
        {
            _nextDirection = snakePathfinding.AStar() ?? _nextDirection;
            // Vector2Int? direction = snakePathfinding.AStar();
            // if (direction == null)
            // {
            //     Time.timeScale = 0;
            //     return;
            // }
            // else
            // {
            //     _nextDirection = (Vector2)direction;
            // }

        }

        private void Move()
        {
            // move segments forward from tail to head
            for (int i = segments.Count - 1; i > 0; i--)
            {
                segments[i].position = segments[i - 1].position;
            }

            // move head
            this.transform.position = new Vector3(
                Mathf.Round(this.transform.position.x) + _nextDirection.x,
                Mathf.Round(this.transform.position.y) + _nextDirection.y,
                0f
            );

            _gameManager.UpdateGridSnake(_gameManager.WorldToGrid(this.transform.position), segments);
        }

        private void Grow()
        {
            Transform segment = Instantiate(this.segmentPrefab);

            segment.position = segments[segments.Count - 1].position;

            segments.Add(segment);
        }

        private void SetState()
        {
            // add head
            segments.Add(this.transform);
            this.transform.position = Vector3.zero;

            for (int i = 1; i < _gameManager.intialSnakeSize; i++)
            {
                segments.Add(Instantiate(this.segmentPrefab));
            }
        }
        private void ResetState()
        {
            // destroy every segment except the first (head)
            for (int i = 1; i < segments.Count; i++)
            {
                Destroy(segments[i].gameObject);
            }
            segments.Clear();

            SetState();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Food")
            {
                Grow();
            }
            else if (other.tag == "Obstacle")
            {
                for (int i = 1; i < segments.Count; i++)
                {
                    Destroy(segments[i].gameObject);
                }
                segments.Clear();

                ResetState();
                _gameManager.isPaused = true;
            }
        }
    }
}