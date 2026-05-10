using UnityEngine;
using UnityEngine.UI;

public class AutoScrollTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.05f;

    private RawImage rawImage;
    private Rect uvRect;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        // Movemos las coordenadas UV (la textura se desliza dentro del marco)
        uvRect.x += scrollSpeedX * Time.unscaledDeltaTime; // Usamos unscaled por si pausas el juego
        uvRect.y += scrollSpeedY * Time.unscaledDeltaTime;

        // Aplicamos el cambio
        rawImage.uvRect = uvRect;
    }
}