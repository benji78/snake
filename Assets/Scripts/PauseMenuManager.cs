using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SnakeGame
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameObject _pausePanel;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _gameScoreText;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private Toggle _pathfindingToggle;

        private void Start()
        {
            _speedSlider.minValue = GameManager.minSpeed;
            _speedSlider.maxValue = GameManager.maxSpeed;
            UpdateGameSpeed();

            // Set up button listeners
            _resumeButton.onClick.AddListener(TogglePauseMenu);
            _restartButton.onClick.AddListener(RestartGame);
            _pathfindingToggle.onValueChanged.AddListener(TogglePathfinding);
            _speedSlider.onValueChanged.AddListener(UpdateMoveInterval);

            // Initialize UI
            _pausePanel.SetActive(_gameManager.isPaused);

            // Set initial toggle state
            _pathfindingToggle.isOn = _gameManager.isPathfinding;
        }

        private void Update()
        {
            // Toggle pause menu on ESC or SpaceBar keypress
            // Toggle needs `Navigation = None` else SpaceBar will activate it again
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                TogglePauseMenu();
            }
            _pausePanel.SetActive(_gameManager.isPaused);
            // Time.timeScale = _gameManager.isPaused ? 0f : 1f;
            Time.fixedDeltaTime = _gameManager.moveInterval;
        }

        private void TogglePauseMenu()
        {
            _gameManager.isPaused = !_gameManager.isPaused;
            _gameScoreText.enabled = !_gameManager.isPaused;
            _pausePanel.SetActive(_gameManager.isPaused);
            // Time.timeScale = _gameManager.isPaused ? 0f : 1f;
        }

        private void TogglePathfinding(bool isOn)
        {
            _gameManager.isPathfinding = isOn;
        }

        public void UpdateScoreText()
        {
            _gameScoreText.text = $"Score: {_gameManager.score}";
            _scoreText.text = $"Score: {_gameManager.score}";
        }

        public void UpdatePathfinding()
        {
            _pathfindingToggle.isOn = _gameManager.isPathfinding;
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            _gameManager.RestartGame();
            _pausePanel.SetActive(false);
        }

        public void UpdateMoveInterval(float speed)
        {
            // moveInt = maxMoveInt - (speed - minSpeed) * moveIntStep
            _gameManager.moveInterval = GameManager.maxMoveInterval - (speed - GameManager.minSpeed) * GameManager.moveIntervalStep;
            UpdateGameSpeed(speed);
        }

        public void UpdateGameSpeed()
        {
            // speed = minSpeed + (maxMoveInt - moveInt) / moveIntStep
            float speed = GameManager.minSpeed + (GameManager.maxMoveInterval - _gameManager.moveInterval) / GameManager.moveIntervalStep;
            UpdateGameSpeed(speed);
        }

        public void UpdateGameSpeed(float speed)
        {
            _speedText.text = $"Speed: {speed}";
            _speedSlider.value = speed;
        }
    }
}