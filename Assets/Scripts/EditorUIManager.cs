using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorUIManager : MonoBehaviour {

    private bool editorUIActive;

    private bool EditorUIActive {
        get { return editorUIActive; }
        set {
            Debug.Log($"Setting editorControlPanel.SetActive() to {value}");
            editorControlPanel.SetActive(value);
            editorUIActive = value;
        }
    }

    private GameObject editorControlPanel;
    private Button recordButton;
    private Button playButton;

    // Start is called before the first frame update
    void Start() {
        editorControlPanel = transform.Find("EditorControlsPanel").gameObject;
        recordButton = editorControlPanel.transform.Find("RecordButton").GetComponent<Button>();
        playButton = editorControlPanel.transform.Find("PlayButton").GetComponent<Button>();

        recordButton.onClick.AddListener(ReferenceManager.EditorManager.StartRecording);
        playButton.onClick.AddListener(ReferenceManager.EditorManager.PlayRecording);

        EditorUIActive = true;
    }

    // Update is called once per frame
    void Update() {
    }

    public void ToggleEditorUI() {
        Debug.Log("Toggling editor UI...");
        EditorUIActive = !EditorUIActive;
    }
}
