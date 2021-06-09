using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorManager : GameplayManager {

    private List<Chart.Event> recordedChartEvents;

    private bool isRecording;
    public bool IsRecording {
        set {
            Debug.Log("Starting recording...");
            audioManager.ToggleTrackMusic();
            ReferenceManager.EditorUIManager.ToggleEditorUI();
            isRecording = value;
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
                Debug.Log("Restarting track via IsPlayingBack modification");
                audioManager.RestartTrack();
                InitializeRecordedEventsWithChart();
                ReferenceManager.BoardManager.ResetBeatLines();
            }
        }
        get { return isPlayingBack; }
    }

    public void InitializeRecordedEventsWithChart() {
        // TODO: Do we need to make a copy? Maybe not? Shared ref issues?
        CurrChart.InitializeExtraChartEvents(new List<Chart.Event>(recordedChartEvents));
    }


    public void PlayRecording() {
        Debug.Log("Executing PlayRecording()");
        IsPlayingBack = true;
        // Because this func is a button onclick callback, the button is "focused" after clicking.
        // This causes hitting KeyCode.Space to count as "clicking button". To avoid this, unfocus the gameObj
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Start is called before the first frame update
    void Start() {
        Initialize(false);

        recordedChartEvents = new List<Chart.Event>();
    }

    // Update is called once per frame
    void Update() {
        if (IsRecording) {
            CurrChart.PlayChart();
            RecordNotes();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                IsRecording = false;
            }
        } else if (IsPlayingBack) {
            if (!IsPaused) {
                CurrChart.PlayChart();
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

    private List<int> validSubdivs = new List<int>{1,2,4}; // TODO: Don't hardcode this.
    private void RecordNote(Chart.Event chartEvent) {
        double playTime = chartEvent.Beat.PlayTime;
        chartEvent.Beat.ApproximateBeatFromPlayTime(validSubdivs);
        Debug.Log($"Buffered new recorded note: notePos:{chartEvent.NotePos}, playTime:{playTime}, Beat:{chartEvent.Beat.ToString()}");
        recordedChartEvents.Add(chartEvent);
    }
}
