 using System;
using System.Collections;
using System.Collections.Generic;
 using System.ComponentModel;
 using UnityEditor;
using UnityEngine;

public class NotesManager : MonoBehaviour {
    private float boardWidth = 10.0f;
    private int numKeys = 7;

    private int spawnDistance = 50;

    private List<KeyCode> notePosToKeyCodeMapping;
    private List<Vector3> noteSpawnPositions;

    private List<NoteController> spawnedNotes;
    // Start is called before the first frame update
    void Start() {
        spawnedNotes = new List<NoteController>();
        foreach (Transform note in ReferenceManager.NotesHierarchyTransform) {
            spawnedNotes.Add(note.gameObject.GetComponent<NoteController>());
        }

        CalculateNotePosToKeyCodes();
        CalculateSpawnPositions();
    }

    void CalculateNotePosToKeyCodes() {
        // TODO: Make more robust/input manager class

        List<KeyCode> keys;

        if (numKeys == 4) {
            keys = new List<KeyCode> { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
        } else if (numKeys == 5) {
            keys = new List<KeyCode> { KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K };
        } else if (numKeys == 6) {
            keys = new List<KeyCode> { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        } else if (numKeys == 7) {
            keys = new List<KeyCode> { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };
        } else {
            throw new Exception("Got invalid number of keys for initialization.");
        }

        notePosToKeyCodeMapping = keys;
    }

    void CalculateSpawnPositions() {
        float xOffset = boardWidth / -2.0f;
        float noteWidth = boardWidth / numKeys;

        noteSpawnPositions = new List<Vector3>();

        for (int i = 0; i < numKeys; i++) {
            noteSpawnPositions.Add(new Vector3(xOffset + (i + 0.5f) * noteWidth, 0, spawnDistance));
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
        SpawnNotes();
        ProcessInput();
    }

    private List<KeyCode> spawnButtons = new List<KeyCode> { KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6 };
    void SpawnNotes() {
        for (int i = 0; i < numKeys; i++) {
            KeyCode key = spawnButtons[i];
            if (Input.GetKeyDown(key)) {
                SpawnNote(i);
            }
        }
    }

    private Quaternion fwdRotation = Quaternion.Euler(new Vector3(0, 0, 90));

    void SpawnNote(int pos) {
        Vector3 position = noteSpawnPositions[pos];
        GameObject note = Instantiate(ReferenceManager.Prefabs.NoteObject, position, fwdRotation, ReferenceManager.NotesHierarchyTransform);

        NoteController noteController = note.GetComponent<NoteController>();
        noteController.Pos = pos;
        Debug.Log($"Spawning note: {noteController.Pos}");
        spawnedNotes.Add(noteController);
    }

    void ProcessInput() {
        foreach (KeyCode key in notePosToKeyCodeMapping) {
            if (Input.GetKeyDown(key)) {
                foreach (NoteController note in GetAllJudgeableNotes()) {
                    JudgeHit(note, key);
                }
            }
        }
    }

    KeyCode TranslateNotePosToKeyCode(int notePos) {
        Debug.Log($"Notepos is {notePos}");
        return notePosToKeyCodeMapping[notePos];
    }

    void JudgeHit(NoteController note, KeyCode pressedKey) {
        // TODO: Make timing based, not distance based
        if (TranslateNotePosToKeyCode(note.Pos) == pressedKey) {
            float distToCenter = Math.Abs(note.transform.position.z);
            if (distToCenter <= 0.25f) {
                note.RegisterHitPerfect();
            }
            else if (distToCenter <= 0.75f) {
                note.RegisterHitGood();
            }
            else {
                note.RegisterHitMiss();
            }
        }
    }
}
