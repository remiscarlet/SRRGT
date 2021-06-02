using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    private Transform hitResultsHierarchy;

    public int Score { private set; get; }

    public float Accuracy {
        get {
            int totalNotes = perfects + goods + misses;
            if (totalNotes == 0) {
                return 0.0f;
            }

            float accScore = perfects * 1.0f + goods * 0.5f;
            return accScore / totalNotes * 100;
        }
    }

    private int perfects;
    private int goods;
    private int misses;

    // Start is called before the first frame update
    void Start() {
        hitResultsHierarchy = ReferenceManager.HitResultsHierarchyTransform;

        Score = 0;
        perfects = 0;
        goods = 0;
        misses = 0;
    }

    public void RegisterMiss(Transform noteTransform) {
        Score += 25;
        misses += 1;
        RegisterNote(noteTransform, ReferenceManager.Prefabs.AccuracyMissText);
    }

    public void RegisterGood(Transform noteTransform) {
        Score += 50;
        goods += 1;
        RegisterNote(noteTransform, ReferenceManager.Prefabs.AccuracyGoodText);
    }

    public void RegisterPerfect(Transform noteTransform) {
        Score += 150;
        perfects += 1;
        RegisterNote(noteTransform, ReferenceManager.Prefabs.AccuracyPerfectText);
    }

    private void RegisterNote(Transform noteTransform, GameObject textPrefab) {
        ReferenceManager.UIManager.UpdateStatsUI();
        SpawnAccuracyText(noteTransform, textPrefab);
    }

    private Quaternion fwdRotation = Quaternion.Euler(Vector3.zero);
    private void SpawnAccuracyText(Transform noteTransform, GameObject textPrefab) {
        Vector3 spawnPosition = new Vector3(noteTransform.position.x, noteTransform.position.y, 0);
        Instantiate(textPrefab, spawnPosition, fwdRotation, hitResultsHierarchy);
    }
}
