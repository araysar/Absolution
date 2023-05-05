using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : Health
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
    }

    private void RespawnEnemy()
    {
        gameObject.SetActive(true);
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
        currentHP = maxHP;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        transform.position = initialPosition;
        myAnim.SetBool("death", false);
        commonMaterial.SetFloat("DeathValue", 0);
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        if (flashCoroutine == null) flashCoroutine = StartCoroutine(Flashing(1, 0.10f));
    }

    public override void Death()
    {
        base.Death();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
        GameManager.instance.HealAllEnemiesEvent -= HealEnemy;
    }

    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }

    private void OnDestroy()
    {
        GameManager.instance.HealAllEnemiesEvent -= HealEnemy;
    }
}