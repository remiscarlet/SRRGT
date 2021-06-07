using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {
    private static AudioManager instance;
    private AudioSource sfxAudioSource; // Sound effects
    private AudioSource bgmAudioSource; // "BGM" that isn't track music - ie menus
    private AudioSource trackAudioSource; // Track/Chart music for game

    private const double defaultTrackStartTime = -2.0;
    public double CurrTrackTime { private set; get; } = defaultTrackStartTime; // 2 second lead time. TODO: Make configurable

    private double lastTrackSnapshot = -1.0;

    public AudioClip hitSoundSFX;
    public AudioClip gameOverSFX;
    public AudioClip trackClip;
    public float volume = 0.3f;

    public static void PlayHitSound() {
        // TODO: Account for multiple notes playing at same time causing additive volume? (Is louder with multistop notes)
        Debug.Log("+++");
        instance.sfxAudioSource.PlayOneShot(instance.hitSoundSFX, instance.volume);
    }

    public static double ConvertTimeToBeatAsDouble(double time, int bpm) {
        double secPerBeat = 60.0 / bpm;
        return time / secPerBeat + 1; // + 1 because 0-index vs 1-index (the 1st beat is at time 0.0)
    }

    public static double ConvertBeatToTimeAsDouble(int beatNum, int bpm) {
        double secPerBeat = 60.0 / bpm;
        return (beatNum - 1) * secPerBeat;
    }

    void Awake() {
        instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length != 3) {
            throw new Exception("Hm? Not enough/too many AudioSources on the AudioManager game object.");
        }
        trackAudioSource = audioSources[0];
        sfxAudioSource = audioSources[1];
        bgmAudioSource = audioSources[2];

        trackAudioSource.clip = trackClip;
    }

    public bool IsPlayingTrack { get; private set; } = false;
    public void ToggleTrackMusic() {
        if (!IsPlayingTrack && ReferenceManager.GameplayManager.CurrChart == null) {
            throw new Exception("Tried to toggle music state while CurrChart was null! Set CurrChart first. ");
        }

        Debug.Log("Toggling AudioManager.ToggleTrackMusic");
        IsPlayingTrack = !IsPlayingTrack;
    }

    public float GetTrackPercentage() {
        return (float) CurrTrackTime / trackAudioSource.clip.length;
    }

    public void RestartTrack() {
        CurrTrackTime = defaultTrackStartTime;
    }

    public void SeekTrack(double percent) {
        CurrTrackTime = trackAudioSource.clip.length * percent;
    }

    void Update() {
        if (IsPlayingTrack) {
            double now = AudioSettings.dspTime;
            if (lastTrackSnapshot == -1.0) {
                lastTrackSnapshot = now;
            }

            CurrTrackTime += now - lastTrackSnapshot;
            lastTrackSnapshot = now;
        } else {
            lastTrackSnapshot = -1.0;
        }

        // This is kind of ugly. Probably refactor eventually.
        // IsPlayingTrack is true when the gameplay "starts" including with the lead in time.
        // trackAudioSource.isPlaying is only true at the point CurrTrackTime >= 0.0.
        // In other words, IsPlayingTrack != trackAudioSource.isPlaying iff CurrTrackTime < 0.0
        if (IsPlayingTrack) {
            if (CurrTrackTime >= 0.0 && ! trackAudioSource.isPlaying) {
                // To account for lead time. How to make cleaner?
                trackAudioSource.Play();
                Debug.Log($"Playing trackAudioSource - {trackAudioSource} - {trackAudioSource.clip}");
            }
        } else if (trackAudioSource.isPlaying) {
            trackAudioSource.Pause();
            Debug.Log("Pausing trackAudioSource");
        }
    }
}
