using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NotesManager : MonoBehaviour {
    private List<NoteController> spawnedNotes;
    // Start is called before the first frame update
    void Start() {
        spawnedNotes = new List<NoteController>();
        foreach (Transform note in ReferenceManager.NotesHierarchyTransform) {
            spawnedNotes.Add(note.gameObject.GetComponent<NoteController>());
        }
    }

    public void DestroyNote(NoteController note) {
        spawnedNotes.Remove(note);
        Destroy(note.gameObject);
    }

    List<NoteController> GetAllJudgeableNotes() {
        return spawnedNotes.FindAll(note => note.gameObject.layer == Layers.JudgeableNotes);
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    void ProcessInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (NoteController note in GetAllJudgeableNotes()) {
                JudgeHit(note, KeyCode.Space);
            }
        }
    }

    void JudgeHit(NoteController note, KeyCode key) {
        float distToCenter = Math.Abs(note.transform.position.z);
        if (distToCenter <= 0.25f) {
            note.RegisterHitPerfect();
        } else if (distToCenter <= 0.75f) {
            note.RegisterHitGood();
        } else {
          note.RegisterHitMiss();
        }
    }
}
