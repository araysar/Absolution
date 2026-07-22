using System.Collections;
using UnityEngine;

public class LifePotion : MonoBehaviour
{
    [SerializeField] private float healAmount;
    public bool isDrop = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player_Health>() != null)
        {
            collision.gameObject.GetComponent<Player_Health>().Heal(healAmount);
            if(!isDrop)GameManager.instance.EnemyRespawnEvent += Respawn;

            gameObject.SetActive(false);
        }
    }

    public void Destroy()
    {
        StartCoroutine(DestroyThis());
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(10);
        gameObject.SetActive(false);
    }
    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
