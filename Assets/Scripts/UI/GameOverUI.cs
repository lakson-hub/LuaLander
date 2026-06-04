using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;
    [SerializeField] private TextMeshProUGUI gameOverTextMesh;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        if (GameManager.GetGameOverReason() == GameManager.GameOverReason.AllLevelsCompleted) {
            gameOverTextMesh.text = "YOU WON THE GAME!";
        }
        else {
            gameOverTextMesh.text = "GAME OVER!";
        }

        scoreTextMesh.text = "FINAL SCORE: " + GameManager.GetTotalScore();
        
        mainMenuButton.Select();
    }
}
