using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup menuCanvasGroup;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    public GameObject primerBoton;
    private GameObject previousSelection;

    [Header("Audio")]
    public AudioMixer mainMixer; // ¡No olvides arrastrar el Mixer aquí!

    [Header("Configuración")]
    public float fadeDuration = 0.5f;

    void Start()
    {
        // 1. Cargar valores desde tu clase estática
        float masterVol = GameSettings.MasterVolume;
        float musicVol = GameSettings.MusicVolume;
        float sfxVol = GameSettings.SFXVolume;

        // 2. Poner los sliders en esa posición visualmente (¡EN SILENCIO!)
        masterSlider.SetValueWithoutNotify(masterVol * 10);
        musicSlider.SetValueWithoutNotify(musicVol * 10);
        sfxSlider.SetValueWithoutNotify(sfxVol * 10);

        // 3. Aplicar el volumen real al Mixer
        SetMixerVolume("MasterVol", masterVol);
        SetMixerVolume("MusicVol", musicVol);
        SetMixerVolume("SFXVol", sfxVol);

        // 4. Suscribirse a los cambios
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    // --- TRANSICIONES (FADE IN / FADE OUT) ---

    public void OpenMenu()
    {
        if (EventSystem.current != null)
        {
            previousSelection = EventSystem.current.currentSelectedGameObject;
        }

        StartCoroutine(FadeCanvas(0, 1)); // De transparente a opaco
    }

    public void CloseMenu()
    {
        if (previousSelection != null && previousSelection.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);
        }
        StartCoroutine(FadeCanvas(1, 0, true)); // De opaco a transparente + apagar
    }

    void SelectButton()
    {
        if (primerBoton != null)
        {
            // TRUCO DE ORO:
            // A veces Unity se confunde si ya había algo seleccionado.
            // Primero limpiamos la selección (poniéndola en null)
            EventSystem.current.SetSelectedGameObject(null);

            // Y ahora sí, forzamos la selección de nuestro botón
            EventSystem.current.SetSelectedGameObject(primerBoton);
        }
    }

    IEnumerator FadeCanvas(float startAlpha, float endAlpha, bool disableOnFinish = false)
    {
        float time = 0;


        // Habilitamos interacción al empezar a abrir
        if (!disableOnFinish)
        {
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
            SelectButton();
        }

        while (time < fadeDuration)
        {
            // Usamos unscaledDeltaTime para que funcione aunque el juego esté en Pausa (Time.timeScale = 0)
            time += Time.unscaledDeltaTime;
            menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            yield return null;
        }

        if(disableOnFinish)
        {
            menuCanvasGroup.interactable = false; // Evitar clicks mientras se cierra
            menuCanvasGroup.blocksRaycasts = false;
        }

        menuCanvasGroup.alpha = endAlpha;
    }

    // --- FUNCIONES QUE LLAMAN LOS SLIDERS ---

    public void OnMasterChanged(float val)
    {
        // val ahora llega del 0 al 10. Lo DIVIDIMOS para que vuelva a ser 0.0 - 1.0
        float realVol = val / 10f;

        GameSettings.MasterVolume = realVol;
        SetMixerVolume("MasterVol", realVol);
    }

    public void OnMusicChanged(float val)
    {
        float realVol = val / 10f;
        GameSettings.MusicVolume = realVol;
        SetMixerVolume("MusicVol", realVol);
    }

    public void OnSFXChanged(float val)
    {
        float realVol = val / 10f;
        GameSettings.SFXVolume = realVol;
        SetMixerVolume("SFXVol", realVol);
    }

    // --- LA MAGIA MATEMÁTICA ---
    void SetMixerVolume(string paramName, float sliderValue)
    {
        // Convertimos 0-1 a Decibeles (-80 a 0)
        // Usamos Mathf.Max(0.0001) para que Log10 nunca de error infinito
        float db = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;

        mainMixer.SetFloat(paramName, db);
    }

    // ... Aquí abajo irían tus funciones OpenMenu() y CloseMenu() con Fade ...
    // ... que ya tenías en la respuesta anterior ...
}
