using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : Health
{
    public Character_Attack myPlayerAttack;
    public GameObject respawnEffect;
    public GameObject playerDeathEffect;
    public AudioClip reviveHealingSfx;
    public AudioClip revivingSfx;
    public GameObject revivingVfx;
    private Coroutine damageAnimCoroutine;

    [Space, Header("UI")]
    public Slider lifeSlider;
    public Image lifeBar;
    public GameObject handle;
    public Animator myUIAnim;
    [SerializeField] private Color fullLifeBarColor = Color.green;
    [SerializeField] private Color lowLifeBarColor = Color.red;
    Action LifeBarEffect = delegate { };

    [Space, Header("Revive System")]
    public bool reviveReady = true;
    public float reviveCooldown = 300f;
    public float reviveCurrentTime = 0;

    void Start()
    {
        myPlayerAttack = GetComponent<Character_Attack>();
        lifeBar.color = fullLifeBarColor;
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
    private void FixedUpdate()
    {
        LifeBarEffect();
    }
    float currentVelocity = 0;

    private void LifeBarAnimation()
    {
        handle.SetActive(true);
        lifeSlider.value = Mathf.SmoothDamp(lifeSlider.value, currentHP / maxHP, ref currentVelocity, 0.25f, 400 * Time.fixedDeltaTime);
        if (lifeSlider.value < currentHP / maxHP + 0.002f)
        {
            lifeSlider.value = currentHP / maxHP;
            handle.SetActive(false);
            LifeBarEffect = delegate { };
        }
    }
    public void RefreshLifeBar()
    {
        LifeBarEffect = LifeBarAnimation;

        if (currentHP / maxHP <= 0.225f)
        {
            lifeBar.color = lowLifeBarColor; 
            myUIAnim.SetBool("onDanger", true);
            if (currentHP == 0) myUIAnim.SetBool("onDanger", false);
        }
        else
        {
            lifeBar.color = fullLifeBarColor;
            myUIAnim.SetBool("onDanger", false);
        }
    }

    public override void TakeDamage(float dmg)
    {
        if(recovering == false)
        {
            float alf = myPlayerAttack.defenseUpgrade ? dmg / 2 : dmg;
            base.TakeDamage(alf);
            RefreshLifeBar();

            if (currentHP <= 0) Death();
            else
            {
                if (flashCoroutine == null) flashCoroutine = StartCoroutine(Flashing(3, 0.15f));
                if (damageAnimCoroutine == null) damageAnimCoroutine = StartCoroutine(TakeDamageTimer());
            }
        }
    }

    IEnumerator TakeDamageTimer()
    {
        myAnim.SetBool("damaged", true);
        yield return new WaitForSeconds(0.35f);
        myAnim.SetBool("damaged", false);
        damageAnimCoroutine = null;
    }

    public override void Death()
    {
        myAnim.SetBool("dead", true);
        base.Death();
        damageAnimCoroutine = null;
        myAnim.SetBool("damaged", false);
        CameraManager.instance.normalCamera.gameObject.SetActive(false);
        CameraManager.instance.playerCamera.gameObject.SetActive(true);
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
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, reviveHealingSfx, transform);
        reviveReady = false;
        reviveCurrentTime = 0;
        StartCoroutine(ReviveHealingTimer());
    }

    IEnumerator ReviveHealingTimer()
    {
        for (float i = 0; i < 3.5f; i += Time.fixedDeltaTime)
        {
            currentHP = (i * maxHP) / 3;
            lifeBar.fillAmount = currentHP / maxHP;
            lifeBar.color = Color.Lerp(lowLifeBarColor, fullLifeBarColor, currentHP / maxHP);
            yield return null;
        }
    }
    public void RespawnTransition()
    {
        GameManager.instance.Transition(GameManager.EventType.PlayerDeathTransition, 0);
    }

    public void DisableMyCamera()
    {
        CameraManager.instance.normalCamera.gameObject.SetActive(true);
        CameraManager.instance.playerCamera.gameObject.SetActive(false);
    }

    public void RespawnPlayerHealth()
    {
        myAnim.SetTrigger("respawn");
        myAnim.SetBool("dead", false);
        StartCoroutine(Flashing(10, 0.15f));
        currentHP = maxHP;
        RefreshLifeBar();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        Time.timeScale = 1;
        revivingVfx.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, revivingSfx, transform);
        myAnim.updateMode = AnimatorUpdateMode.Normal;
    }

    private void DisablePlayer()
    {
        myRenderer.enabled = false;
    }
}
