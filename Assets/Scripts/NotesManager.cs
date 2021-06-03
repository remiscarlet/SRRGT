 using System;
using System.Collections;
using System.Collections.Generic;
 using System.ComponentModel;
 using JetBrains.Annotations;
 using UnityEditor;
using UnityEngine;

public class NotesManager : MonoBehaviour {
    private int numKeys = 7;

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
        float xOffset = (float) BoardManager.BoardWidth / -2.0f;
        float noteWidth = (float) BoardManager.BoardWidth / numKeys;

        noteSpawnPositions = new List<Vector3>();

        for (int i = 0; i < numKeys; i++) {
            noteSpawnPositions.Add(new Vector3(xOffset + (i + 0.5f) * noteWidth, 0, BoardManager.ChartEventSpawnDistance));
        }
    }

    public void DestroyNote(NoteController note) {
        spawnedNotes.Remove(note);
        Destroy(note.gameObject);
    }

    List<NoteController> GetAllJudgeableNotesForPos(int notePos) {
        return spawnedNotes.FindAll(note => note.gameObject.layer == Layers.JudgeableNotes && note.Pos == notePos);
    }

    /// <summary>
    /// This or GetAllJudgeableNotesForPos?
    /// Normally one keypress should never judge more than one note at a time. If that's the case
    /// do we ever need to return more than the "most relevant" note when judging accuracy?
    /// </summary>
    /// <param name="notePos"></param>
    /// <returns></returns>
    [CanBeNull]
    NoteController GetNextJudgeableNoteForPos(int notePos) {
        List<NoteController> notes = GetAllJudgeableNotesForPos(notePos);
        if (notes.Count == 0) {
            return null;
        }

        return notes[0];
    }

    public void SpawnChartEvent(ChartEvent chartEvent) {
        SpawnNote(chartEvent.NotePos, chartEvent.Beat);
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
                SpawnNote(i, AudioSettings.dspTime + 0.5); // Hard code target time of "0.5 seconds later"
            }
        }
    }

    private Quaternion fwdRotation = Quaternion.Euler(new Vector3(0, 0, 90));

    void SpawnNote(int pos, ChartBeat beat) {
        SpawnNote(pos, beat.PlayTime);
    }

    void SpawnNote(int pos, double playTime) {
        Vector3 position = noteSpawnPositions[pos];
        GameObject note = Instantiate(ReferenceManager.Prefabs.NoteObject, position, fwdRotation, ReferenceManager.NotesHierarchyTransform);

        NoteController noteController = note.GetComponent<NoteController>();
        noteController.Pos = pos;
        noteController.InitializeTargetTime(playTime);
        spawnedNotes.Add(noteController);
    }

    void ProcessInput() {
        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            foreach (KeyCode key in notePosToKeyCodeMapping) {
                if (Input.GetKeyDown(key)) {
                    NoteController? note = GetNextJudgeableNoteForPos(KeyCodeToNotePos(key));
                    if (note != null) {
                        JudgeHit(note);
                    }
                }
            }
        }
    }

    KeyCode NotePosToKeyCode(int notePos) {
        if (notePos >= notePosToKeyCodeMapping.Count) {
            throw new Exception($"Got notePos value greater than configured number of keys! Got: {notePos}");
        }
        Debug.Log($"Notepos is {notePos}");
        return notePosToKeyCodeMapping[notePos];
    }

    int KeyCodeToNotePos(KeyCode key) {
        if (!notePosToKeyCodeMapping.Contains(key)) {
            throw new Exception($"Tried to get notepos of invalid key. Got: {key}");
        }
        return notePosToKeyCodeMapping.IndexOf(key);
    }

    private const double MinPerfectJudgementDelta = 0.02; // seconds
    private const double MinGoodJudgementDelta = 0.075; // seconds
    void JudgeHit(NoteController note) {
        Debug.Log($"Registering hit at: beat:{AudioManager.ConvertTimeToBeatAsDouble(ReferenceManager.AudioManager.CurrTrackTime, ReferenceManager.GameplayManager.CurrChart.BPM)}, time:{ReferenceManager.AudioManager.CurrTrackTime}");
        AudioManager.PlayHitSound();

        double beatDuration = 60.0 / ReferenceManager.GameplayManager.CurrChart.CurrBPM();
        double timingDelta = note.TimeFromTarget();

        double perfectDelta = Math.Min(beatDuration * 0.05, MinPerfectJudgementDelta);
        double goodDelta = Math.Min(beatDuration * 0.15, MinGoodJudgementDelta);

        if (timingDelta <= perfectDelta) {
            note.RegisterHitPerfect();
        }
        else if (timingDelta <= goodDelta) {
            note.RegisterHitGood();
        }
        else {
            note.RegisterHitMiss();
        }
        Debug.Log($"Judging note hit: timingDelta:{timingDelta}, beatDuration:{beatDuration}");
    }
}
