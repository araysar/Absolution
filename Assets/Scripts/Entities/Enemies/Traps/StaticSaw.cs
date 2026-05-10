using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSaw : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastrá acá el objeto hijo que tiene el Sprite de la sierra")]
    public Transform visualTransform;

    [Header("Configuración del Temblor")]
    public float intensidad = 0.05f; // Qué tan lejos se mueve el sprite
    public float velocidad = 30f;    // Qué tan frenético es el motor fallando

    private Vector3 posicionOriginalLocal;
    private float offsetAleatorioX;
    private float offsetAleatorioY;

    void Start()
    {
        if (visualTransform != null)
        {
            // Guardamos la posición central del sprite para que no se vaya volando
            posicionOriginalLocal = visualTransform.localPosition;

            // Elegimos un punto de inicio aleatorio en el "mapa" del ruido
            // para que si ponés 5 sierras rotas, no tiemblen todas en perfecta sincronía
            offsetAleatorioX = Random.Range(0f, 100f);
            offsetAleatorioY = Random.Range(0f, 100f);
        }
    }

    void Update()
    {
        if (visualTransform == null) return;

        // Calculamos el temblor errático usando Perlin Noise
        // Multiplicamos por 2 y restamos 1 para que el valor vaya de -1 a 1 (y no de 0 a 1)
        float ruidoX = (Mathf.PerlinNoise(Time.time * velocidad + offsetAleatorioX, 0f) * 2f) - 1f;
        float ruidoY = (Mathf.PerlinNoise(0f, Time.time * velocidad + offsetAleatorioY) * 2f) - 1f;

        // Aplicamos el temblor sumándolo a la posición base del sprite
        Vector3 temblor = new Vector3(ruidoX, ruidoY, 0f) * intensidad;
        visualTransform.localPosition = posicionOriginalLocal + temblor;
    }
}