using UnityEngine;

[RequireComponent(typeof(ParticleSystemRenderer))]
public class ParticleFader : MonoBehaviour
{
    [Header("Referencia al Canvas Group del Menú")]
    public CanvasGroup menuCanvasGroup;

    private ParticleSystemRenderer psRenderer;
    private Color originalColor;
    private string colorProperty = "_TintColor"; // La propiedad estándar de shaders Legacy

    void Start()
    {
        psRenderer = GetComponent<ParticleSystemRenderer>();

        // 1. Guardamos el color original del material para no perderlo
        // Nota: Al acceder a .material (y no .sharedMaterial) Unity crea una instancia única 
        // para este objeto, así no rompes otros prefabs.
        if (psRenderer.material.HasProperty("_TintColor"))
        {
            colorProperty = "_TintColor";
        }
        else if (psRenderer.material.HasProperty("_Color"))
        {
            colorProperty = "_Color";
        }

        originalColor = psRenderer.material.GetColor(colorProperty);
    }

    void Update()
    {
        if (menuCanvasGroup == null) return;

        // 2. Calculamos el nuevo color basado en el Alpha del grupo
        // En modo Additive: Bajar el Alpha oscurece la partícula, 
        // pero multiplicar el color por el alpha es más seguro visualmente.
        float alpha = menuCanvasGroup.alpha;

        Color newColor = originalColor;

        // Multiplicamos el Alpha original por el Alpha del Canvas
        newColor.a *= alpha;

        // TRUCO PARA ADDITIVE: 
        // En shaders aditivos, el Negro es transparente. 
        // Así que también oscurecemos el color RGB para que desaparezca suavemente.
        newColor.r *= alpha;
        newColor.g *= alpha;
        newColor.b *= alpha;

        // 3. Aplicamos el color al material
        psRenderer.material.SetColor(colorProperty, newColor);
    }
}
