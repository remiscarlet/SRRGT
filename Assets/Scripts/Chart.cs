using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chart {
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int NumKeys { get; private set; }
    public int BPM { get; private set; }
    public double LeadOffset { get; private set; } // Offset of first beat in music
    public double NoteLeadDurationInSec { get; private set; } // Duration of time before music starts. Ie, gives time for first chartevent lead in. In future have speed modifiers modify in get;

    private AudioManager audioManager;

    private List<ChartEvent> chartEvents;

    public List<ChartEvent> BPMChanges { private set; get; } // Should guarantee order by ascending beat

    public Chart(string title, string artist, int numKeys, int bpm, double leadOffset) {
        Title = title;
        Artist = artist;
        NumKeys = numKeys;
        BPM = bpm;
        LeadOffset = leadOffset;

        NoteLeadDurationInSec = 1.0;

        BPMChanges = new List<ChartEvent>();
        chartEvents = new List<ChartEvent>();
        audioManager = ReferenceManager.AudioManager;

        ChartEvent initialBPM = new ChartEvent(this, ChartEvent.Types.BPMChange, 1);
        initialBPM.InitializeBPMChange(bpm);
        BPMChanges.Add(initialBPM);
    }

    public void PlayChart() {
        double currTime = ReferenceManager.AudioManager.CurrTrackTime;

        // TODO: God, this is so inefficient. Clean it up.
        List<ChartEvent> playedEvents = new List<ChartEvent>();
        foreach (ChartEvent chartEvent in chartEvents) {
            //Debug.Log($"BeatNum: {chartEvent.Beat.AsFloat()} - {LeadDuration} <= {currTime}");

            if (chartEvent.Beat.PlayTime - NoteLeadDurationInSec <= currTime) {
                Debug.Log($"Spawning chartEvent at `currTme:{currTime}` => {chartEvent.ToString()} - BPM:{BPM} - Target beat: {chartEvent.Beat.ToString()}");
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

        if (chartEvent.EventType == ChartEvent.Types.BPMChange) {
            BPMChanges.Add(chartEvent);
        }
    }
}
