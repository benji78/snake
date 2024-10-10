using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallTop;
    public GameObject wallBottom;
    public BoxCollider2D foodBound;
    // needs to be odd for wall to aligh with grid
    public int wallThickness = 1;
    public bool isPaused = false;
    public bool pathfind = true;

    public int gridWidth { get { return _gridWidth; } }
    public int gridHeight { get { return _gridHeight; } }
    private int _gridWidth;
    private int _gridHeight;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
        MoveWallsToCamera();
        if (isPaused) Time.timeScale = 0;
    }

    void Update()
    {
        // (un)pause on spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }
    }

    void FixedUpdate()
    {
        Time.timeScale = isPaused ? 0 : 1;
    }

    void MoveWallsToCamera()
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

        // Move and resize walls to camera view
        wallLeft.transform.position = new Vector2(bottomLeft.x, bottomLeft.y + height / 2);
        wallLeft.transform.localScale = new Vector2(wallThickness, height + wallThickness); // add wallThickness so that they fully overlap
        wallRight.transform.position = new Vector2(topRight.x, bottomLeft.y + height / 2);
        wallRight.transform.localScale = new Vector2(wallThickness, height + wallThickness);
        wallBottom.transform.position = new Vector2(bottomLeft.x + width / 2, bottomLeft.y);
        wallBottom.transform.localScale = new Vector2(width + wallThickness, wallThickness);
        wallTop.transform.position = new Vector2(bottomLeft.x + width / 2, topRight.y);
        wallTop.transform.localScale = new Vector2(width + wallThickness, wallThickness);

        _gridWidth = width - wallThickness;
        _gridHeight = height - wallThickness;

        // Resize food boundary BoxCollider2D (position already at 0,0)
        foodBound.size = new Vector2(width - 1 - wallThickness, height - 1 - wallThickness);
        // remove 1m and wallThickness (half that on either side) so the food don't spawn on the walls
    }
}
