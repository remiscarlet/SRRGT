using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    protected Transform environmentHierarchyTransform;
    protected GameObject noteBoundaryPrefab;
    protected Quaternion noteBoundaryRot;

    protected List<GameObject> boundaries;

    public const float BoardWidth = 10.0f; // TODO: Tie this to config/class property of something else.
    public const int ChartEventSpawnDistance = 50;

    protected void Start() {
        environmentHierarchyTransform = ReferenceManager.EnvironmentHierarchyTransform;
        Debug.Log($">>>> NoteBoundary: {ReferenceManager.Prefabs.NoteBoundaryLine}");
        noteBoundaryPrefab = ReferenceManager.Prefabs.NoteBoundaryLine;
        noteBoundaryRot = Quaternion.Euler(noteBoundaryPrefab.transform.forward);

        boundaries = new List<GameObject>();
    }

    public void InitializeNoteBoundaries(int numKeys) {
        float rowWidth = BoardWidth / numKeys;
        Vector3 position;
        for (int idx = 0; idx < numKeys + 1; idx++) {
            // Num boundaries = numKeys + 1. For far right boundary.
            position = new Vector3(idx * rowWidth, 0.0f, 0.0f);
            Debug.Log(ReferenceManager.Prefabs);
            Debug.Log(ReferenceManager.Prefabs.NoteBoundaryLine);
            GameObject boundary = Instantiate(noteBoundaryPrefab, position, noteBoundaryRot, environmentHierarchyTransform);

            boundaries.Add(boundary);
        }
    }

    protected Quaternion beatLineSpawnRot = Quaternion.Euler(new Vector3(0, 0, 0));
    protected Vector3 beatLineSpawnPos = new Vector3(0, 0, ChartEventSpawnDistance);
    protected void SpawnBeatLine(Chart.Beat beat) {
        GameObject note = Instantiate(ReferenceManager.Prefabs.BeatLineObject, beatLineSpawnPos, beatLineSpawnRot, ReferenceManager.BeatLinesHierarchyTransform);

        BeatLineController beatLineController = note.GetComponent<BeatLineController>();
        beatLineController.InitializeTargetTime(beat.PlayTime);
    }

    // Update is called once per frame
    protected void Update() {
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            AttemptSpawnBeatLine();
        }
    }

    protected int currBeat = 0;
    [CanBeNull] protected Chart.Beat nextBeatLineBeat = null;
    protected void AttemptSpawnBeatLine() {
        Chart.Chart? currChart = ReferenceManager.GameplayManager.CurrChart;
        if (currChart == null) {
            throw new Exception("Tried to spawn a beatline but CurrChart was still null! How is this possible?");
        }

        if (nextBeatLineBeat != null) {
            if (nextBeatLineBeat.PlayTime - currChart.NoteLeadDurationInSec < ReferenceManager.AudioManager.CurrTrackTime) {
                SpawnBeatLine(nextBeatLineBeat);
                nextBeatLineBeat = null;
            }
        }

        if (nextBeatLineBeat == null) {
            nextBeatLineBeat = new Chart.Beat(currChart, currBeat, 1, 0);
            currBeat += 1;
        }
    }

    public void ResetBeatLines() {
        currBeat = 0;
        nextBeatLineBeat = null;
    }
}
