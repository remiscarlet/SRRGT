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
        scoreManager = GetComponent<ScoreManager>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        cameraObject = GameObject.Find("Main Camera");
        notesManager = gameObject.GetComponent<NotesManager>();
        hitResultsHierarchyTransform = GameObject.Find("HitResults").transform;
        notesHierarchyTransform = GameObject.Find("Notes").transform;

        environmentHierarchyTransform = GameObject.Find("Environment").transform;
        hitPlaneOuterObject = environmentHierarchyTransform.Find("HitPlaneOuter").gameObject;
        hitPlaneInnerObject = environmentHierarchyTransform.Find("HitPlaneInner").gameObject;
        hitPlaneCenterObject = environmentHierarchyTransform.Find("HitPlaneCenter").gameObject;
    }

    [System.NonSerialized] public GameController gameController;
    [System.NonSerialized] public PrefabManager prefabs;
    [System.NonSerialized] public ScoreManager scoreManager;
    [System.NonSerialized] public AudioManager audioManager;
    [System.NonSerialized] public UIManager uiManager;
    [System.NonSerialized] public GameObject cameraObject;
    [System.NonSerialized] public NotesManager notesManager;

    [System.NonSerialized] public Transform hitResultsHierarchyTransform;
    [System.NonSerialized] public Transform notesHierarchyTransform;

    [System.NonSerialized] public Transform environmentHierarchyTransform;
    [System.NonSerialized] public GameObject hitPlaneOuterObject;
    [System.NonSerialized] public GameObject hitPlaneInnerObject;
    [System.NonSerialized] public GameObject hitPlaneCenterObject;

    public static GameController GameController {
        get => instance.gameController;
    }
    public static PrefabManager Prefabs {
        get => instance.prefabs;
    }

    public static ScoreManager ScoreManager {
        get => instance.scoreManager;
    }

    public static AudioManager AudioManager {
        get => instance.audioManager;
    }

    public static UIManager UIManager {
        get => instance.uiManager;
    }

    public static GameObject CameraObject {
        get => instance.cameraObject;
    }

    public static NotesManager NotesManager {
        get => instance.notesManager;
    }

    public static Transform HitResultsHierarchyTransform {
        get => instance.hitResultsHierarchyTransform;
    }

    public static Transform NotesHierarchyTransform {
        get => instance.notesHierarchyTransform;
    }

    public static Transform EnvironmentHierarchyTransform {
        get => instance.environmentHierarchyTransform;
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
}
