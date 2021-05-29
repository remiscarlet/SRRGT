using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    private int samplesPlayed;

    private double trackStartTime = -1.0;

    public AudioClip hitSoundSFX;
    public AudioClip gameOverSFX;
    public AudioClip trackClip;
    public float volume = 0.3f;

    public static void PlayHitSound() {
        Debug.Log("+++");
        instance.sfxAudioSource.PlayOneShot(instance.hitSoundSFX, instance.volume);
    }


    void Awake() {
        instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length != 3) {
            throw new Exception("Hm? Not enough AudioSources on the AudioManager game object.");
        }
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];
        trackAudioSource = audioSources[2];

        trackAudioSource.clip = trackClip;
        Debug.Log($"trackClip: {trackClip}");
    }

    private bool isPlayingTrack = false;
    void ToggleTrackMusic() {
        isPlayingTrack = !isPlayingTrack;

        if (isPlayingTrack) {
            trackAudioSource.Play();
            Debug.Log("Playing trackAudioSource");
        } else {
            trackAudioSource.Pause();
            Debug.Log("Pausing trackAudioSource");
        }
    }

    void Update() {
        if (isPlayingTrack) {
            if (trackStartTime == -1.0) {
                trackStartTime = AudioSettings.dspTime;
            }
            Debug.Log($"dspTime is: {trackStartTime}");


        }

        if (Input.GetKeyDown(KeyCode.P)) {
            ToggleTrackMusic();
        }
    }

}
