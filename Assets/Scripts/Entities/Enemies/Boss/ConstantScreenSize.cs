using UnityEngine;

public class ConstantScreenSize : MonoBehaviour
{
    [Header("Configuración Base")]
    [Tooltip("El Orthographic Size normal de tu cámara (ej: 5 o 7)")]
    [SerializeField] private float referenceOrthoSize = 6f;

    [Tooltip("El Scale (X,Y,Z) que tiene este objeto cuando se ve bien en la cámara normal")]
    [SerializeField] private Vector3 referenceScale = Vector3.one;

    private Camera mainCam;

    private void Start()
    {
        // Buscamos la cámara principal automáticamente
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (mainCam != null)
        {
            // Calculamos la proporción de diferencia. 
            // Si la cámara hace zoom in (ej: pasa de 5 a 2.5), el factor será 0.5.
            float scaleFactor = mainCam.orthographicSize / referenceOrthoSize;

            // Multiplicamos nuestra escala base por ese factor.
            // Así, el objeto se achica físicamente en el mundo para verse del mismo tamaño en la pantalla.
            transform.localScale = referenceScale * scaleFactor;
        }
    }
}