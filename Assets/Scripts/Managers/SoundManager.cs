using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudioSource, musicAudioSource, voiceAudioSource, ambienceAudioSource;

    public static SoundManager instance;

    public enum SoundChannel
    {
        SFX,
        Music,
        Voice,
        Ambience,
    };

    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void PlaySound(SoundManager.SoundChannel channel, AudioClip clip)
    {
        switch (channel)
        {
            case SoundChannel.SFX:
                sfxAudioSource.PlayOneShot(clip);
                break;
            case SoundChannel.Music:
                musicAudioSource.clip = clip;
                musicAudioSource.Play();
                break;
            case SoundChannel.Voice:
                voiceAudioSource.PlayOneShot(clip);
                break;
            case SoundChannel.Ambience:
                ambienceAudioSource.PlayOneShot(clip);
                break;
        }
    }
}