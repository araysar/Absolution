using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Tornado : MonoBehaviour
{
    [SerializeField] private float damagePerSecond;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null && collision.gameObject.tag != "Player")
        {
            collision.GetComponent<IDamageable>().TakeDamage(damagePerSecond / 60);
        }
    }
}