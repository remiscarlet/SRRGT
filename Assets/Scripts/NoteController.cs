using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.UI;
using UnityEngine;

public class NoteController : Chart.EventController {
    private bool noteAlreadyMissed = false;

    public int Pos {
        get;
        set;
    } = 0;

    public new string ToString() {
        return $"NoteController[targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}]";
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
        base.Update();

        if (NoteWasMissed() && ! noteAlreadyMissed) {
            // Point at which note is missed is, ie spawning hitresult text, is not the exact
            // same as when the object should be deleted. Deletion comes a little later.

            noteAlreadyMissed = true;
            RegisterHitMiss();
        }
    }

    bool NoteWasMissed() {
        // TODO: Make this timing based eventually. For now, naive position based.
        return gameObject.layer == Layers.MissedNotes;
    }

    void SelfDestroy() {
        NotesManager.DestroyNote(this);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HitOuter")) {
            gameObject.layer = Layers.JudgeableNotes;
        } else if (other.CompareTag("MissedNoteLine")) {
            gameObject.layer = Layers.MissedNotes;
        }
    }
}
