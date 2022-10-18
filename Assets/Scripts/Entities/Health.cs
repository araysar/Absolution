using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Life")]
    public float maxHP = 10;
    public float currentHP;
    public float defense = 0;
    public float ulti1Stacks = 5;
    [SerializeField] private GameObject damagedEffect;
    [SerializeField] private bool invulnerability = false;
    private bool recovering = false;
    public GameObject respawnEffect;
    public Animator myAnim;
    [HideInInspector] public Vector2 initialPosition;

    [Space, Header("UI")]
    public Image lifeBar;

    [Space, Header("Flash")]
    [SerializeField] private float flashTimes = 3;
    [SerializeField] private float flash1Duration = 0.2f;
    [SerializeField] private Material paintableMaterial;
    [SerializeField] private Material commonMaterial;
    [SerializeField] private Color flashColor = Color.red;
    [HideInInspector] public SpriteRenderer myRenderer;
    private Coroutine flashCoroutine;
    public EntityType myType = EntityType.common;
    public ArmorType myArmor = ArmorType.flesh;
    public enum EntityType
    {
        common,
        special,
        boss,
        player,
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

        if(myType == EntityType.player)
        {
            GameManager.instance.PlayerRespawnEvent += RespawnPlayer;
            GameManager.instance.PlayerDisableEvent += DisablePlayer;

        }
        else
        {
            GameManager.instance.HealAllEnemiesEvent += RespawnEnemy;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (recovering || currentHP <= 0) return;

        float totalDamage = dmg - defense;
        if (totalDamage > 0)
        {
            currentHP -= totalDamage;
            if (currentHP < 0) currentHP = 0;
            if (damagedEffect != null)
            {
                damagedEffect.SetActive(false);
                damagedEffect.SetActive(true);
            }

            if (lifeBar != null) RefreshLifeBar();

            if(myAnim != null)
            {
                myAnim.SetTrigger("damaged");
            }

            if (currentHP <= 0)
            {
                Death();
            }
            else
            {
                if (flashCoroutine != null) return;

                flashCoroutine = StartCoroutine(Flashing());
            }
        }
        else
        {
            //no me hace daño
        }
    }

    public void RefreshLifeBar()
    {
        lifeBar.fillAmount = currentHP / maxHP;
    }

    public void Death()
    {
        StopAllCoroutines();
        flashCoroutine = null;
        recovering = false;
        myRenderer.material = commonMaterial;
        commonMaterial.color = Color.white;
        GameManager.instance.DeathEffect(myType, gameObject);
        switch (myType)
        {
            case EntityType.player:
                GameManager.instance.TransitionEvent(GameManager.EventType.PlayerDeathTransition);
                break;
            default:
                GameManager.instance.EnemyRespawnEvent += RespawnEnemy;
                gameObject.SetActive(false);
                break;
        }
    }


    private IEnumerator Flashing()
    {
        if (invulnerability) recovering = true;
        int count = 0;

        Material myMaterial = myRenderer.material;
        Color myColor = myRenderer.color;

        while(count < flashTimes)
        {
            myRenderer.material = paintableMaterial;
            myRenderer.color = flashColor;
            yield return new WaitForSeconds(flash1Duration);
            myRenderer.material = myMaterial;
            myRenderer.color = myColor;
            yield return new WaitForSeconds(flash1Duration);
            count++;
        }
        if (invulnerability) recovering = false;
        flashCoroutine = null;
    }

    #region Respawn

    private void RespawnEnemy()
    {
        gameObject.SetActive(true);
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
        GameManager.instance.DeathEffect(myType, gameObject);
        currentHP = 0;
    }

    void OnDestroy()
    {
        if(myType != EntityType.player)
        {
            GameManager.instance.HealAllEnemiesEvent -= RespawnEnemy;
        }
    }
    #endregion
}