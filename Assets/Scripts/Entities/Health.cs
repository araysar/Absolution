using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Life")]
    public bool impactBomb = false;
    public float maxHP = 10;
    public float currentHP;
    public bool defenseActive = false;
    public bool invulnerable = false;
    public float ulti1Stacks = 5;
    [SerializeField] private GameObject damagedEffect;
    [SerializeField] private AudioClip damagedSfx;
    [SerializeField] private bool invulnerability = false;
    private bool recovering = false;
    public GameObject respawnEffect;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private GameObject deathEffect;
    public Animator myAnim;
    public Animator myUIAnim;
    [HideInInspector] public Vector2 initialPosition;
    public SpriteRenderer weakPoint;
    [SerializeField] private Color fullLifeColor;
    [SerializeField] private Color lowLifeColor;
    [SerializeField] private List<GameObject> disableAfterDeath;
    [SerializeField] private GameObject healVfx;
    [SerializeField] private AudioClip healSfx;

    [Space, Header("UI")]
    public Image lifeBar;
    [SerializeField] private Color fullLifeBarColor = Color.green;
    [SerializeField] private Color lowLifeBarColor = Color.red;

    [Space, Header("Flash")]
    [SerializeField] private float flashTimes = 0;
    [SerializeField] private float flash1Duration = 0.2f;
    [SerializeField] private Material paintableMaterial;
    [SerializeField] private Material commonMaterial;
    [SerializeField] private Color flashColor = Color.red;
    [HideInInspector] public SpriteRenderer myRenderer;
    private Coroutine flashCoroutine;
    public EntityType myType = EntityType.common;
    public ArmorType myArmor = ArmorType.flesh;

    [Space, Header("Drop")]
    [SerializeField] private GameObject myDrop;

    public enum EntityType
    {
        common,
        special,
        boss,
        player,
        isDestroyableObject,
    };

    public enum ArmorType
    {
        flesh,
        metal,
        hero,
        shield,
        wall,
    }

    private void Start()
    {
        initialPosition = transform.position;
        if (!invulnerability) flashTimes = 1;
        myRenderer = GetComponent<SpriteRenderer>();
        commonMaterial = myRenderer.material;
        currentHP = maxHP;

        switch (myType)
        {
            case EntityType.common:
                GameManager.instance.HealAllEnemiesEvent += HealEnemy;
                break;
            case EntityType.special:
                GameManager.instance.HealAllEnemiesEvent += HealEnemy;
                break;
            case EntityType.boss:
                GameManager.instance.ResetBossBattleEvent += HealEnemy;
                break;
            case EntityType.player:
                GameManager.instance.PlayerRespawnEvent += RespawnPlayer;
                GameManager.instance.PlayerDisableEvent += DisablePlayer;
                break;
            case EntityType.isDestroyableObject:
                GameManager.instance.AllwaysRespawnEvent += RespawnEnemy;
                break;
            default:
                break;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (recovering || currentHP <= 0) return;

        float totalDamage = defenseActive? dmg / 2 : dmg;
        if (!invulnerable)
        {
            currentHP -= totalDamage;
            if (currentHP < 0) currentHP = 0;
            if (damagedEffect != null)
            {
                damagedEffect.SetActive(false);
                damagedEffect.SetActive(true);
            }

            if (lifeBar != null || weakPoint != null) RefreshLifeBar();

            if(myAnim != null)
            {
                myAnim.SetBool("damaged", true);
            }
            if (deathSfx != null && currentHP <= 0) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, deathSfx);
            if (flashCoroutine == null)
            {
                if(currentHP > 0)
                {
                    if (damagedSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, damagedSfx);
                }
                flashCoroutine = StartCoroutine(Flashing());
            }
        }
        else
        {
            //no me hace daño
        }
    }
    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, healSfx);
        RefreshLifeBar();
        healVfx.SetActive(false);
        healVfx.SetActive(true);
    }    

    public void RefreshLifeBar()
    {
        if(lifeBar != null)
        {
            lifeBar.fillAmount = currentHP / maxHP;
            lifeBar.color = Color.Lerp(lowLifeBarColor, fullLifeBarColor, currentHP / maxHP);
        }
       
        if(weakPoint != null)
        {
            weakPoint.color = Color.Lerp(lowLifeColor, fullLifeColor, currentHP / maxHP );
        }
        
        if(myUIAnim != null)
        {
            if(currentHP / maxHP <= 0.225f && currentHP > 0)
            {
                myUIAnim.SetBool("onDanger", true);
            }
            else
            {
                myUIAnim.SetBool("onDanger", false);
            }
        }
    }

    public void EndDamaged()
    {
        myAnim.SetBool("damaged", false);
    }

    public void Death()
    {
        StopAllCoroutines();
        flashCoroutine = null;
        recovering = false;
        myRenderer.material = commonMaterial;
        myAnim.SetTrigger("death");
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void DeathEffect()
    {
        if (myDrop != null && myType != EntityType.boss) Instantiate(myDrop, transform.position, Quaternion.identity);
        if (deathEffect == null)
        {
            switch (myType)
            {
                case Health.EntityType.common:
                    myAnim.SetTrigger("exit");
                    GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
                    gameObject.SetActive(false);
                    break;
                case Health.EntityType.special:
                    GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
                    gameObject.SetActive(false);
                    break;
                case Health.EntityType.player:
                    GameManager.instance.Transition(GameManager.EventType.PlayerDeathTransition, 2.5f);
                    break;
                case EntityType.boss:
                    myAnim.SetTrigger("death");
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, null);
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, SoundManager.instance.winMusic);
                    break;
                case EntityType.isDestroyableObject:
                    break;
                default:
                    GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
                    gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
            gameObject.SetActive(false);
        }

    }


    private void BossDeathAnimation()
    {
        GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
        Instantiate(myDrop, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    private IEnumerator Flashing()
    {
        if (invulnerability) recovering = true;
        
        int count = 1;

        Material myMaterial = weakPoint != null ? weakPoint.material : myRenderer.material;
        Color myColor = weakPoint != null ? weakPoint.color : myRenderer.color;


        if (flashTimes > 0)
        {
            if(weakPoint == null)
            {
                myRenderer.material = paintableMaterial;
                myRenderer.color = Color.white;
                yield return new WaitForSeconds(flash1Duration);
                myRenderer.material = myMaterial;
                myRenderer.color = myColor;
            }
            else
            {
                weakPoint.material = paintableMaterial;
                weakPoint.color = Color.white;
                yield return new WaitForSeconds(flash1Duration);
                weakPoint.material = myMaterial;
                weakPoint.color = myColor;
            }
        }

        if (currentHP <= 0)
        {
            if (invulnerability) recovering = false;
            flashCoroutine = null;
            Death();
            yield break;
        }
        else
        {
            if(flashTimes > count)
            {
                yield return new WaitForSeconds(flash1Duration);
                while (count < flashTimes)
                {
                    myRenderer.material = paintableMaterial;
                    myRenderer.color = flashColor;
                    yield return new WaitForSeconds(flash1Duration);
                    myRenderer.material = myMaterial;
                    myRenderer.color = myColor;
                    count++;
                }
            }
            
            if (invulnerability) recovering = false;
            flashCoroutine = null;
        }
    }

    #region Respawn

    private void RespawnEnemy()
    {
        gameObject.SetActive(true);
        GameManager.instance.HealAllEnemiesEvent += HealEnemy;
        currentHP = maxHP;
        transform.position = initialPosition;
        myAnim.SetTrigger("exit");
        commonMaterial.SetFloat("DeathValue", 0);
    }

    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }
    private void RespawnPlayer()
    {
        myRenderer.enabled = true;
        if(respawnEffect != null) respawnEffect.SetActive(true);
        currentHP = maxHP;
    }

    private void DisablePlayer()
    {
        myRenderer.enabled = false;
        GameManager.instance.ParticleEffect(GameManager.ParticleType.PlayerDeathEffect, gameObject);
        currentHP = 0;
    }

    void OnDestroy()
    {
        switch (myType)
        {
            case EntityType.player:
                break;
            case EntityType.isDestroyableObject:
                GameManager.instance.AllwaysRespawnEvent -= RespawnEnemy;
                break;
            default:
                GameManager.instance.HealAllEnemiesEvent -= HealEnemy;
                break;
        }
    }
    #endregion
}