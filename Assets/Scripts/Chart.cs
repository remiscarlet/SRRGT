using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chart {
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int NumKeys { get; private set; }
    public int BPM { get; private set; }
    public double LeadOffset { get; private set; } // Offset of first beat in music
    public double LeadDuration { get; private set; } // Duration of time before music starts. Ie, gives time for first chartevent lead in. In future have speed modifiers modify in get;

    private AudioManager audioManager;

    private List<ChartEvent> chartEvents;

    public Chart(string title, string artist, int numKeys, int bpm, double leadOffset) {
        Title = title;
        Artist = artist;
        NumKeys = numKeys;
        BPM = bpm;
        LeadOffset = leadOffset;

        LeadDuration = 2.0;

        chartEvents = new List<ChartEvent>();
        audioManager = ReferenceManager.AudioManager;
    }

    public void PlayChart() {
        double currTime = ReferenceManager.AudioManager.CurrTrackTime;
        double currBeat = AudioManager.ConvertTimeToBeatAsDouble(currTime, BPM);

        // TODO: God, this is so inefficient. Clean it up.
        List<ChartEvent> playedEvents = new List<ChartEvent>();
        foreach (ChartEvent chartEvent in chartEvents) {
            Debug.Log($"BeatNum: {chartEvent.BeatNum} - {LeadDuration} <= {currBeat}");

            if (chartEvent.BeatNum - LeadDuration <= currBeat) {
                Debug.Log($"Spawning chartEvent at `beat:{currBeat}` => {chartEvent} - currTime:{currTime}, BPM:{BPM} - Target beat: {chartEvent.BeatNum}");
                ReferenceManager.NotesManager.SpawnChartEvent(chartEvent);
                playedEvents.Add(chartEvent);
                Debug.Log($"Spawned: {chartEvent}");
            }
        }

        foreach (ChartEvent chartEvent in playedEvents) {
            chartEvents.Remove(chartEvent);
        }
    }


    /// <summary>
    /// Eh...? This or AddChartEvent()
    /// </summary>
    /// <param name="events"></param>
    public void SetChartEvents(List<ChartEvent> events) {
        chartEvents = events;
    }

    public void AddChartEvent(ChartEvent chartEvent) {
        chartEvents.Add(chartEvent);
    }
}
