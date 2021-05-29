using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.PlayerLoop;

public class ChartReader {
    public const string TITLE_KEY = "title:"; // Note the colons.
    public const string ARTIST_KEY = "artist:";
    public const string NUMKEYS_KEY = "numkeys:";
    public const string BPM_KEY = "bpm:";
    public const string LEAD_KEY = "lead:"; // "Start offset"
    public const string CHART_KEY = "chart:";

    /// <summary>
    /// This function is mega gross.
    /// Refactor to be prettier.
    /// </summary>
    /// <param name="chartPath"></param>
    /// <exception cref="Exception"></exception>
    public void ReadAndParseChart(string chartPath) {
        StreamReader sr = new StreamReader(chartPath);

        String title = "", artist = "";
        int numKeys = -1, bpm = -1;
        double lead = -1.0;

        String? line = null;
        bool hitChartDataDelimiter = false;
        while (!hitChartDataDelimiter) {
            line = sr.ReadLine();
            if (line == null) {
                throw new Exception("Hit EOF of chart file before finished initializing!");
            }

            switch (line) {
                case TITLE_KEY:
                    title = line.Remove(0, TITLE_KEY.Length + 1); // +1 to account for space after colon
                    break;
                case ARTIST_KEY:
                    artist = line.Remove(0, ARTIST_KEY.Length + 1);
                    break;
                case NUMKEYS_KEY:
                    numKeys = int.Parse(line.Remove(0, NUMKEYS_KEY.Length + 1));
                    break;
                case BPM_KEY:
                    bpm = int.Parse(line.Remove(0, BPM_KEY.Length + 1));
                    break;
                case LEAD_KEY:
                    lead = double.Parse(line.Remove(0, LEAD_KEY.Length + 1));
                    break;
                case CHART_KEY:
                    hitChartDataDelimiter = true;
                    break;
                default:
                    throw new Exception($"Got to a line that we couldn't parse! Got: `{line}`");
            }
        }

        Chart chart = new Chart(title, artist, numKeys, bpm, lead);

        if (line == null) {
            throw new Exception("How is line null???");
        }

        // Chart notes from here
        while (line != null) {
            line = sr.ReadLine();
            ParseChartLine(line, chart);
        }
    }

    private void ParseChartLine(string line, Chart chart) {
        string[] parts = line.Split(',');

        ChartEvent.Types eventType = (ChartEvent.Types) int.Parse(parts[0]);
        if (eventType == ChartEvent.Types.Note) {
            string notePosString = parts[1];
            int[] notePositions = Array.ConvertAll(notePosString.Split('-'), int.Parse);

            bool isByBeat = Convert.ToBoolean(parts[2]);
            int? beatNum = null;
            double? playTime = null;
            if (isByBeat) {
                beatNum = int.Parse(parts[3]);
            } else {
                playTime = double.Parse(parts[3]);
            }

            foreach (int notePos in notePositions) {
                ChartEvent chartEvent = InstantiateNoteEvent(eventType, notePos, beatNum, playTime);
                chart.AddChartEvent(chartEvent);
            }
        } else if (eventType == ChartEvent.Types.BPMChange) {
            throw new Exception("Unimplemented");
        } else if (eventType == ChartEvent.Types.NumKeysChange) {
            throw new Exception("Unimplemented");
        }
    }

    private ChartEvent InstantiateNoteEvent(ChartEvent.Types eventType, int notePos, int? beatNum, double? playTime) {
        ChartEvent chartEvent = new ChartEvent(eventType, beatNum, playTime);
        if (eventType == ChartEvent.Types.Note) {
            chartEvent.InitializeNote(notePos);
        } else {
            throw new Exception("Unimplemented exception");
        }

        return chartEvent;
    }
}
