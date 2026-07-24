using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trap_CircularSaw : MonoBehaviour
{
    [Header("Configuración de Daño Fijo")]
    // Cantidad de daño "chunk" (de golpe) que hace la sierra
    [SerializeField] private float damageAmount = 15f;

    // Usamos OnTriggerEnter2D para daño inmediato al contacto
    private void OnTriggerStay2D(Collider2D other)
    {
        // INTENTAMOS OBTENER LA INTERFAZ IDAMAGEABLE
        if (other.TryGetComponent<Player_Health>(out Player_Health target))
        {
            // APLICAMOS EL DAÑO FIJO DE UNA SOLA VEZ
            target.TakeDamage(damageAmount);

            // Aquí puedes agregar efectos visuales/sonoros de "impacto"
            // Ej: SoundManager.instance.PlaySawHitSound();
        }
    }
}