using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    private Transform hitResultsHierarchy;

    private int score;

    private int perfects;
    private int goods;
    private int misses;

    // Start is called before the first frame update
    void Start() {
        hitResultsHierarchy = ReferenceManager.HitResultsHierarchyTransform;

        score = 0;
        perfects = 0;
        goods = 0;
        misses = 0;
    }

    public void RegisterMiss(Transform noteTransform) {
        score += 25;
        misses += 1;

        SpawnAccuracyText(noteTransform, ReferenceManager.Prefabs.AccuracyMissText);
        Debug.Log("Registered a miss.");
    }

    public void RegisterGood(Transform noteTransform) {
        score += 50;
        goods += 1;
        SpawnAccuracyText(noteTransform, ReferenceManager.Prefabs.AccuracyGoodText);
    }

    public void RegisterPerfect(Transform noteTransform) {
        score += 150;
        perfects += 1;
        SpawnAccuracyText(noteTransform, ReferenceManager.Prefabs.AccuracyPerfectText);
    }

    private Quaternion fwdRotation = Quaternion.Euler(Vector3.zero);
    private void SpawnAccuracyText(Transform noteTransform, GameObject textPrefab) {
        Vector3 spawnPosition = new Vector3(noteTransform.position.x, noteTransform.position.y, 0);
        Instantiate(textPrefab, spawnPosition, fwdRotation, hitResultsHierarchy);
    }
}
