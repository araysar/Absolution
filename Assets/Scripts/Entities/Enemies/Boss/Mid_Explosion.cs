using UnityEngine;

public class Mid_Explosion : MonoBehaviour
{
    public float damage = 35;
    bool alreadyHit = false;

    private void OnEnable()
    {
        alreadyHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyHit) return;

        Player_Health player = collision.GetComponent<Player_Health>();

        if (collision.GetComponent<Player_Health>() != null)
        {
            player.TakeDamage(damage);
            alreadyHit = true;
        }
    }
}
