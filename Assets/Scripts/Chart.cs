using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chart {
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int NumKeys { get; private set; }
    public int BPM { get; private set; }
    public double LeadOffset { get; private set; }

    public Chart(string title, string artist, int numKeys, int bpm, double leadOffset) {
        Title = title;
        Artist = artist;
        NumKeys = numKeys;
        BPM = bpm;
        LeadOffset = leadOffset;

        chartEvents = new List<ChartEvent>();
    }

    private List<ChartEvent> chartEvents;

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
