using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private bool gameplayUiActive;
    private bool GameplayUIActive {
        get { return gameplayUiActive; }
        set {
            loadingPanel.SetActive(value);
            statsPanel.SetActive(value);
            gameplayUiActive = value;
        }
    }

    private GameObject loadingPanel;
    private Slider loadingBar;

    private GameObject statsPanel;
    private TextMeshProUGUI scoreValue;
    private TextMeshProUGUI accuracyValue;

    private GameObject pausePanel;
    private Button resumeButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start() {
        loadingPanel = transform.Find("ProgressBarPanel").gameObject;
        loadingBar = loadingPanel.transform.Find("ProgressBar").GetComponent<Slider>();

        statsPanel = transform.Find("StatsPanel").gameObject;
        scoreValue = statsPanel.transform.Find("ScoreValue").GetComponent<TextMeshProUGUI>();
        accuracyValue = statsPanel.transform.Find("AccuracyValue").GetComponent<TextMeshProUGUI>();

        pausePanel = transform.Find("PausePanel").gameObject;
        resumeButton = pausePanel.transform.Find("Resume").GetComponent<Button>();
        quitButton = pausePanel.transform.Find("Quit").GetComponent<Button>();

        resumeButton.onClick.AddListener(ReferenceManager.GameManager.TogglePauseState);
    }

    // Update is called once per frame
    void Update() {
        // TODO: Probably want this to be an explicit method call in the future?
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            if (!GameplayUIActive) {
                GameplayUIActive = true;
            }

            loadingBar.value = ReferenceManager.AudioManager.GetTrackPercentage();
        } else if (GameplayUIActive) {
            GameplayUIActive = false;
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
