using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class Food : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameManager _gameManager;

        private Dictionary<Vector2Int, Node> _grid;

        private void Start()
        {
            _grid = _gameManager.grid;

            RandomizePosition();
        }

        public bool IsValidFoodPosition(Vector2Int position)
        {
            return _grid.ContainsKey(position) && _grid[position].type == NodeType.Empty;
        }

        public List<Vector2Int> GetEmptyPositions()
        {
            List<Vector2Int> emptyPositions = new List<Vector2Int>();
            foreach (KeyValuePair<Vector2Int, Node> kvpNode in _grid)
            {
                if (kvpNode.Value.type == NodeType.Empty)
                {
                    emptyPositions.Add(kvpNode.Key);
                }
            }
            return emptyPositions;
        }

        private void RandomizePosition()
        {
            List<Vector2Int> emptyPositions = GetEmptyPositions();
            if (emptyPositions.Count == 0)
            {
                Debug.LogWarning("No empty positions available for food!"); return;
            }

            // Pick a random empty position
            int randomIndex = Random.Range(0, emptyPositions.Count);
            Vector2Int newPos = emptyPositions[randomIndex];

            // Update position
            transform.position = _gameManager.GridToWorld(newPos);

            // add to grid
            _gameManager.grid[newPos].type = NodeType.Food;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // when the snake eats the food
            if (other.tag == "Player")
            {
                RandomizePosition();
            }
        }
    }
}