using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    private static AudioManager instance;

    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip gameOver;

    public float volume = 0.5f;

    void Awake() {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayHitSound() {
        Debug.Log("+++");
        instance.audioSource.PlayOneShot(instance.hitSound, instance.volume);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
