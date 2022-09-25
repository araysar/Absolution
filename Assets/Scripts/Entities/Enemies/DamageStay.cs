using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageStay : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    private Health myHealth;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask myTargetLayer;

    private bool playerOnBounds = false;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        myHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if(playerOnBounds)
        {
            Collider2D[] allObjectives = Physics2D.OverlapBoxAll((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0, myTargetLayer);
            foreach (var item in allObjectives)
            {
                if(item.GetComponent<IDamageable>() != null)
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
        Gizmos.DrawWireCube((Vector2)transform.position + boxCollider.offset, boxCollider.size);
    }
}
