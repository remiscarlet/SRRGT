using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.UI;
using UnityEngine;

public class NoteController : MonoBehaviour {
    private bool noteAlreadyMissed = false;

    public int Pos {
        get;
        set;
    } = 0;

    private ScoreManager ScoreManager;
    private NotesManager NotesManager;

    // Start is called before the first frame update
    void Start() {
        ScoreManager = ReferenceManager.ScoreManager;
        NotesManager = ReferenceManager.NotesManager;
    }

    private double targetTime = -1.0;
    private double spawnedTime = -1.0;
    private float spawnZOffset = -1.0f;
    public void InitializeTargetTime(double targetTime) {
        this.targetTime = targetTime;
        this.spawnedTime = ReferenceManager.AudioManager.CurrTrackTime;
        this.spawnZOffset = transform.position.z;
        Debug.Log($"Initializing NoteController with: targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}");
    }

    public void RegisterHitMiss() {
        ScoreManager.RegisterMiss(transform);
        SelfDestroy();
    }

    public void RegisterHitGood() {
        ScoreManager.RegisterGood(transform);
        SelfDestroy();
    }

    public void RegisterHitPerfect() {
        ScoreManager.RegisterPerfect(transform);
        SelfDestroy();
    }

    // Update is called once per frame
    void Update() {
        if (targetTime == -1.0) {
            return;
        }

        UpdatePosition();

        if (NoteWasMissed() && ! noteAlreadyMissed) {
            // Point at which note is missed is, ie spawning hitresult text, is not the exact
            // same as when the object should be deleted. Deletion comes a little later.

            noteAlreadyMissed = true;
            RegisterHitMiss();
        } else if (IsOutOfBounds()) {
            // Shouldn't hit If not miss is at earlier z?
            SelfDestroy();
        }
    }

    bool NoteWasMissed() {
        // TODO: Make this timing based eventually. For now, naive position based.
        return gameObject.layer == Layers.MissedNotes;
    }

    public int minZPos = -3;

    bool IsOutOfBounds() {
        return transform.position.z <= minZPos;
    }

    void SelfDestroy() {
        NotesManager.DestroyNote(this);
    }

    void UpdatePosition() {
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            if (spawnedTime == -1.0 || targetTime == -1.0) {
                throw new Exception("Tried updating note event position before initializing!");
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

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HitOuter")) {
            gameObject.layer = Layers.JudgeableNotes;
        } else if (other.CompareTag("MissedNoteLine")) {
            gameObject.layer = Layers.MissedNotes;
        }
    }
}
