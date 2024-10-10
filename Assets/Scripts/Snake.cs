using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _nextDirection = Vector2.right;
    public List<Transform> segments = new List<Transform>();
    public Transform segmentPrefab;
    public int intialSize = 4;
    public GameMaster gameMaster;
    public SnakePathfinding snakePathfinding;

    // Start is called before the first frame update
    private void Start()
    {
        SetState();
    }

    // Update is called once per frame
    private void Update()
    {
        // set direction and prevent instant u-turn suicide
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _direction != Vector2.down)
        {
            _nextDirection = Vector2.up;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && _direction != Vector2.up)
        {
            _nextDirection = Vector2.down;
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && _direction != Vector2.right)
        {
            _nextDirection = Vector2.left;
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && _direction != Vector2.left)
        {
            _nextDirection = Vector2.right;
        }
    }

    private void FixedUpdate()
    {
        if (gameMaster.pathfind) Pathfind();

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

        for (int i = 1; i < this.intialSize; i++)
        {
            segments.Add(Instantiate(this.segmentPrefab));
        }
        this.transform.position = Vector3.zero;
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
            gameMaster.isPaused = true;
        }
    }
}
