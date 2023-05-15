using UnityEngine;

public class Destroyable_Health : Health
{

    void Start()
    {
        GameManager.instance.AllwaysRespawnEvent += RespawnEnemy;
    }

    private void RespawnEnemy()
    {
        gameObject.SetActive(true);
        if (damagedVfx != null) damagedVfx.SetActive(false);
        if (deathVfx != null) deathVfx.SetActive(false);
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        currentHP = maxHP;
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
        GameManager.instance.AllwaysRespawnEvent -= RespawnEnemy;
    }
}
