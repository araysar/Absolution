using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("El objeto padre que contiene todos los botones, fondos e inventario.")]
    public GameObject menuContainer;

    [Tooltip("La imagen negra que cubre toda la pantalla y tiene el material con el Shader.")]
    public Image transitionImage;

    [Header("Configuración")]
    [Tooltip("Qué tan rápido se mueven los rombos.")]
    public float transitionSpeed = 3f;

    private Material _transMaterial;
    private bool _isPaused = false;
    private bool _isAnimating = false; // Bloqueo para no romper la animación

    void Start()
    {
        // 1. Instanciamos el material para no modificar el archivo original del proyecto
        if (transitionImage != null)
        {
            _transMaterial = Instantiate(transitionImage.material);
            transitionImage.material = _transMaterial;

            // Empezamos con la transición "abierta" (transparente, Cutoff en 0)
            _transMaterial.SetFloat("_Cutoff", -0.1f);
        }
        else
        {
            Debug.LogError("¡Falta asignar la Transition Image en el inspector!");
        }

        // 2. Aseguramos que el menú arranque apagado
        if (menuContainer != null) menuContainer.SetActive(false);
    }

    void Update()
    {
        // Si ya estamos animando, ignoramos cualquier input
        if (_isAnimating) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
        {
            if (_isPaused) StartCoroutine(CloseMenuRoutine());
            else StartCoroutine(OpenMenuRoutine());
        }
    }

    // --- RUTINA DE APERTURA (Juego -> Menú) ---
    IEnumerator OpenMenuRoutine()
    {
        _isAnimating = true;

        // 1. PAUSA INMEDIATA: Congelamos el juego antes de que empiece a taparse la pantalla
        Time.timeScale = 0;
        _isPaused = true;

        // 2. FASE "IN" (Tapar pantalla con rombos)
        yield return StartCoroutine(AnimateCutoff(-0.1f, 1.1f));

        // 3. CAMBIO DE ESCENA (Detrás del telón)
        if (menuContainer != null) menuContainer.SetActive(true); // Prendemos el menú visualmente

        // Esperamos un frame real para asegurar que la UI se dibuje antes de destapar
        yield return new WaitForSecondsRealtime(0.02f);

        // 4. FASE "OUT" (Destapar pantalla para mostrar el menú)
        yield return StartCoroutine(AnimateCutoff(1.1f, -0.1f));

        _isAnimating = false;
    }

    // --- RUTINA DE CIERRE (Menú -> Juego) ---
    IEnumerator CloseMenuRoutine()
    {
        _isAnimating = true;

        // Nota: NO despausamos todavía. El jugador sigue en el menú (o saliendo de él).

        // 1. FASE "IN" (Tapar el menú con rombos)
        yield return StartCoroutine(AnimateCutoff(-0.1f, 1.1f));

        // 2. CAMBIO DE ESCENA (Detrás del telón)
        if (menuContainer != null) menuContainer.SetActive(false); // Apagamos el menú visualmente

        // 3. FASE "OUT" (Destapar pantalla para mostrar el juego)
        yield return StartCoroutine(AnimateCutoff(1.1f, -0.1f));

        // 4. DESPAUSA FINAL: Ahora que la pantalla está limpia y el jugador ve dónde está...
        Time.timeScale = 1;
        _isPaused = false;

        _isAnimating = false;
    }

    // --- HELPER PARA ANIMAR EL SHADER ---
    IEnumerator AnimateCutoff(float startValue, float endValue)
    {
        float t = 0;
        while (t < 1f)
        {
            // Usamos unscaledDeltaTime porque Time.timeScale podría ser 0
            t += Time.unscaledDeltaTime * transitionSpeed;
            float val = Mathf.Lerp(startValue, endValue, t);

            if (_transMaterial != null) _transMaterial.SetFloat("_Cutoff", val);

            yield return null;
        }
        // Aseguramos valor final exacto
        if (_transMaterial != null) _transMaterial.SetFloat("_Cutoff", endValue);
    }
}