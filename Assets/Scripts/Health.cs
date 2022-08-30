using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHP = 10;
    public float currentHP;
    [SerializeField] private GameObject damagedEffect;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        damagedEffect.SetActive(false);
        damagedEffect.SetActive(true);
        if (currentHP <= 0) Death();
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    
}
