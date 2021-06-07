using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

/// <summary>
/// This is a singleton class that houses references to a bunch of commonly used
/// components, gameobjects, etc etc.
///
/// Calls to GetComponent() or otherwise searching for the right GameObject to call it on can be expensive.
/// Use this static class to have a quick and cheap way to reference common objects.
/// </summary>
public class ReferenceManager : MonoBehaviour {
    static ReferenceManager instance;

    void Awake() {
        instance = this;

        bool isValidName = false;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == GameSceneManager.MainMenuScene) {
            mainMenuManager = GetComponent<MainMenuManager>();
            gameSceneManager = GetComponent<GameSceneManager>();
            mainMenuUiManager = GameObject.Find("Canvas").GetComponent<MainMenuUIManager>();
            isValidName = true;
        } else if (sceneName == GameSceneManager.EditorScene ||
                   sceneName == GameSceneManager.GameplayScene) {
            hitResultsHierarchyTransform = GameObject.Find("HitResults").transform;
            notesHierarchyTransform = GameObject.Find("Notes").transform;
            beatLinesHierarchyTransform = GameObject.Find("BeatLines").transform;
            environmentHierarchyTransform = GameObject.Find("Environment").transform;

            cameraObject = GameObject.Find("Main Camera");
            hitPlaneOuterObject = environmentHierarchyTransform.Find("HitPlaneOuter").gameObject;
            hitPlaneInnerObject = environmentHierarchyTransform.Find("HitPlaneInner").gameObject;
            hitPlaneCenterObject = environmentHierarchyTransform.Find("HitPlaneCenter").gameObject;

            prefabs = GetComponent<PrefabManager>();
            Debug.Log($">>> prefabs: {prefabs} - {prefabs.NoteBoundaryLine}");
            notesManager = gameObject.GetComponent<NotesManager>();
            scoreManager = GetComponent<ScoreManager>();
            canvas = GameObject.Find("Canvas");
            gameplayUiManager = canvas.GetComponent<GameplayUIManager>();
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            boardManager = environmentHierarchyTransform.Find("Board").gameObject.GetComponent<BoardManager>();

            gameplayManager = GetComponent<GameplayManager>();
            gameSceneManager = GetComponent<GameSceneManager>();
            isValidName = true;
        }

        if (sceneName == GameSceneManager.EditorScene) {
            editorManager = GetComponent<EditorManager>();
            editorUiManager = canvas.GetComponent<EditorUIManager>();
        }

        if (!isValidName) {
            throw new Exception($"Got invalid scene name! Got: {sceneName}");
        }

    }

    [System.NonSerialized] public Transform hitResultsHierarchyTransform;
    [System.NonSerialized] public Transform notesHierarchyTransform;
    [System.NonSerialized] public Transform beatLinesHierarchyTransform;
    [System.NonSerialized] public Transform environmentHierarchyTransform;

    [System.NonSerialized] public GameObject cameraObject;
    [System.NonSerialized] public GameObject hitPlaneOuterObject;
    [System.NonSerialized] public GameObject hitPlaneInnerObject;
    [System.NonSerialized] public GameObject hitPlaneCenterObject;
    [System.NonSerialized] public GameObject canvas;

    [System.NonSerialized] public GameplayManager gameplayManager;
    [System.NonSerialized] public EditorManager editorManager;
    [System.NonSerialized] public EditorUIManager editorUiManager;
    [System.NonSerialized] public MainMenuManager mainMenuManager;
    [System.NonSerialized] public GameSceneManager gameSceneManager;
    [System.NonSerialized] public PrefabManager prefabs;
    [System.NonSerialized] public NotesManager notesManager;
    [System.NonSerialized] public ScoreManager scoreManager;
    [System.NonSerialized] public GameplayUIManager gameplayUiManager;
    [System.NonSerialized] public MainMenuUIManager mainMenuUiManager;
    [System.NonSerialized] public AudioManager audioManager;
    [System.NonSerialized] public BoardManager boardManager;

    /// <summary>
    /// Transforms
    /// </summary>
    public static Transform HitResultsHierarchyTransform {
        get => instance.hitResultsHierarchyTransform;
    }
    public static Transform NotesHierarchyTransform {
        get => instance.notesHierarchyTransform;
    }
    public static Transform BeatLinesHierarchyTransform {
        get => instance.beatLinesHierarchyTransform;
    }
    public static Transform EnvironmentHierarchyTransform {
        get => instance.environmentHierarchyTransform;
    }

    /// <summary>
    /// GameObjects
    /// </summary>
    public static GameObject CameraObject {
        get => instance.cameraObject;
    }
    public static GameObject HitPlaneOuterObject {
        get => instance.hitPlaneOuterObject;
    }
    public static GameObject HitPlaneInnerObject {
        get => instance.hitPlaneInnerObject;
    }
    public static GameObject HitPlaneCenterObject {
        get => instance.hitPlaneCenterObject;
    }

    /// <summary>
    /// Managers
    /// </summary>
    public static GameplayManager GameplayManager {
        get => instance.gameplayManager;
    }
    public static EditorManager EditorManager {
        get => instance.editorManager;
    }
    public static EditorUIManager EditorUIManager {
        get => instance.editorUiManager;
    }
    public static MainMenuManager MainMenuManager{
        get => instance.mainMenuManager;
    }
    public static GameSceneManager GameSceneManager {
        get => instance.gameSceneManager;
    }
    public static PrefabManager Prefabs {
        get => instance.prefabs;
    }
    public static NotesManager NotesManager {
        get => instance.notesManager;
    }
    public static ScoreManager ScoreManager {
        get => instance.scoreManager;
    }
    public static GameplayUIManager GameplayUIManager {
        get => instance.gameplayUiManager;
    }
    public static MainMenuUIManager MainMenuUIManager {
        get => instance.mainMenuUiManager;
    }
    public static AudioManager AudioManager {
        get => instance.audioManager;
    }
    public static BoardManager BoardManager {
        get => instance.boardManager;
    }
}
