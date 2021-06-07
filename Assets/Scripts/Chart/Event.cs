using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Chart {
    public class Event {
        public enum Types {
            Note = 0,
            BPMChange = 1,
            NumKeysChange = 2
        }

        private Chart chart;
        private Beat beat;
        public Beat Beat {
            private set { beat = value; }
            get {
                if (!isInitialized) {
                    throw new Exception("Please initialize this Chart.Event first.");
                }

                return beat;
            }
        }
        public Types EventType { private set; get; }

        private int notePos = -1;

        public int NotePos {
            set {
                notePos = value;
            }
            get {
                if (notePos == -1) {
                    throw new Exception("Please initialize Chart.Event before getting NotePos!");
                }

                return notePos;
            }
        }

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

        public Event(Chart chart, Types eventType, int beatNum, int beatSubdiv, int beatSubdivIdx) {
            this.chart = chart;
            EventType = eventType;
            Beat = new Beat(chart, beatNum, beatSubdiv, beatSubdivIdx);
        }

        public Event(Chart chart, Types eventType, double playTime) {
            this.chart = chart;
            EventType = eventType;
            Beat = new Beat(chart, playTime);
        }

        public string ToString() {
            if (NotePos != -1) {
                return $"ChartEvent[type:{EventType}, beat:{Beat.ToString()}, NotePos:{NotePos}]";
            }
            else {
                return $"ChartEvent[type:{EventType}, beat:{Beat.ToString()}]";
            }
        }

        public bool Equals(Event other) {
            Debug.Log($"Comparing {this.ToString()} to {other.ToString()}");
            return this.EventType == other.EventType &&
                   this.NotePos == other.NotePos &&
                   this.Beat == other.Beat;
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
}
