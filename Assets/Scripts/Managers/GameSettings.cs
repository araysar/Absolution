using UnityEngine;

public static class GameSettings
{
    // --- VOLUMEN MAESTRO ---
    public static float MasterVolume
    {
        // CAMBIO: Default 0.5f (La mitad)
        get => PlayerPrefs.GetFloat("MasterVol", 0.5f);
        set
        {
            PlayerPrefs.SetFloat("MasterVol", Mathf.Clamp(value, 0.0001f, 1f));
            PlayerPrefs.Save();
            // Nota: Ya no modificamos AudioListener aqui, lo haremos en el script del Menú 
            // para centralizar la lógica del Mixer.
        }
    }

    // --- MÚSICA ---
    public static float MusicVolume
    {
        // CAMBIO: Default 0.5f
        get => PlayerPrefs.GetFloat("MusicVol", 0.5f);
        set
        {
            PlayerPrefs.SetFloat("MusicVol", Mathf.Clamp(value, 0.0001f, 1f));
            PlayerPrefs.Save();
        }
    }

    // --- SFX ---
    public static float SFXVolume
    {
        // CAMBIO: Default 0.5f
        get => PlayerPrefs.GetFloat("SFXVol", 0.5f);
        set
        {
            PlayerPrefs.SetFloat("SFXVol", Mathf.Clamp(value, 0.0001f, 1f));
            PlayerPrefs.Save();
        }
    }
}