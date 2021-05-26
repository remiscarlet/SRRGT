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

        prefabs = GetComponent<PrefabManager>();
        scoreManager = GetComponent<ScoreManager>();

        cameraObject = GameObject.Find("Main Camera");
        notesManagerComponent = gameObject.GetComponent<NotesManager>();
        hitResultsHierarchyTransform = GameObject.Find("HitResults").transform;
        notesHierarchyTransform = GameObject.Find("Notes").transform;

        environmentHierarchyTransform = GameObject.Find("Environment").transform;
        hitPlaneOuterObject = environmentHierarchyTransform.Find("HitPlaneOuter").gameObject;
        hitPlaneInnerObject = environmentHierarchyTransform.Find("HitPlaneInner").gameObject;
        hitPlaneCenterObject = environmentHierarchyTransform.Find("HitPlaneCenter").gameObject;
    }

    [System.NonSerialized] public PrefabManager prefabs;
    [System.NonSerialized] public ScoreManager scoreManager;
    [System.NonSerialized] public GameObject cameraObject;
    [System.NonSerialized] public NotesManager notesManagerComponent;

    [System.NonSerialized] public Transform hitResultsHierarchyTransform;
    [System.NonSerialized] public Transform notesHierarchyTransform;

    [System.NonSerialized] public Transform environmentHierarchyTransform;
    [System.NonSerialized] public GameObject hitPlaneOuterObject;
    [System.NonSerialized] public GameObject hitPlaneInnerObject;
    [System.NonSerialized] public GameObject hitPlaneCenterObject;

    public static PrefabManager Prefabs {
        get => instance.prefabs;
    }

    public static ScoreManager ScoreManager{
        get => instance.scoreManager;
    }

    public static GameObject CameraObject {
        get => instance.cameraObject;
    }

    public static NotesManager NotesManagerComponent {
        get => instance.notesManagerComponent;
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
