using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar el puntero

public class AutoSelectOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Esta función se dispara AUTOMÁTICAMENTE cuando el mouse entra en el botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Le ordenamos al EventSystem: "Olvídate de lo que tenías, AHORA ESTE es el seleccionado"
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // Opcional: Cuando el mouse sale, ¿queremos deseleccionar?
    // Generalmente NO. En consolas, la selección se queda en el último botón tocado 
    // hasta que tocas otro. Así que dejamos esto vacío o lo borramos.
    public void OnPointerExit(PointerEventData eventData)
    {
        // No hacemos nada para que la selección se "pegue" al botón 
        // hasta que el mouse toque otro.
    }
}
