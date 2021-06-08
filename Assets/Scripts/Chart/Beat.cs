using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chart {
    public class Beat {
        /*
         * A beat assumes quarter notes as default.
         *
         * beatInt is the "whole number" beat. The 5th eighth note in a measure is still the "2nd beat" (+ half a beat)
         * beatSubdiv is for specifying submetric rhythm. What's the subdivision? subdiv of 2 is eigth notes, 3 eigth triplets, 4 sixteenth, etc
         * beatSubdivIdx is which exact beat of the submetric beat are we on based on above? Val of 1 is same as downbeat.
         *
         * Eg,
         * beatInt:1, beatSubdiv:4, beatSubdivIdx:3 == "3rd sixteenth note on the first beat"
         * beatInt:1, beatSubdiv:16, beatSubdivIdx:1 == "downbeat of 1st beat"
         * beatInt:4, beatSubdiv:3, beatSubdivIdx:2 == "2nd eighth triplet on beat 4"
         */
        private int beatNum;
        private int beatSubdiv;
        private int beatSubdivIdx;

        private Chart chart;
        private double? playTime;

        public Beat(Chart chart, int beatNum, int beatSubdiv, int beatSubdivIdx) {
            if (beatSubdivIdx > beatSubdiv) {
                throw new Exception(
                    $"Subdivision values cannot be greater than ratio of 1. Got: beatSubdiv:{beatSubdiv}, beatSubdivIdx:{beatSubdivIdx}");
            }

            this.chart = chart;
            this.beatNum = beatNum;
            this.beatSubdiv = beatSubdiv;
            this.beatSubdivIdx = beatSubdivIdx;
        }

        public Beat(Chart chart, double playTime) {
            this.chart = chart;
            this.playTime = playTime;
        }

        public Beat(double playTime) {
            this.playTime = playTime;
        }

        public double PlayTime {
            private set { playTime = value; }
            get {
                if (playTime != null) {
                    return (double) playTime;
                }

                return GetPlayTimeFromBeat();
            }
        }

        public string ToString() {
            return (playTime == null) ? $"Beat:{AsFloat()}" : $"Beat-playtime:{playTime}";
        }

        /// <summary>
        /// Returns the beat as a float.
        ///
        /// 0-index. Not 1-index
        ///
        /// Eg, the "and" of beat one would return 0.5
        /// Eg, the "e" of beat one would return 0.25
        /// Eg, the 2nd eighth note triplet in beat 3 of measure 2 in 4/4 time would be 6.666 etc etc
        /// </summary>
        /// <returns></returns>
        public double AsFloat() {
            if (beatSubdivIdx == 0) {
                return beatNum;
            }

            double beatPartial = (double) beatSubdivIdx / beatSubdiv;
            return beatNum + beatPartial;
        }

        public bool Equals(Beat other) {
            return (
                       this.playTime != null &&
                       this.playTime == other.playTime
                   ) ||
                   (
                        this.beatNum == other.beatNum &&
                        this.beatSubdiv == other.beatSubdiv &&
                        this.beatSubdivIdx == other.beatSubdivIdx
                   );
        }

        private const double MinOffsetEpsilon = 0.05; // Any timing inaccuracy smaller than epsilon is ignored
        public void ApproximateBeatFromPlayTime(List<int> validBeatSubdivs) {
            if (playTime == null) {
                throw new Exception("Cannot convert playtime to beat if playtime is null!");
            }
            int approxBeatNum, approxBeatSubdiv, approxBeatSubdivIdx;
            double beatsCounted = 0;
            double time = 0.0;

            int currBPM = -1;
            foreach (Event bpmChange in chart.BPMChanges) {
                // TODO: Sub metric tempo changes might screw with beatsCounted
                if (currBPM == -1) {
                    currBPM = bpmChange.BPM;
                    continue;
                }

                if (bpmChange.Beat.AsFloat() > AsFloat()) {
                    // The bpm change we're looking at is in the "future" relative to this ChartBeat.
                    break;
                }

                double numBeatsWithBPM = Math.Min(AsFloat(), bpmChange.Beat.AsFloat()) - beatsCounted;
                double secsInBeat = 60.0 / currBPM;
                time += secsInBeat * numBeatsWithBPM;
                beatsCounted += numBeatsWithBPM;
                currBPM = bpmChange.BPM;
            }

            double remTime = (double) playTime - time;
            if (remTime >= MinOffsetEpsilon) {
                double secsPerBeat = 60.0 / currBPM; // TODO: Clean way to not repeat declaration?
                double subdivDur;

                List<KeyValuePair<int, double>> subdivDivisibility = new List<KeyValuePair<int, double>>();
                foreach (int subdiv in validBeatSubdivs) {
                    // Calculate the "divisibility" of each subdiv by checking how "far" from the subdiv beat
                    // our remTime would be for each valid subdiv.
                    subdivDur = secsPerBeat / subdiv;
                    double subdivRemainder = remTime % subdivDur;
                    subdivDivisibility.Add(new KeyValuePair<int, double>(subdiv, subdivRemainder));
                }

                int closestSubdiv = 1;
                double offsetForClosestSubdiv = -1.0;
                foreach (KeyValuePair<int, double> subdiv in subdivDivisibility) {
                    if (offsetForClosestSubdiv == -1.0 || offsetForClosestSubdiv > subdiv.Value) {
                        closestSubdiv = subdiv.Key;
                        offsetForClosestSubdiv = subdiv.Value;
                    }
                }

                subdivDur = secsPerBeat / closestSubdiv;
                this.beatNum = (int) Math.Floor(beatsCounted);
                this.beatSubdiv = closestSubdiv;
                this.beatSubdivIdx = (closestSubdiv == 1) ? 1 : (int) Math.Round(remTime / subdivDur);
                this.playTime = null;
            }
        }

        private double GetPlayTimeFromBeat() {
            double time = 0.0;
            double beatsCounted = 0;

            int currBPM = -1;
            foreach (Event bpmChange in chart.BPMChanges) {
                if (currBPM == -1) {
                    currBPM = bpmChange.BPM;
                    continue;
                }

                if (bpmChange.Beat.AsFloat() > AsFloat()) {
                    // The bpm change we're looking at is in the "future" relative to this ChartBeat.
                    break;
                }

                double numBeatsWithBPM = Math.Min(AsFloat(), bpmChange.Beat.AsFloat()) - beatsCounted;
                double secsInBeat = 60.0 / currBPM;
                time += secsInBeat * numBeatsWithBPM;
                beatsCounted += numBeatsWithBPM;
                currBPM = bpmChange.BPM;
            }

            if (currBPM == -1) {
                throw new Exception(
                    "BPM was still set to -1. All charts must have a bpmChange event as its first event!");
            }

            if (beatsCounted < AsFloat()) {
                // We have some beats to count after the last bpm change.
                double secsInBeat = 60.0 / currBPM;
                double beatsToCount = AsFloat() - beatsCounted;
                time += secsInBeat * beatsToCount;
            }

            //Debug.Log(
            //    $"Calculated playtime from beat. Time:{time}, beat:{beatNum}, beatSubdiv:{beatSubdiv}, beatSubdivIdx:{beatSubdivIdx}");
            return time;
        }
    }
}
