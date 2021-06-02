using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour {

    private bool uiActive;
    private bool UIActive {
        get { return uiActive; }
        set {
            optionsPanel.SetActive(value);
            uiActive = value;
        }
    }

    private GameObject optionsPanel;
    private Button playButton;
    private Button editorButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start() {
        optionsPanel = transform.Find("OptionsPanel").gameObject;
        playButton = optionsPanel.transform.Find("Play").GetComponent<Button>();
        editorButton = optionsPanel.transform.Find("Editor").GetComponent<Button>();
        quitButton = optionsPanel.transform.Find("Quit").GetComponent<Button>();

        playButton.onClick.AddListener(StartGameplay);
        editorButton.onClick.AddListener(StartEditor);
        quitButton.onClick.AddListener(QuitGame);
        Debug.Log("Set listeners...");

        UIActive = true;
    }

    void StartGameplay() {
        UIActive = false;
        Debug.Log("Attempting to switch to gameplay scene?");
        ReferenceManager.GameSceneManager.LoadGameplayScene();
    }

    void StartEditor() {
        UIActive = false;
        Debug.Log("Starting Editor.");
        throw new Exception("Unimplimented, sike.");
    }

    void QuitGame() {
        UIActive = false;
        Debug.Log("Qutting. Bye!");
        Application.Quit();
    }

    // Update is called once per frame
    void Update() {
    }
}
