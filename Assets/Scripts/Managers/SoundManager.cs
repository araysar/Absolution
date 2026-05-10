using System.Collections; // Necesario para las Corrutinas
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxAudioSource, musicAudioSource;
    
    // CORRECCIÓN 1: Tenías dos "instance" (una con minúscula y otra con mayúscula). 
    // Dejé solo la estándar con mayúscula.
    public static SoundManager instance; 

    public AudioClip bossFightMusic;
    public AudioClip winMusic;
    public AudioClip clickSfx;
    public AudioClip openCommonDoor;
    public AudioClip bossDoor;

    [Header("Referencias")]
    public AudioMixer mainMixer;

    public enum SoundChannel
    {
        SFX,
        Music,
    }

    // Variable para controlar la atenuación de la música en diálogos (1 = normal, 0.25 = bajito)
    private float musicDuckingMultiplier = 1f;
    private Coroutine duckingCoroutine;

    private void Awake()
    {
        // Ajustado para usar la 'Instance' con mayúscula
        if(instance != this && instance != null)
        {
            Destroy(this.gameObject); // Corrección: Destroy(this) solo borra el script, (this.gameObject) borra el clon entero.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        if(FindObjectOfType<GameManager>() != null)
        {
            GameManager.instance.DestroyEvent += Destroy;
            GameManager.instance.EndGameEvent += StopSong;
        }
    }

    public void PlaySound(SoundChannel channel, AudioClip clip, Transform position)
    {
        // Tu lógica actual está perfecta aquí
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
                if (clip == musicAudioSource.clip && musicAudioSource.isPlaying) break;

                musicAudioSource.clip = clip;
                if (clip == null) musicAudioSource.Stop();
                else musicAudioSource.Play();
                break;
        }
    }

    // --- NUEVAS FUNCIONES DE DIÁLOGO (FADE SUAVE) ---

    public void PauseChannels()
    {
        // Reducimos la música al 25% (dividido 4) en 0.5 segundos
        if (duckingCoroutine != null) StopCoroutine(duckingCoroutine);
        duckingCoroutine = StartCoroutine(FadeMusicMultiplier(0.25f, 0.5f));
    }

    public void UnPauseChannels()
    {
        // Restauramos la música al 100% en 0.5 segundos
        if (duckingCoroutine != null) StopCoroutine(duckingCoroutine);
        duckingCoroutine = StartCoroutine(FadeMusicMultiplier(1f, 0.5f));
    }

    private IEnumerator FadeMusicMultiplier(float targetMultiplier, float duration)
    {
        float time = 0;
        float startMultiplier = musicDuckingMultiplier;

        while (time < duration)
        {
            // Usamos unscaledDeltaTime por si el juego está en "Pausa" durante el diálogo (Time.timeScale = 0)
            time += Time.unscaledDeltaTime; 
            musicDuckingMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, time / duration);
            
            // Calculamos y aplicamos el volumen en tiempo real
            ApplyCurrentMusicVolume();
            
            yield return null;
        }

        musicDuckingMultiplier = targetMultiplier;
        ApplyCurrentMusicVolume();
    }

    // Función auxiliar que junta el volumen de las Opciones + el Multiplicador de Diálogo
    private void ApplyCurrentMusicVolume()
    {
        float currentLinear = GameSettings.MusicVolume * musicDuckingMultiplier;
        
        // CORRECCIÓN 2: Mathf.Max(..., 0.0001f) evita que Log10(0) tire un error que rompa el juego
        float db = Mathf.Log10(Mathf.Max(currentLinear, 0.0001f)) * 20f;
        mainMixer.SetFloat("MusicVol", db);
    }

    // --- MÉTODOS DE OPCIONES ---

    public void SetMusicVolume(float sliderValue)
    {
        GameSettings.MusicVolume = sliderValue; // 1. Guardamos la preferencia
        ApplyCurrentMusicVolume();              // 2. Aplicamos (respetando si hay un diálogo activo)
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
        mainMixer.SetFloat("SFXVol", dbValue);
        GameSettings.SFXVolume = sliderValue;
    }

    // ... (Tus otras funciones StopSong, CurrentSong, Destroy) ...
    public void StopSong()
    {
        musicAudioSource.Stop();
        sfxAudioSource.Stop();
    }

    public AudioClip CurrentSong()
    {
        return musicAudioSource.clip;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}