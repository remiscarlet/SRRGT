 using System;
using System.Collections;
using System.Collections.Generic;
 using System.ComponentModel;
 using JetBrains.Annotations;
 using UnityEditor;
using UnityEngine;

public class NotesManager : MonoBehaviour {
    private int NumKeys {
        get {
            Chart.Chart currChart = ReferenceManager.GameplayManager.CurrChart;
            if (currChart == null) {
                throw new Exception("Tried to get NumKeys from currChart but currChart is null!");
            }

            return currChart.NumKeys;
        }
    }

    public List<KeyCode> NotePosToKeyCodeMapping { private set; get; }
    private List<Vector3> noteSpawnPositions;

    private List<NoteController> spawnedNotes;
    // Start is called before the first frame update
    private void Start() {
        spawnedNotes = new List<NoteController>();
        foreach (Transform note in ReferenceManager.NotesHierarchyTransform) {
            spawnedNotes.Add(note.gameObject.GetComponent<NoteController>());
        }

        CalculateNotePosToKeyCodes();
        CalculateSpawnPositions();
    }

    private void CalculateNotePosToKeyCodes() {
        // TODO: Make more robust/input manager class

        List<KeyCode> keys;

        if (NumKeys == 4) {
            keys = new List<KeyCode> { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
        } else if (NumKeys == 5) {
            keys = new List<KeyCode> { KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K };
        } else if (NumKeys == 6) {
            keys = new List<KeyCode> { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        } else if (NumKeys == 7) {
            keys = new List<KeyCode> { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };
        } else {
            throw new Exception("Got invalid number of keys for initialization.");
        }

        NotePosToKeyCodeMapping = keys;
    }

    private void CalculateSpawnPositions() {
        float xOffset = BoardManager.BoardWidth / -2.0f;
        float noteWidth = BoardManager.BoardWidth / NumKeys;

        noteSpawnPositions = new List<Vector3>();

        for (int i = 0; i < NumKeys; i++) {
            noteSpawnPositions.Add(new Vector3(xOffset + (i + 0.5f) * noteWidth, 0, BoardManager.ChartEventSpawnDistance));
        }
    }

    public void DestroyNote(NoteController note) {
        spawnedNotes.Remove(note);
        Destroy(note.gameObject);
    }

    private List<NoteController> GetAllJudgeableNotesForPos(int notePos) {
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
    private NoteController GetNextJudgeableNoteForPos(int notePos) {
        List<NoteController> notes = GetAllJudgeableNotesForPos(notePos);
        return notes.Count == 0 ? null : notes[0];
    }

    public void SpawnChartEvent(Chart.Event chartEvent) {
        SpawnNote(chartEvent.NotePos, chartEvent.Beat);
    }

    // Update is called once per frame
    private void Update() {
        SpawnNotes();

        if (ReferenceManager.AudioManager.IsPlayingTrack) {
            ProcessInput();
        }
    }

    private List<KeyCode> spawnButtons = new List<KeyCode> { KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6 };
    void SpawnNotes() {
        for (int i = 0; i < NumKeys; i++) {
            KeyCode key = spawnButtons[i];
            if (Input.GetKeyDown(key)) {
                SpawnNote(i, AudioSettings.dspTime + 0.5); // Hard code target time of "0.5 seconds later"
            }
        }
    }

    private Quaternion fwdRotation = Quaternion.Euler(new Vector3(0, 0, 90));

    private void SpawnNote(int pos, Chart.Beat beat) {
        SpawnNote(pos, beat.PlayTime, beat.OrigPlayTime != null);
    }

    private void SpawnNote(int pos, double playTime, bool isApproximated = false) {
        //Debug.Log($"Spawning at pos `{pos}` and at time `{playTime}`");
        Vector3 position = noteSpawnPositions[pos];
        GameObject note = Instantiate(ReferenceManager.Prefabs.NoteObject, position, fwdRotation, ReferenceManager.NotesHierarchyTransform);

        NoteController noteController = note.GetComponent<NoteController>();
        noteController.Pos = pos;
        noteController.InitializeTargetTime(playTime);
        if (isApproximated) {
            noteController.InitializeApproximatedNote();
        }
        spawnedNotes.Add(noteController);
    }

    private void ProcessInput() {
        foreach (KeyCode key in NotePosToKeyCodeMapping) {
            if (!Input.GetKeyDown(key)) {
                continue;
            }

            NoteController? note = GetNextJudgeableNoteForPos(KeyCodeToNotePos(key));
            if (note != null) {
                JudgeHit(note);
            }
        }
    }

    private KeyCode NotePosToKeyCode(int notePos) {
        if (notePos >= NotePosToKeyCodeMapping.Count) {
            throw new Exception($"Got notePos value greater than configured number of keys! Got: {notePos}");
        }
        Debug.Log($"Notepos is {notePos}");
        return NotePosToKeyCodeMapping[notePos];
    }

    private int KeyCodeToNotePos(KeyCode key) {
        if (!NotePosToKeyCodeMapping.Contains(key)) {
            throw new Exception($"Tried to get notepos of invalid key. Got: {key}");
        }
        return NotePosToKeyCodeMapping.IndexOf(key);
    }

    private const double MinPerfectJudgementDelta = 0.02; // seconds
    private const double MinGoodJudgementDelta = 0.075; // seconds
    private void JudgeHit(NoteController note) {
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
