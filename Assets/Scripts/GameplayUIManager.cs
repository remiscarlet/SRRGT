using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour {

    private bool uiActive;
    private bool UIActive {
        get { return uiActive; }
        set {
            //loadingPanel.SetActive(value);
            statsPanel.SetActive(value);
            uiActive = value;
        }
    }

    private GameObject loadingPanel;
    private Slider loadingBar;

    private GameObject statsPanel;
    private TextMeshProUGUI scoreValue;
    private TextMeshProUGUI accuracyValue;

    private GameObject pausePanel;
    private Button resumeButton;
    private Button mainMenuButton;

    // Start is called before the first frame update
    void Start() {
        loadingPanel = transform.Find("ProgressBarPanel").gameObject;
        loadingBar = loadingPanel.transform.Find("ProgressBar").GetComponent<Slider>();

        statsPanel = transform.Find("StatsPanel").gameObject;
        scoreValue = statsPanel.transform.Find("ScoreValue").GetComponent<TextMeshProUGUI>();
        accuracyValue = statsPanel.transform.Find("AccuracyValue").GetComponent<TextMeshProUGUI>();

        pausePanel = transform.Find("PausePanel").gameObject;
        resumeButton = pausePanel.transform.Find("Resume").GetComponent<Button>();
        mainMenuButton = pausePanel.transform.Find("MainMenu").GetComponent<Button>();

        resumeButton.onClick.AddListener(ReferenceManager.GameplayManager.TogglePauseState);
        mainMenuButton.onClick.AddListener(ReferenceManager.GameSceneManager.LoadMainMenuScene);
    }

    // Update is called once per frame
    void Update() {
        // TODO: Probably want this to be an explicit method call in the future?
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            if (!UIActive) {
                UIActive = true;
            }

            loadingBar.value = ReferenceManager.AudioManager.GetTrackPercentage();
        } else if (UIActive) {
            UIActive = false;
        }
    }

    public void UpdateStatsUI() {
        // We only need to update these whenever we register a note - not every frame.
        scoreValue.text = ReferenceManager.ScoreManager.Score.ToString();
        accuracyValue.text = $"{ReferenceManager.ScoreManager.Accuracy:F2}%";
    }

    public void TogglePauseScreen() {
        pausePanel.SetActive(!pausePanel.activeSelf);
        Debug.Log($"pausePanel.activeSelf: {pausePanel.activeSelf}");
    }
}
