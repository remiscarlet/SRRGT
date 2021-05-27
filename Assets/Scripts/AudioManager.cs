using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    private static AudioManager instance;

    private AudioSource sfxAudioSource;
    private AudioSource bgmAudioSource;

    public AudioClip hitSoundSFX;
    public AudioClip gameOverSFX;

    public AudioClip chartMusic;

    public float volume = 0.5f;

    void Awake() {
        instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];

        bgmAudioSource.clip = chartMusic;
    }

    public static void PlayHitSound() {
        Debug.Log("+++");
        instance.sfxAudioSource.PlayOneShot(instance.hitSoundSFX, instance.volume);
    }

    private bool isPlayingTrack = false;
    void ToggleTrackMusic() {
        isPlayingTrack = !isPlayingTrack;

        if (isPlayingTrack) {
            bgmAudioSource.Play();
        } else {
            bgmAudioSource.Pause();
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            ToggleTrackMusic();
        }
    }

}
