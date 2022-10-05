using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponObstacle : MonoBehaviour
{
    public Action_Shoot.AttackType attackType;

    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private DamageStay myDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (attackType)
        {
            case Action_Shoot.AttackType.FrozenDaggers:
                break;
            case Action_Shoot.AttackType.PyroSphere:
                if(collision.gameObject.GetComponent<PyroSphere_Explosion>() != null)
                {
                    Destroy();
                }
                break;
        }
    }
    public void Destroy()
    {
        if (destroyEffect != null) Instantiate(destroyEffect, transform.position, Quaternion.identity);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        if (myDamage != null) myDamage.enabled = false;
    }
}