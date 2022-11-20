using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxAudioSource, musicAudioSource, unscalledAudioSource;

    public static SoundManager instance;
    public AudioClip winMusic;

    public float sfxVolume = 0.2f;
    public float musicVolume = 0.2f;

    public enum SoundChannel
    {
        SFX,
        Music,
        Unscalled,
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

        sfxAudioSource.volume = sfxVolume;
        musicAudioSource.volume = musicVolume;
        unscalledAudioSource = sfxAudioSource;
    }

    private void Start()
    {
        GameManager.instance.DestroyEvent += Destroy;
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
                if (clip == null) musicAudioSource.Stop();
                else musicAudioSource.Play();
                break;
            case SoundChannel.Unscalled:
                unscalledAudioSource.PlayOneShot(clip);
                break;
        }
    }

    public void StopSong()
    {
        musicAudioSource.Stop();
    }

    public AudioClip CurrentSong()
    {
        return musicAudioSource.clip;
    }

    public void PauseChannels()
    {
        sfxAudioSource.volume = sfxVolume / 4;
        musicAudioSource.volume = musicVolume / 4;
    }

    public void UnPauseChannels()
    {
        sfxAudioSource.volume = sfxVolume;
        musicAudioSource.volume = musicVolume;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}