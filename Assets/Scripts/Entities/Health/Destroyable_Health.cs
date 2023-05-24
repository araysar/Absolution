using UnityEngine;

public class Destroyable_Health : Health
{
    public GameObject disableBody;
    public bool isStatic = false;
    void Start()
    {
        GameManager.instance.AllwaysRespawnEvent += RespawnEnemy;
    }

    private void RespawnEnemy()
    {
        if (damagedVfx != null) damagedVfx.SetActive(false);
        if (deathVfx != null) deathVfx.SetActive(false);
        if (disableBody != null) disableBody.SetActive(true);
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
        if(!isStatic) GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        myRenderer.enabled = true;
        currentHP = maxHP;
        transform.position = initialPosition;
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
        deathVfx.SetActive(true);
        if (disableBody != null) disableBody.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        myRenderer.enabled = false;
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
        GameManager.instance.AllwaysRespawnEvent -= RespawnEnemy;
    }
}
