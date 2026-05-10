using UnityEngine;
using UnityEngine.EventSystems; // ¡Necesario para controlar la selección!

public class MenuStartupSelection : MonoBehaviour
{
    [Header("Arrastra aquí el botón/slider inicial")]
    public GameObject primerBoton;

    // Usamos OnEnable en lugar de Start.
    // ¿Por qué? Porque 'Start' solo ocurre una vez.
    // 'OnEnable' ocurre CADA VEZ que abres (activas) este menú.
    private void OnEnable()
    {
        SelectButton();
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
}
