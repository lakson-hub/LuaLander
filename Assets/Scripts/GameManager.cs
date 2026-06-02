using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance { get; private set; }

    private const int MAX_LIVES = 3;

    private static int levelNumber = 1;
    private static int totalScore = 0;
    private static int livesRemaining = MAX_LIVES;

    public static void ResetStaticData() {
        levelNumber = 1;
        totalScore = 0;
        livesRemaining = MAX_LIVES;
    }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler<OnLivesChangedEventArgs> OnLivesChanged;
    public class OnLivesChangedEventArgs : EventArgs {
        public int livesRemaining;
    }
    
    [SerializeField] private List<GameLevel> gameLevelList;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private int score;
    private float time;
    private bool isTimerActive;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    private void GameInput_OnMenuButtonPressed(object sender, EventArgs e) {
        PauseUnpauseGame();
    }

    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventsArgs e) {
        isTimerActive = e.state == Lander.State.Normal;

        if (e.state == Lander.State.Normal) {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();
        }
    }

    private void Update() {
        if (isTimerActive) { 
            time += Time.deltaTime;   
        }
    }

    private void LoadCurrentLevel() {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedGameLevel.GetLanderStartPosition();
        cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.GetCameraStartTargetTransform();
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.GetZoomedOutOrthographicSize());
    }

    private GameLevel GetGameLevel() {
        foreach (GameLevel gameLevel in gameLevelList) {
            if (gameLevel.GetLevelNumber() == levelNumber) {
                return gameLevel;
            }
        }

        return null;
    }
    
    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
        if (e.landingType != Lander.LandingType.Success) {
            LoseLife();
            if (GetRemainingLives() <= 0) {
                GoToGameOver();
                return;
            }
        }

        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e) {
        AddScore(500);
    }

    public void AddScore(int addScoreAmount) {
        score += addScoreAmount;
        Debug.Log(score);
    }

    public int GetScore() {
        return score;
    }

    public float GetTime() {
        return time;
    }

    public int GetTotalScore() {
        return totalScore;
    }

    public void GoToNextLevel() {
        levelNumber++;
        totalScore += score;

        if (GetGameLevel() == null) {
            // No more levels
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        }
        else {
            // We still have more levels
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);   
        }
    }

    public void RetryLevel() {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void GoToGameOver() {
        SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
    }

    public int GetLevelNumber() {
        return levelNumber;
    }

    public void PauseUnpauseGame() {
        if (Time.timeScale == 1f) {
            PauseGame();
        }
        else {
            UnpauseGame();
        }
    }
    
    public void PauseGame() {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }
    
    public void UnpauseGame() {
        Time.timeScale = 1f;
        OnGameUnpaused?.Invoke(this, EventArgs.Empty);
    }

    public int GetRemainingLives() {
        return livesRemaining;
    }

    private void LoseLife() {
        livesRemaining--;
        OnLivesChanged?.Invoke(this, new OnLivesChangedEventArgs {
            livesRemaining = livesRemaining
        });
    }
}
