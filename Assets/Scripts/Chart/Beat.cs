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

        public double? OrigPlayTime { // Only set if ApproximateBeatFromPlayTime() is called AND we are able to approximate with confidence
            private set;
            get;
        }

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
            this.PlayTime = (playTime < 0.0) ? 0.0 : playTime;
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
        /// Returns the beat num as a float.
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
                       this.PlayTime == other.playTime
                   ) ||
                   (
                       this.beatNum == other.beatNum &&
                       this.beatSubdiv == other.beatSubdiv &&
                       this.beatSubdivIdx == other.beatSubdivIdx
                   );
        }

        private const double MinOffsetEpsilonPercent = 0.25; // Any timing inaccuracy smaller than epsilon*subdivdur is ignored
        public void ApproximateBeatFromPlayTime(List<int> validBeatSubdivs) {
            if (playTime == null) {
                throw new Exception("Cannot convert playtime to beat if playtime is null!");
            }

            if (chart.BPMChanges.Count == 0) {
                throw new Exception("Chart must have at least one BPM event to denote the initial BPM.");
            }

            Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++");

            int approxBeatNum, approxBeatSubdiv, approxBeatSubdivIdx;
            double secsPerBeat = 0.0; // Eh...? Can cause divbyzero exceptions below but might be better than say -1.0?
            double beatsCounted = 0.0;
            double time = 0.0;

            double numBeatsWithBPM;
            int currBPM = -1;
            foreach (Event bpmChange in chart.BPMChanges) {
                if (currBPM == -1) {
                    currBPM = bpmChange.BPM;
                    if (chart.BPMChanges.Count == 1) {
                        secsPerBeat = 60.0 / currBPM;
                        numBeatsWithBPM = Math.Floor((double) playTime / secsPerBeat);
                        time += secsPerBeat * numBeatsWithBPM;
                        beatsCounted += numBeatsWithBPM;
                        Debug.Log($"+++ Calculating one-bpmchange chart. playTime:{playTime}, numBeatsWithBPM:{numBeatsWithBPM}, secsPerBeat:{secsPerBeat}, time:{time}");
                    }

                    continue;
                }

                if (bpmChange.Beat.AsFloat() > AsFloat()) {
                    // The bpm change we're looking at is in the "future" relative to this Beat.
                    break;
                }

                numBeatsWithBPM = Math.Min(AsFloat(), bpmChange.Beat.AsFloat()) - beatsCounted;

                secsPerBeat = 60.0 / currBPM;
                time += secsPerBeat * numBeatsWithBPM;
                beatsCounted += numBeatsWithBPM;
                currBPM = bpmChange.BPM;
            }


            double remTime = (double) playTime - time;
            Debug.Log($"BEAT APPROX MID CALC: beatsCounted:{beatsCounted}, time:{time}, remTime: {remTime}");
            if (remTime < MinOffsetEpsilonPercent * secsPerBeat) {
                approxBeatNum = (int) Math.Round(beatsCounted);
                approxBeatSubdiv = 1;
                approxBeatSubdivIdx = 0;
            } else {
                double subdivDur;
                secsPerBeat = 60.0 / currBPM;

                List<KeyValuePair<int, double>> subdivDivisibility = new List<KeyValuePair<int, double>>();
                foreach (int subdiv in validBeatSubdivs) {
                    // Calculate the "divisibility" of each subdiv by checking how "far" from the subdiv beat
                    // our remTime would be for each valid subdiv.
                    subdivDur = secsPerBeat / subdiv;
                    double subdivRemainder = remTime % subdivDur;
                    subdivDivisibility.Add(new KeyValuePair<int, double>(subdiv, subdivRemainder));
                    Debug.Log($"Subdiv:{subdiv}, subdivDur:{subdivDur}, subdivRemainder:{subdivRemainder} - remTime:{remTime}, secsPerBeat:{secsPerBeat}");
                }

                int closestSubdiv = 1;
                double offsetForClosestSubdiv = -1.0;
                bool wasApproximated = false;
                foreach (KeyValuePair<int, double> subdivData in subdivDivisibility) {
                    int subdiv = subdivData.Key;
                    double subdivOffset = subdivData.Value;
                    subdivDur = secsPerBeat / subdiv;

                    if (offsetForClosestSubdiv == -1.0 || offsetForClosestSubdiv > subdivOffset) {
                        closestSubdiv = subdiv;
                        offsetForClosestSubdiv = subdivOffset;

                        if (0.0 < offsetForClosestSubdiv && offsetForClosestSubdiv < MinOffsetEpsilonPercent * subdivDur) {
                            // If less than zero, subdiv is "in the future". Eg, subdiv is the 'and' of the beat but the true timing is the 'e' of the beat.
                            Debug.Log($"Calculated offset for subdiv:{subdiv} as offset:{subdivOffset} which was under min:{MinOffsetEpsilonPercent * subdivDur}");
                            wasApproximated = true;
                            break;
                        }
                    }
                }

                if (!wasApproximated) {
                    // We don't have particularly high confidence that our approximation is correct. Don't approximate.
                    Debug.Log("##### COULD NOT APPROXIMATE BEAT. ABORTING.");
                    return;
                }

                subdivDur = secsPerBeat / closestSubdiv;
                // TODO: Submetric bpm changes will get screwy with this Floor().
                approxBeatNum = (int) Math.Floor(beatsCounted);
                approxBeatSubdiv = closestSubdiv;
                approxBeatSubdivIdx = (closestSubdiv == 1) ? 0 : (int) Math.Round(remTime / subdivDur);
                Debug.Log($"Approximated vals... Subdiv:{approxBeatSubdiv}, subdivIdx: Math.Round(remTime:{remTime} / subdivDur:{subdivDur})");
                // TODO: Can get things like "subdiv:4, subdivIdx:2" which is equiv to "subdiv:2, idx:1". Should simplify where possible.
            }

            Debug.Log($"APPROXIMATING BEAT: Playtime: {playTime} - APPROX-BEATNUM:{approxBeatNum}, SUBDIV:{approxBeatSubdiv}, IDX:{approxBeatSubdivIdx}");
            this.beatNum = approxBeatNum;
            this.beatSubdiv = approxBeatSubdiv;
            this.beatSubdivIdx = approxBeatSubdivIdx;
            this.OrigPlayTime = this.playTime;
            this.playTime = null;
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
