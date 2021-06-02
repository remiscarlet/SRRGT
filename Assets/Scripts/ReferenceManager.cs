using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        gameController = GetComponent<GameController>();
        prefabs = GetComponent<PrefabManager>();

        hitResultsHierarchyTransform = GameObject.Find("HitResults").transform;
        notesHierarchyTransform = GameObject.Find("Notes").transform;
        environmentHierarchyTransform = GameObject.Find("Environment").transform;

        cameraObject = GameObject.Find("Main Camera");
        hitPlaneOuterObject = environmentHierarchyTransform.Find("HitPlaneOuter").gameObject;
        hitPlaneInnerObject = environmentHierarchyTransform.Find("HitPlaneInner").gameObject;
        hitPlaneCenterObject = environmentHierarchyTransform.Find("HitPlaneCenter").gameObject;

        notesManager = gameObject.GetComponent<NotesManager>();
        scoreManager = GetComponent<ScoreManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        boardManager = environmentHierarchyTransform.Find("Board").gameObject.GetComponent<BoardManager>();
    }

    [System.NonSerialized] public GameController gameController;
    [System.NonSerialized] public PrefabManager prefabs;

    [System.NonSerialized] public Transform hitResultsHierarchyTransform;
    [System.NonSerialized] public Transform notesHierarchyTransform;
    [System.NonSerialized] public Transform environmentHierarchyTransform;

    [System.NonSerialized] public GameObject cameraObject;
    [System.NonSerialized] public GameObject hitPlaneOuterObject;
    [System.NonSerialized] public GameObject hitPlaneInnerObject;
    [System.NonSerialized] public GameObject hitPlaneCenterObject;

    [System.NonSerialized] public NotesManager notesManager;
    [System.NonSerialized] public ScoreManager scoreManager;
    [System.NonSerialized] public UIManager uiManager;
    [System.NonSerialized] public AudioManager audioManager;
    [System.NonSerialized] public BoardManager boardManager;

    /// <summary>
    /// Etc...?
    /// </summary>
    public static GameController GameController {
        get => instance.gameController;
    }
    public static PrefabManager Prefabs {
        get => instance.prefabs;
    }

    /// <summary>
    /// Transforms
    /// </summary>
    public static Transform HitResultsHierarchyTransform {
        get => instance.hitResultsHierarchyTransform;
    }
    public static Transform NotesHierarchyTransform {
        get => instance.notesHierarchyTransform;
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
    public static NotesManager NotesManager {
        get => instance.notesManager;
    }
    public static ScoreManager ScoreManager {
        get => instance.scoreManager;
    }
    public static UIManager UIManager {
        get => instance.uiManager;
    }
    public static AudioManager AudioManager {
        get => instance.audioManager;
    }
    public static BoardManager BoardManager {
        get => instance.boardManager;
    }
}
