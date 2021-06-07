using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EditorManager : GameplayManager {

    private List<Chart.Event> recordedChartEvents;
    private List<Chart.Event> bufferedChartEvents;

    private bool isRecording;
    public bool IsRecording {
        set {
            Debug.Log("Starting recording...");
            audioManager.ToggleTrackMusic();
            ReferenceManager.EditorUIManager.ToggleEditorUI();
            isRecording = value;

            if (!isRecording) {
                // Create a new copy of the recorded notes every time we stop recording.
                // Upon playback, we remove references from the copied list so as to not modify the original
                // recordedChartEvents list.
                bufferedChartEvents = new List<Chart.Event>(recordedChartEvents);
            }
        }
        get { return isRecording; }
    }

    public void StartRecording() {
        IsRecording = true;
    }

    private bool isPlayingBack;
    public bool IsPlayingBack {
        set {
            audioManager.ToggleTrackMusic();
            isPlayingBack = value;
            if (value) {
                audioManager.RestartTrack();
            }
        }
        get { return isPlayingBack; }
    }

    public void PlayRecording() {
        IsPlayingBack = true;
    }

    // Start is called before the first frame update
    void Start() {
        Initialize(false);

        recordedChartEvents = new List<Chart.Event>();
    }

    // Update is called once per frame
    void Update() {
        if (IsRecording) {
            Debug.Log("IsRecording: True");
            CurrChart.PlayChart();
            RecordNotes();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                IsRecording = false;
            }
        } else if (IsPlayingBack) {
            Debug.Log("IsPlayingBack: True");
            if (!IsPaused) {
                Debug.Log($">> Passing by ref: bufferedChartEvents: {bufferedChartEvents}");
                CurrChart.PlayChart(ref bufferedChartEvents);
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                TogglePauseState();
            }
        }
    }

    private void RecordNotes() {
        //foreach (KeyCode key in ReferenceManager.NotesManager.NotePosToKeyCodeMapping) {
        List<KeyCode> notes = ReferenceManager.NotesManager.NotePosToKeyCodeMapping;
        for (int notePos = 0; notePos < notes.Count; notePos++) {
            KeyCode key = notes[notePos];
            if (Input.GetKeyDown(key)) {
                Chart.Event chartEvent = new Chart.Event(CurrChart, Chart.Event.Types.Note, ReferenceManager.AudioManager.CurrTrackTime);
                chartEvent.InitializeNote(notePos);
                RecordNote(chartEvent);
            }
        }
    }

    private void RecordNote(Chart.Event chartEvent) {
        Debug.Log($"Buffered new recorded note: {chartEvent} - {chartEvent.NotePos}, {chartEvent.Beat}");
        recordedChartEvents.Add(chartEvent);
    }
}
