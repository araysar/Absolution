using UnityEngine;
using UnityEngine.UI;

public class Player_Health : Health
{
    public GameObject myCamera;
    public Character_Attack myPlayerAttack;
    public GameObject respawnEffect;
    public GameObject playerDeathEffect;
    public AudioClip reviveHealingSfx;
    public AudioClip revivingSfx;
    public GameObject revivingVfx;

    [Space, Header("UI")]
    public Image lifeBar;
    public Animator myUIAnim;
    [SerializeField] private Color fullLifeBarColor = Color.green;
    [SerializeField] private Color lowLifeBarColor = Color.red;

    [Space, Header("Revive System")]
    public bool reviveReady = true;
    public float reviveCooldown = 300f;
    public float reviveCurrentTime = 0;

    void Start()
    {
        myPlayerAttack = GetComponent<Character_Attack>();
    }

    void Update()
    {
        if (myPlayerAttack.reviveUpgrade)
        {
            if(reviveCurrentTime >= reviveCooldown && !reviveReady)
            {
                reviveReady = true;
            }
            else if(reviveCurrentTime < reviveCooldown && !reviveReady)
            {
                reviveCurrentTime += Time.deltaTime;
            }
        }
    }

    public void RefreshLifeBar()
    {

        lifeBar.fillAmount = currentHP / maxHP;
        lifeBar.color = Color.Lerp(lowLifeBarColor, fullLifeBarColor, currentHP / maxHP);

        if (currentHP / maxHP <= 0.225f && currentHP > 0)
        {
            myUIAnim.SetBool("onDanger", true);
        }
        else
        {
            myUIAnim.SetBool("onDanger", false);
        }
    }

    public override void TakeDamage(float dmg)
    {
        float alf = myPlayerAttack.defenseUpgrade? dmg / 2 : dmg;
        base.TakeDamage(alf);
        RefreshLifeBar();

        if (currentHP <= 0) Death();
        else
        {
            if (flashCoroutine == null) flashCoroutine = StartCoroutine(Flashing(3, 0.15f));
        }
    }


    public override void Death()
    {
        base.Death();
        myCamera.SetActive(true);
        myAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void CheckDeath()
    {
        recovering = false;
        if (myPlayerAttack.reviveUpgrade && reviveReady)
        {
            myAnim.SetBool("reviveReady", true);
            SoundManager.instance.StopSong();
            Time.timeScale = 0;
        }
        else
        {
            myAnim.SetBool("reviveReady", false);
        }
    }

    public void ReviveHealing()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, reviveHealingSfx);
    }

    public void RespawnTransition()
    {
        GameManager.instance.Transition(GameManager.EventType.PlayerDeathTransition, 0);
    }

    public void DisableMyCamera()
    {
        myCamera.gameObject.SetActive(false);
    }

    public void RespawnPlayerHealth()
    {
        myAnim.SetTrigger("respawn");
        currentHP = maxHP;
        RefreshLifeBar();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        Time.timeScale = 1;
        revivingVfx.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, revivingSfx);
        myAnim.updateMode = AnimatorUpdateMode.Normal;
    }

    private void DisablePlayer()
    {
        myRenderer.enabled = false;
    }
}
