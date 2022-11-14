using UnityEngine;

public class TopTornado : MonoBehaviour
{
    [SerializeField] private float damagePerTick = 20;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() != null && collision.gameObject.tag == "Player")
        {
            collision.GetComponent<IDamageable>().TakeDamage(damagePerTick);
        }
    }
}
