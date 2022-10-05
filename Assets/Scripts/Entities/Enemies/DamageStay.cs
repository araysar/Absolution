using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private int myTargetLayer = 6;
    private bool playerOnBounds = false;
    private Health myHealth;

    private void Start()
    {
        myHealth = GetComponentInParent<Health>();
    }
    private void Update()
    {
        if(playerOnBounds)
        {
            if(myHealth != null)
            {
                if(myHealth.currentHP <= 0)
                {
                    return;
                }
            }

            Collider2D[] allObjectives = Physics2D.OverlapBoxAll(transform.position, bounds, 0);
            foreach (var item in allObjectives)
            {
                if(item.GetComponent<IDamageable>() != null && item.gameObject.layer == myTargetLayer)
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
            playerOnBounds = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerOnBounds = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, bounds);
    }
}
