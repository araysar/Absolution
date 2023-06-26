using UnityEngine;

public class Pilar_Health : Health
{
    public WeaponObstacle myObstacle;
    public TriggerObstacle myShoot;
    public int myNumber;

    void Start()
    {
        if(GameManager.instance.pilarDestroyed.Contains(myNumber))
        {
            myObstacle.myDoor.SetActive(true);
            myObstacle.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        GameManager.instance.AllwaysRespawnEvent += RespawnEnemy;
    }

    private void RespawnEnemy()
    {
        if (damagedVfx != null) damagedVfx.SetActive(false);
        if (deathVfx != null) deathVfx.SetActive(false);
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
        if (GameManager.instance.pilarDestroyed.Contains(myNumber)) GameManager.instance.pilarDestroyed.Remove(myNumber);
        myObstacle.Enable();
        myShoot.transform.position = transform.position;
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
        GetComponent<Collider2D>().enabled = false;
        myRenderer.enabled = false;
        myShoot.gameObject.SetActive(true);
        myShoot.transform.position = transform.position;

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
