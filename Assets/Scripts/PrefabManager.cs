using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Similar to <c>ReferenceManager</c> but for prefabs.
///
/// This class is used via ReferenceManager.
/// Ie, <c>ReferenceManager.Prefabs.DamageText</c>
/// NOT, <c>PrefabManager.DamageText</c>
/// </summary>
public class PrefabManager : MonoBehaviour {
    public GameObject noteObject;

    public GameObject accuracyMissText;
    public GameObject accuracyGoodText;
    public GameObject accuracyPerfectText;


    public GameObject NoteObject {
        get => noteObject;
    }

    public GameObject AccuracyMissText {
        get => accuracyMissText;
    }
    public GameObject AccuracyGoodText {
        get => accuracyGoodText;
    }
    public GameObject AccuracyPerfectText {
        get => accuracyPerfectText;
    }
}
