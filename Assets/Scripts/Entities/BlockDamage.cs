using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour
{
    [SerializeField] private Health myHealth;
    [SerializeField] private bool noHealth = false;
    private void Start()
    {
        if (myHealth == null)
        {
            GetComponentInParent<Health>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile collider = collision.GetComponent<Projectile>();
        if (collider != null && (myHealth.currentHP > 0 || noHealth))
        {

        }
    }
}
