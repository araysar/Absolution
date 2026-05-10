using UnityEngine;
using UnityEngine.EventSystems;

public class InputSwitcher : MonoBehaviour
{
    [Header("Configuración")]
    public float mouseThreshold = 2.0f; // Sensibilidad para detectar movimiento del mouse

    private Vector3 lastMousePos;
    private GameObject lastSelectedObject; // La "Memoria" del Joystick

    void Start()
    {
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        // --- 1. DETECTAR MOVIMIENTO DEL MOUSE ---
        if ((Input.mousePosition - lastMousePos).sqrMagnitude > mouseThreshold)
        {
            // Si el jugador mueve el mouse...
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // Guardamos qué estaba seleccionado antes de borrarlo
                lastSelectedObject = EventSystem.current.currentSelectedGameObject;

                // Limpiamos la selección para que no haya dos botones brillando
                EventSystem.current.SetSelectedGameObject(null);
            }

            // Hacemos visible el mouse y NO LO BLOQUEAMOS
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // ¡Esto arregla el teletransporte!

            lastMousePos = Input.mousePosition;
        }

        // --- 2. DETECTAR JOYSTICK / TECLADO ---
        // Si se presiona cualquier tecla o se mueve un eje
        if (Input.anyKeyDown || IsJoystickInput())
        {
            // Ocultamos el mouse
            Cursor.visible = false;

            // LA CLAVE: RECUPERACIÓN DE SELECCIÓN
            // Si el sistema no tiene nada seleccionado (porque el mouse lo borró)...
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (lastSelectedObject != null && lastSelectedObject.activeInHierarchy)
                {
                    // ... Restauramos el último botón conocido
                    EventSystem.current.SetSelectedGameObject(lastSelectedObject);
                }
                else
                {
                    // Si no hay memoria (ej: primer uso), buscamos el primer botón disponible en la pantalla
                    // Esto es un salvavidas por si acaso.
                    SelectFirstAvailable();
                }
            }
        }

        // --- 3. ACTUALIZAR MEMORIA CONTINUAMENTE ---
        // Si hay algo seleccionado actualmente, actualizamos la memoria
        // Así siempre sabemos cuál fue el último botón que tocó el Joystick
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
    }

    bool IsJoystickInput()
    {
        // Detectar si se mueven las palancas o flechas con fuerza suficiente
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f ||
               Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f;
    }

    void SelectFirstAvailable()
    {
        // Buscamos cualquier botón activo en el canvas como plan de emergencia
        // (Opcional, pero útil si se rompe todo)
        var buttton = FindObjectOfType<UnityEngine.UI.Button>();
        if (buttton != null) EventSystem.current.SetSelectedGameObject(buttton.gameObject);
    }
}