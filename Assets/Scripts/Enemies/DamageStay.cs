using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    Health myHealth;

    [SerializeField] private bool spikes = false;
    private void Start()
    {
        myHealth = GetComponentInParent<Health>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" & !spikes)
        {
            if(myHealth.currentHP > 0)
            collision.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.tag == "Player" && spikes)
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(9999);
        }
    }
}
