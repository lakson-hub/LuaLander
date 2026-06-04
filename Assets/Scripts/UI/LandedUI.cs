using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class LandedUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private TextMeshProUGUI nextButtonTextMesh;
    [SerializeField] private Button nextButton;

    private Action nextButtonClickAction;
    
    private void Awake() {
        nextButton.onClick.AddListener(() => {
            nextButtonClickAction();
        });
    }

    private void Start() {
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnFellOutOfMap += Lander_OnFellOutOfMap;
        
        Hide();
    }

    private void Lander_OnFellOutOfMap(object sender, EventArgs e) {
        ShowCrashRetryUI(0f, 0f ,0);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
        if (e.landingType == Lander.LandingType.Success) {
            titleTextMesh.text = "SUCCESSFUL LANDING!";
            nextButtonTextMesh.text = "CONTINUE";
            nextButtonClickAction = GameManager.Instance.GoToNextLevel;
            statsTextMesh.text =
                Mathf.Round(e.landingSpeed * 2f) + "\n" +
                Mathf.Round(e.dotVector * 100f) + "\n" +
                "x" + e.scoreMultiplier + "\n" +
                GameManager.Instance.GetScore() + "\n" +
                GameManager.Instance.GetRemainingLives();
            Show();
            return;
        }
        ShowCrashRetryUI(
            Mathf.Round(e.landingSpeed * 2f),
            Mathf.Round(e.dotVector * 100f),
            e.scoreMultiplier);
    }

    private void ShowCrashRetryUI(float landingSpeedDisplay, float dotVectorDisplay, float scoreMultiplier) {
        if (GameManager.Instance.GetRemainingLives() <= 0) {
            return;
        }
        
        titleTextMesh.text = "<color=#ff0000>CRASH!</color>";
        nextButtonTextMesh.text = "RETRY";
        nextButtonClickAction = GameManager.Instance.RetryLevel;

        statsTextMesh.text =
            landingSpeedDisplay + "\n" +
            dotVectorDisplay + "\n" +
            "x" + scoreMultiplier + "\n" +
            "0\n" +
            GameManager.Instance.GetRemainingLives();
        
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
        
        nextButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
