using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    Health myHealth;
    private void Start()
    {
        myHealth = GetComponentInParent<Health>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(myHealth.currentHP > 0)
            collision.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
}
