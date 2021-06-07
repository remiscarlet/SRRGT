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
            List<Event> noExtraEvents = new List<Event>();
            PlayChart(ref noExtraEvents);
        }

        public void PlayChart(ref List<Event> extraChartEvents) {
            double currTime = ReferenceManager.AudioManager.CurrTrackTime;

            List<Event> events = chartEvents;
            if (extraChartEvents.Count > 0) {
                Debug.Log($"Adding extra chart events: {extraChartEvents} - {extraChartEvents.Count}");
                events.AddRange(extraChartEvents);
            }

            // TODO: God, this is so inefficient. Clean it up.
            List<Event> playedEvents = new List<Event>();
            foreach (Event chartEvent in events) {
                //Debug.Log($"BeatNum: {chartEvent.Beat.AsFloat()} - {LeadDuration} <= {currTime}");

                if (chartEvent.Beat.PlayTime - NoteLeadDurationInSec <= currTime) {
                    //Debug.Log(
                    //    $"Spawning chartEvent at `currTme:{currTime}` => {chartEvent.ToString()} - BPM:{BPM} - Target beat: {chartEvent.Beat.ToString()}");
                    ReferenceManager.NotesManager.SpawnChartEvent(chartEvent);
                    playedEvents.Add(chartEvent);
                    //Debug.Log($"Spawned: {chartEvent}");
                }
            }

            chartEvents = chartEvents
                .Where(chartEvent => !playedEvents.Any(playedEvent => playedEvent == chartEvent))
                .ToList();
            extraChartEvents = extraChartEvents
                .Where(chartEvent => !playedEvents.Any(playedEvent => playedEvent == chartEvent))
                .ToList();
            /*
            foreach (Event playedEvent in playedEvents) {
                Debug.Log($"Seeing where {playedEvent.ToString()} exists.");
                chartEvents = chartEvents.Where(chartEvent => chartEvent != playedEvent).ToList();
                extraChartEvents = extraChartEvents.Where(chartEvent => chartEvent != playedEvent).ToList();

                if (chartEvents.Contains(chartEvent)) {
                    Debug.Log("Here???");
                    chartEvents.Remove(chartEvent);
                } else if (extraChartEvents.Contains(chartEvent)) {
                    Debug.Log($">>> Removing chartEvent {chartEvent}");
                    extraChartEvents.Remove(chartEvent);
                }
            }
            */
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
