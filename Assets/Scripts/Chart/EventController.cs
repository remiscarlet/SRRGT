using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.UI;
using UnityEngine;

namespace Chart {
    public class EventController : MonoBehaviour {
        protected ScoreManager ScoreManager;
        protected NotesManager NotesManager;

        // Start is called before the first frame update
        void Start() {
            ScoreManager = ReferenceManager.ScoreManager;
            NotesManager = ReferenceManager.NotesManager;
        }

        protected double targetTime = -1.0;
        protected double spawnedTime = -1.0;
        protected float spawnZOffset = -1.0f;

        public void InitializeTargetTime(double targetTime) {
            this.targetTime = targetTime;
            this.spawnedTime = ReferenceManager.AudioManager.CurrTrackTime;
            this.spawnZOffset = transform.position.z;
            //Debug.Log(
            //    $"Initializing ChartEventController with: targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}");
        }

        public double TimeFromTarget() {
            return Math.Abs(ReferenceManager.AudioManager.CurrTrackTime - targetTime);
        }

        public new string ToString() {
            return
                $"ChartEventController[targetTime:{targetTime}, spawnedTime:{spawnedTime}, spawnZOffset:{spawnZOffset}]";
        }

        // Update is called once per frame
        protected void Update() {
            if (targetTime == -1.0) {
                return;
            }

            UpdatePosition();
        }

        public int minZPos = -3;

        protected bool IsOutOfBounds() {
            return transform.position.z <= minZPos;
        }

        protected float CalculateZPos(double percentToPlayTime) {
            int sign = percentToPlayTime >= 0.0 ? +1 : -1;
            float distPercent = (float) Math.Pow(Math.Abs(percentToPlayTime), 1.25);
            return
                sign * spawnZOffset *
                distPercent; // spawnZOffset being a float means we can convert down to a float cuz least accurate
        }

        protected void UpdatePosition() {
            // TODO: Make movement non-linear? Eg, move faster the farther away note is from judgement line. Maffs.
            if (ReferenceManager.AudioManager.IsPlayingTrack) {
                if (targetTime == -1.0) {
                    throw new Exception($"Tried updating note event position before initializing! {ToString()}");
                }

                double now = ReferenceManager.AudioManager.CurrTrackTime;
                double timeToPlayTime = now - spawnedTime;

                double percentToPlayTime = 1 - timeToPlayTime / (targetTime - spawnedTime);

                float zPos = CalculateZPos(percentToPlayTime);
                //Debug.Log($"Calculated zPos update to be: {zPos} - percentToPlayTime:{percentToPlayTime}");

                Vector3 newPos = new Vector3(transform.position.x, transform.position.y, zPos);
                // Debug.Log($"Moving note {this} to newPos: {newPos} with zPos:{zPos}, pcntToPlayTime:{percentToPlayTime}, timeToPlayTime:{timeToPlayTime}, spawnedTime:{spawnedTime}, targetTime:{targetTime}, spawnZOffset:{spawnZOffset}");
                transform.position = newPos;
            }
        }
    }
}
