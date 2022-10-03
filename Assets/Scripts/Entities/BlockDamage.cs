using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour
{
    [SerializeField] private GameObject blockedEffect;
    [SerializeField] private Health myHealth;

    private void Start()
    {
        if (myHealth == null) GetComponentInParent<Health>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile collider = collision.GetComponent<Projectile>();
        if (collider != null && myHealth.currentHP > 0)
        {
            collider.Impact(true);
            collider.myPool.AddToPool(collision.gameObject);
        }
    }

    private void Impact()
    {
        if(blockedEffect != null && myHealth.currentHP > 0)
        {
            blockedEffect.SetActive(false);
            blockedEffect.SetActive(true);
        }
    }
}
