using UnityEngine;

public class ImpactSoundLimiter : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private AudioClip destroySfx;
    // Si bajas esto a 0.05f, sonará más "metralleta". 
    // Si lo subes a 0.1f, sonará más limpio (solo un golpe).
    public float minTimeBetweenSounds = 0.1f;

    // ESTÁTICO: Esta memoria se comparte entre TODOS los hielos del juego.
    private static float lastImpactTime = -1f;

    public void PlaySound()
    {
        // Preguntamos al reloj global: ¿Pasó suficiente tiempo?
        float currentTime = Time.time;

        if (currentTime - lastImpactTime >= minTimeBetweenSounds)
        {
            // SI: Reproducir
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, destroySfx, transform);

            // Actualizamos la última vez que sonó CUALQUIER hielo
            lastImpactTime = currentTime;
        }
        else
        {
            // NO: Silencio absoluto. Evitamos la saturación.
        }
    }
}