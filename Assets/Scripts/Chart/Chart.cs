using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Chart {
    public class Chart {
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int NumKeys { get; private set; }
        public int BPM { get; private set; }
        public double LeadOffset { get; private set; } // Offset of first beat in music
        public double NoteLeadDurationInSec { get; private set; } // Duration of time before music starts

        private AudioManager audioManager;

        private List<Event> chartEvents;

        private List<Event> allChartEvents;

        private List<Event> AllChartEvents {
            get {
                if (allChartEvents == null) {
                    allChartEvents = chartEvents;
                }

                return allChartEvents;
            }
        }
        public void InitializeExtraChartEvents(List<Event> extraChartEvents) {
            allChartEvents = new List<Event>(chartEvents);
            allChartEvents.AddRange(extraChartEvents);
        }

        public List<Event> BPMChanges { private set; get; } // Should guarantee order by ascending beat

        public Chart(string title, string artist, int numKeys, int bpm, double leadOffset) {
            Title = title;
            Artist = artist;
            NumKeys = numKeys;
            BPM = bpm;
            LeadOffset = leadOffset;

            NoteLeadDurationInSec = 1.0;

            BPMChanges = new List<Event>();
            chartEvents = new List<Event>();
            audioManager = ReferenceManager.AudioManager;

            Event initialBPM = new Event(this, Event.Types.BPMChange, 1);
            initialBPM.InitializeBPMChange(bpm);
            BPMChanges.Add(initialBPM);
        }

        public int CurrBPM() {
            if (BPMChanges.Count == 1) {
                return BPMChanges[0].BPM;
            }

            foreach (Event bpmChange in BPMChanges) {
                if (bpmChange.Beat.PlayTime <= ReferenceManager.AudioManager.CurrTrackTime) {
                    return bpmChange.BPM;
                }
            }

            throw new Exception("Huh!? Could not find a valid bpm...?");
        }

        public void PlayChart() {
            double currTime = ReferenceManager.AudioManager.CurrTrackTime;

            // TODO: God, this is so inefficient. Clean it up.
            List<Event> playedEvents = new List<Event>();
            foreach (Event chartEvent in AllChartEvents) {
                if (chartEvent.Beat.PlayTime <= currTime) {
                    // We're "too late" to play this note. Don't "play" it but add it to playedEvents to remove from list.
                    // This happens when we seek to a new track position and re-initialize extraChartEvents
                    playedEvents.Add(chartEvent);
                    continue;
                }

                if (chartEvent.Beat.PlayTime - NoteLeadDurationInSec <= currTime) {
                    ReferenceManager.NotesManager.SpawnChartEvent(chartEvent);
                    playedEvents.Add(chartEvent);
                }
            }

            /*
            chartEvents = chartEvents
                .Where(chartEvent => !playedEvents.Any(playedEvent => playedEvent == chartEvent))
                .ToList();
            extraChartEvents = extraChartEvents
                .Where(chartEvent => !playedEvents.Any(playedEvent => playedEvent == chartEvent))
                .ToList();
            */

            foreach (Event playedEvent in playedEvents) {
                allChartEvents.Remove(playedEvent);
            }
        }


        /// <summary>
        /// Eh...? This or AddChartEvent()
        /// </summary>
        /// <param name="events"></param>
        public void SetChartEvents(List<Event> events) {
            chartEvents = events;
        }

        public void AddChartEvent(Event chartEvent) {
            chartEvents.Add(chartEvent);

            if (chartEvent.EventType == Event.Types.BPMChange) {
                BPMChanges.Add(chartEvent);
            }
        }
    }
}
