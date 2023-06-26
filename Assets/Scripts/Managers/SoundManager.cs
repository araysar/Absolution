using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxAudioSource, musicAudioSource, unscalledAudioSource;
    [HideInInspector] public List<AudioSource> audioSources = new List<AudioSource>();
    [HideInInspector] public List<AudioSource> exAudioSources = new List<AudioSource>();
    public static SoundManager instance;
    public AudioClip winMusic;
    public AudioClip clickSfx;
    public AudioClip openCommonDoor;
    public AudioClip bossDoor;

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
        unscalledAudioSource.volume = sfxVolume;
    }

    private void Start()
    {
        GameManager.instance.DestroyEvent += Destroy;
        GameManager.instance.EndGameEvent += StopSong;
    }
    public void PlaySound(SoundChannel channel, AudioClip clip, Transform position)
    {
        switch (channel)
        {
            case SoundChannel.SFX:
                if (Vector2.Distance(position.position, GameManager.instance.player.transform.position) < 8 || GameManager.instance.fightingBoss)
                {
                    sfxAudioSource.PlayOneShot(clip);
                    break;
                }
                else break;

            case SoundChannel.Music:
                if (clip == musicAudioSource.clip) break;

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
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].Stop();
        }
    }

    public AudioClip CurrentSong()
    {
        return musicAudioSource.clip;
    }

    public void PauseChannels()
    {
        sfxAudioSource.volume = sfxVolume / 4;
        musicAudioSource.volume = musicVolume / 4;
        for (int i = 0; i < exAudioSources.Count; i++)
        {
            audioSources[i].volume = sfxVolume / 4;
        }
    }

    public void UnPauseChannels()
    {
        sfxAudioSource.volume = sfxVolume;
        musicAudioSource.volume = musicVolume;

        for (int i = 0; i < exAudioSources.Count; i++)
        {
            if(audioSources[i].gameObject.activeInHierarchy)
                audioSources[i].volume = sfxVolume;
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}