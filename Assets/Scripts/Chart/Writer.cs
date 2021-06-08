using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.PlayerLoop;

namespace Chart {
    public class Writer {
        public const string TITLE_KEY = "title";
        public const string ARTIST_KEY = "artist";
        public const string NUMKEYS_KEY = "numkeys";
        public const string BPM_KEY = "bpm";
        public const string LEAD_KEY = "lead"; // "Start offset"
        public const string CHART_KEY = "chart";

        /// <summary>
        /// This function is mega gross.
        /// Refactor to be prettier.
        /// </summary>
        /// <param name="chartPath"></param>
        /// <exception cref="Exception"></exception>
        public Chart InstantiateChart(string chartPath) {
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

                string[] line_parts = line.Split(':');
                string line_key = line_parts[0], line_content = line_parts[1].Trim();
                switch (line_key) {
                    case TITLE_KEY:
                        title = line_content;
                        break;
                    case ARTIST_KEY:
                        artist = line_content;
                        break;
                    case NUMKEYS_KEY:
                        numKeys = int.Parse(line_content);
                        break;
                    case BPM_KEY:
                        bpm = int.Parse(line_content);
                        break;
                    case LEAD_KEY:
                        lead = double.Parse(line_content);
                        break;
                    case CHART_KEY:
                        if (title == "" || artist == "" || numKeys == -1 || bpm == -1 || lead == -1.0) {
                            throw new Exception("Got to CHART_KEY without initializing all other fields first!");
                        }

                        Debug.Log($"title: {title}, artist: {artist}, numKeys: {numKeys}, bpm: {bpm}, lead: {lead}");
                        hitChartDataDelimiter = true;
                        break;
                    default:
                        throw new Exception($"Got to a line that we couldn't parse! Got: `{line}` => `{line_key}`");
                }
            }

            Chart chart = new Chart(title, artist, numKeys, bpm, lead);

            if (line == null) {
                throw new Exception("How is line null???");
            }

            // At this point 'line' should still be set to CHART_KEY. Move to next line.
            line = sr.ReadLine();
            while (line != null) {
                if (line[0] != '#') {
                    // Allow lines starting with octothorp be considered 'comments'
                    try {
                        ParseChartLine(line, chart);
                    }
                    catch (Exception ex) {
                        throw new Exception($"Failed to correctly parse line: {line}", ex);
                    }
                }

                line = sr.ReadLine();
            }

            return chart;
        }

        private void ParseChartLine(string line, Chart chart) {
            Debug.Log($"Parsing line: {line}");
            string[] parts = line.Split(',');

            Event.Types eventType = (Event.Types) int.Parse(parts[0]);
            if (eventType == Event.Types.Note) {
                string notePosString = parts[1];
                int[] notePositions = Array.ConvertAll(notePosString.Split('-'), int.Parse);

                // Debug.Log($"Attempting to convert {parts[2]} into a boolean...");

                int? beatNum = null, beatSubdiv = null, beatSubdivIdx = null;
                double? playTime = null;

                bool isByBeat = Convert.ToBoolean(int.Parse(parts[2]));
                if (isByBeat) {
                    beatNum = int.Parse(parts[3]);
                    beatSubdiv = int.Parse(parts[4]);
                    beatSubdivIdx = int.Parse(parts[5]);
                }
                else {
                    playTime = double.Parse(parts[3]);
                }

                foreach (int notePos in notePositions) {
                    Event chartEvent; // Each notePos is still a unique chartEvent.
                    if (isByBeat) {
                        chartEvent = new Event(chart, eventType, (int) beatNum, (int) beatSubdiv,
                            (int) beatSubdivIdx);
                    }
                    else {
                        chartEvent = new Event(chart, eventType, (double) playTime);
                    }

                    InstantiateNoteEvent(chart, chartEvent, notePos);
                }
            }
            else if (eventType == Event.Types.BPMChange) {
                throw new Exception("Unimplemented");
            }
            else if (eventType == Event.Types.NumKeysChange) {
                throw new Exception("Unimplemented");
            }
        }

        private void InstantiateNoteEvent(Chart chart, Event chartEvent, int notePos) {
            chartEvent.InitializeNote(notePos);
            chart.AddChartEvent(chartEvent);
            Debug.Log($"Instantiated note event: {chartEvent.ToString()}");
        }
    }
}
