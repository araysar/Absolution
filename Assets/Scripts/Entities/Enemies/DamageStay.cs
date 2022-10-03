using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private int myTargetLayer = 6;
    private Health myHealth;

    private bool playerOnBounds = false;


    private void Start()
    {
        myHealth = GetComponentInParent<Health>();
        if (myHealth == null)
        {
            myHealth = gameObject.AddComponent<Health>();
            myHealth.currentHP = 100;
        }


    }
    private void Update()
    {
        if(playerOnBounds && myHealth.currentHP > 0)
        {
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
