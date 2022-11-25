using UnityEngine;

public class Character_Tornado : MonoBehaviour
{
    [SerializeField] private float damagePerSecond;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null && collision.gameObject.tag != "Player")
        {
            collision.GetComponent<IDamageable>().TakeDamage(
            collision.GetComponent<Health>().myType == Health.EntityType.boss? (damagePerSecond / 3) / 60 : damagePerSecond / 60);
        }
    }
}