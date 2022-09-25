using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : MonoBehaviour
{
    [SerializeField] private GameObject blockedEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile collider = collision.GetComponent<Projectile>();
        if (collider != null)
        {
            collider.Impact(true);
            collider.myPool.AddToPool(collision.gameObject);
        }
    }

    private void Impact()
    {
        if(blockedEffect != null)
        {
            blockedEffect.SetActive(false);
            blockedEffect.SetActive(true);
        }
    }
}
