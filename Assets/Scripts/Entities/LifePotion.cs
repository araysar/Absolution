using UnityEngine;

public class LifePotion : MonoBehaviour
{
    [SerializeField] private float healAmount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Health>().Heal(healAmount);
            GameManager.instance.EnemyRespawnEvent += Respawn;
            gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
