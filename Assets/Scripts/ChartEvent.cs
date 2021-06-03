using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChartEvent {
    public enum Types {
        Note = 0,
        BPMChange = 1,
        NumKeysChange = 2
    }

    private Chart chart;
    public ChartBeat Beat { private set; get; }
    public Types EventType { private set; get; }

    public int NotePos { set; get; } = -1;

    private int bpm = -1;
    public int BPM {
        private set { bpm = value; }
        get {
            if (bpm == -1) {
                throw new Exception("No BPM has been configured for this ChartEvent!");
            }
            return bpm;
        }
    } // Unintuitively, bpm doesn't actually affect the "beat" so it's not a part of it.

    [CanBeNull] private GameObject note;

    public ChartEvent(Chart chart, Types eventType, int beatNum, int beatSubdiv, int beatSubdivIdx) {
        this.chart = chart;
        EventType = eventType;
        Beat = new ChartBeat(chart, beatNum, beatSubdiv, beatSubdivIdx);
    }

    public ChartEvent(Chart chart, Types eventType, double playTime) {
        this.chart = chart;
        EventType = eventType;
        Beat = new ChartBeat(chart, playTime);
    }

    public string ToString() {
        if (NotePos != -1) {
            return $"ChartEvent[type:{EventType}, beat:{Beat.ToString()}, NotePos:{NotePos}]";
        } else {
            return $"ChartEvent[type:{EventType}, beat:{Beat.ToString()}]";
        }

    }

    private bool isInitialized = false;

    /// <summary>
    /// This feels super not-CSharp-like. Really want python kwargs right about now. And function decorators.
    /// </summary>
    public void InitializeNote(int notePos) {
        if (isInitialized) {
            throw new Exception("Trying to initialize a note that's already been initialized!");
        }
        NotePos = notePos;
        isInitialized = true;
    }

    public void InitializeBPMChange(int newBPM) {
        if (isInitialized) {
            throw new Exception("Trying to initialize a note that's already been initialized!");
        }
        BPM = newBPM;
        isInitialized = true;
    }

    public void InitializeNumKeysChange(int newNumKeys) {
        throw new Exception("Unimplemented");
    }
}
