using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChartEvent {
    public enum Types {
        Note,
        BPMChange,
        numKeysChange
    }

    public ChartEvent(Types eventType, int? beatNum, double? playTime) {
        if (beatNum == null && playTime == null) {
            throw new Exception("Cannot initialize ChartNote without either beatNum or playTime. Must provide one!");
        }
        if (beatNum != null && playTime != null) {
            throw new Exception("Cannot initialize ChartNote with both beatNum and playTime. Must choose one!");
        }

        if (beatNum != null) {
            BeatNum = (int) beatNum; // I always hate explicit casts rather than relying on typechecking but should be safe in this case.
        } else if (playTime != null) {
            PlayTime = (double) playTime;
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
        bpm = newBPM;
        isInitialized = true;

    }

    public void InitializeNumKeysChange(int newNumKeys) {
        throw new Exception("Unimplemented");
    }

    public int NotePos { set; get; }

    private int bpm = -1;
    private int beatNum = -1;
    private double playTime = -1.0; // As in "the time at which to 'play' this note" in seconds. Use AudioSettings.dspTime

    public int BeatNum {
        set {
            beatNum = value;
            playTime = CalculatePlayTimeByBeat(beatNum);
        }
        get {
            if (beatNum == -1) {
                throw new Exception("Tried to get BeatNum before it was set. How is this possible?");
            }
            return beatNum;
        }
    }

    public double PlayTime {
        set {
            playTime = value;
        }
        get {
            if (playTime == -1.0) {
                throw new Exception("Tried to get PlayTime before it was set. How is this possible?");
            }

            return playTime;
        }
    }

    /// <summary>
    /// Does NOT account for start offset.
    ///
    /// All ChartEvent objects should be in the context of an already playing chart - ie offsets are accounted for already.
    /// </summary>
    /// <param name="beatNum"></param>
    /// <returns></returns>
    private double CalculatePlayTimeByBeat(int beatNum) {

        return 0.0;
    }
}
