using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private int myTargetLayer = 6;
    private bool playerOnBounds = false;
    private Health myHealth;
    private Action PlayerOnBounds = delegate { };

    private void Start()
    {
        myHealth = GetComponentInParent<Health>();
    }
    private void Update()
    {
        PlayerOnBounds();
    }

    private void OnBounds()
    {
        if(!GameManager.instance.onPause)
        {
            if (myHealth != null)
            {
                if (myHealth.currentHP <= 0)
                {
                    return;
                }
            }

            Collider2D[] allObjectives = Physics2D.OverlapBoxAll(GetComponent<BoxCollider2D>().bounds.center, bounds, 0);
            foreach (var item in allObjectives)
            {
                if (item.GetComponent<IDamageable>() != null && item.gameObject.layer == myTargetLayer)
                {
                    item.GetComponent<IDamageable>().TakeDamage(damage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerOnBounds = OnBounds;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerOnBounds = delegate { };
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(GetComponent<BoxCollider2D>().bounds.center, bounds);
    }
}
