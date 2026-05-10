using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StatusIcon : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IDeselectHandler, IPointerExitHandler
{
    [Header("Referencias")]
    public string description;
    public TextMeshProUGUI descriptionLabel;
    public GameObject selectionFX;

    // --- LA SOLUCIÓN MÁGICA ---
    // Variable estática: Se comparte entre TODOS los botones. 
    // Solo puede haber un "current" en todo el juego a la vez.
    private static StatusIcon currentActiveIcon;
    // --------------------------

    // JOYSTICK: Al seleccionar
    public void OnSelect(BaseEventData eventData)
    {
        ActivateMe();
    }

    // MOUSE: Al pasar por encima
    public void OnPointerEnter(PointerEventData eventData)
    {
        // OPCIONAL: Si quieres que el mouse también "Seleccione" el botón para poder darle click con el Joystick:
        // EventSystem.current.SetSelectedGameObject(this.gameObject);

        ActivateMe();
    }

    // JOYSTICK: Al irse
    public void OnDeselect(BaseEventData eventData)
    {
        // Solo me apago si sigo siendo yo el activo (para evitar parpadeos)
        if (currentActiveIcon == this) DeactivateMe();
    }

    // MOUSE: Al salir
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentActiveIcon == this) DeactivateMe();
    }

    // --- LÓGICA CENTRALIZADA ---

    void ActivateMe()
    {
        // 1. Si hay otro botón prendido que NO soy yo... ¡APÁGALO!
        if (currentActiveIcon != null && currentActiveIcon != this)
        {
            currentActiveIcon.DeactivateMe();
        }

        // 2. Ahora yo soy el Rey
        currentActiveIcon = this;

        // 3. Prendo mis gráficos
        if (descriptionLabel != null) descriptionLabel.text = description;
        if (selectionFX != null) selectionFX.SetActive(true);
    }

    void DeactivateMe()
    {
        if (selectionFX != null) selectionFX.SetActive(false);
        if (descriptionLabel != null) descriptionLabel.text = "";
        // Si yo era el activo, ahora no hay nadie (hasta que se seleccione otro)
        if (currentActiveIcon == this) currentActiveIcon = null;
    }
}
