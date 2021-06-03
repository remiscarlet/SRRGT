using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.UI;
using UnityEngine;

public class ChartEventController : MonoBehaviour {
    protected ScoreManager ScoreManager;
    protected NotesManager NotesManager;

    // Start is called before the first frame update
    void Start() {
        ScoreManager = ReferenceManager.ScoreManager;
        NotesManager = ReferenceManager.NotesManager;
    }

    protected double targetTime = -1.0;
    protected double spawnedTime = -1.0;
    protected float spawnZOffset = -1.0f;

    public void InitializeTargetTime(double targetTime) {
        this.targetTime = targetTime;
        this.spawnedTime = ReferenceManager.AudioManager.CurrTrackTime;
        this.spawnZOffset = transform.position.z;
        Debug.Log(
            $"Initializing ChartEventController with: targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}");
    }

    public new string ToString() {
        return $"ChartEventController[targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}]";
    }

    // Update is called once per frame
    protected void Update() {
        if (targetTime == -1.0) {
            return;
        }

        UpdatePosition();
    }

    public int minZPos = -3;

    protected bool IsOutOfBounds() {
        return transform.position.z <= minZPos;
    }

    protected void UpdatePosition() {
        // TODO: Make movement non-linear? Eg, move faster the farther away note is from judgement line. Maffs.
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            if (targetTime == -1.0) {
                throw new Exception($"Tried updating note event position before initializing! {ToString()}");
            }

            double now = ReferenceManager.AudioManager.CurrTrackTime;
            double timeToPlayTime = now - spawnedTime;
            double percentToPlayTime = timeToPlayTime / (targetTime - spawnedTime);

            float zPos = spawnZOffset * (float) (1 - percentToPlayTime);

            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, zPos);
            // Debug.Log($"Moving note {this} to newPos: {newPos} with zPos:{zPos}, pcntToPlayTime:{percentToPlayTime}, timeToPlayTime:{timeToPlayTime}, spawnedTime:{spawnedTime}, targetTime:{targetTime}, spawnZOffset:{spawnZOffset}");
            transform.position = newPos;
        }
    }
}
