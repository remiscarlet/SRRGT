using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private Transform environmentHierarchyTransform;
    private GameObject noteBoundaryPrefab;
    private Quaternion noteBoundaryRot;

    private List<GameObject> boundaries;

    public const float BoardWidth = 10.0f; // TODO: Tie this to config/class property of something else.

    public const int ChartEventSpawnDistance = 50;

    // Start is called before the first frame update
    void Start() {
        environmentHierarchyTransform = ReferenceManager.EnvironmentHierarchyTransform;
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
            GameObject boundary = Instantiate(noteBoundaryPrefab, position, noteBoundaryRot, environmentHierarchyTransform);

            boundaries.Add(boundary);
        }
    }

    private Quaternion beatLineSpawnRot = Quaternion.Euler(new Vector3(0, 0, 0));
    private Vector3 beatLineSpawnPos = new Vector3(0, 0, ChartEventSpawnDistance);
    private void SpawnBeatLine(ChartBeat beat) {
        GameObject note = Instantiate(ReferenceManager.Prefabs.BeatLineObject, beatLineSpawnPos, beatLineSpawnRot, ReferenceManager.BeatLinesHierarchyTransform);

        BeatLineController beatLineController = note.GetComponent<BeatLineController>();
        beatLineController.InitializeTargetTime(beat.PlayTime);
    }

    // Update is called once per frame
    void Update() {
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            AttemptSpawnBeatLine();
        }
    }

    private int currBeat = 0;
    [CanBeNull] private ChartBeat nextBeatLineBeat = null;
    void AttemptSpawnBeatLine() {
        Chart? currChart = ReferenceManager.GameplayManager.CurrChart;
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
            nextBeatLineBeat = new ChartBeat(currChart, currBeat, 1, 1);
            currBeat += 1;
        }
    }
}
