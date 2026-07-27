using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Ice : Boss
{
    [SerializeField] private float delayAttacks = 3;
    [SerializeField] private float currentTimer = 0;
    [SerializeField] private bool canAttack = false;
    [SerializeField] private bool isResting = false;
    [SerializeField] private bool isFighting = false;
    [SerializeField] private bool canMove = true;
    public bool enrage = false;
    [SerializeField] private IceBossAttacks currentAttack = IceBossAttacks.TopSpikes;
    [SerializeField] private List<IceSpikes> roofSpikes;
    [SerializeField] private List<IceSpikes> topSpikes;
    [SerializeField] private List<IceSpikes> botSpikes;
    [SerializeField] private AudioClip myBossScream;
    [SerializeField] private GameObject myScream;
    [SerializeField] private GameObject myEyes;

    public GameObject midPreparation;
    public GameObject midExplosion;
    public float timePreparation = 1.5f;
    public float timeExplosion = 1.5f;
    public AudioClip midExplosionClip;

    [SerializeField] private List<GameObject> floorSpikesPreparation;
    [SerializeField] private AudioClip floorPreparationSFX;
    [SerializeField] private AudioClip floorSpikesSFX;
    [SerializeField] private List<GameObject> floorSpikes;

    public enum IceBossAttacks
    {
        IceSpikes,
        TopSpikes,
        BotSpikes,
        MidAttack,
        FloorSpikes
    };

    void Start()
    {
        myManager.StartFightEvent += StartingFight;
        GameManager.instance.PlayerRespawnEvent += Respawn;
        GameManager.instance.StopMovementEvent += StopMovement;
        GameManager.instance.ResumeMovementEvent += ResumeMovement;
    }


    void Update()
    {
        if(isFighting && canMove && Character_Movement.instance.myHealth.currentHP > 0)
        {
            if (myHealth.currentHP > 0 && canAttack)
            {
                canAttack = false;
                switch (currentAttack)
                {
                    case IceBossAttacks.IceSpikes:
                        myAnim.SetTrigger("iceSpikes");
                        break;
                    case IceBossAttacks.TopSpikes:
                        myAnim.SetTrigger("topAttack");
                        break;
                    case IceBossAttacks.BotSpikes:
                        myAnim.SetTrigger("botAttack");
                        break;
                    case IceBossAttacks.MidAttack:
                        midPreparation.SetActive(false);
                        myAnim.SetTrigger("midAttack");
                        break;
                    case IceBossAttacks.FloorSpikes:
                        myAnim.SetTrigger("floorSpikes");
                        break;
                    default:
                        myAnim.SetTrigger("iceSpikes");
                        break;
                }
            }
            else if (myHealth.currentHP > 0 && isResting)
            {
                currentTimer -= Time.deltaTime;
                if (currentTimer <= 0)
                {
                    isResting = false;
                    SelectAttack();
                }
            }
        }
    }
    #region Attacks

    public override void GenerateShield()
    {
        
    }

    public void IceSpikesAttack()
    {
        foreach (var item in roofSpikes)
        {
            item.transform.position = item.initialPosition;
            item.gameObject.SetActive(true);
            item.transform.localScale = Vector2.zero;
            item.myAnim.SetTrigger("Spawn");
        }
        myAnim.SetTrigger("exit");
    }

    public void TopTornado()
    {
        foreach (var item in topSpikes)
        {
            item.transform.position = item.initialPosition;
            item.gameObject.SetActive(true);
            item.transform.localScale = Vector2.zero;
            item.myAnim.SetTrigger("Spawn");
        }
        myAnim.SetTrigger("exit");
    }

    public void BotAttack()
    {
        foreach (var item in botSpikes)
        {
            item.transform.position = item.initialPosition;
            item.gameObject.SetActive(true);
            item.transform.localScale = Vector2.zero;
            item.myAnim.SetTrigger("Spawn");
        }
        myAnim.SetTrigger("exit");
    }

    IEnumerator FloorSpikes()
    {
        FinishAttack(3);
        myAnim.SetTrigger("exit");
        int random = UnityEngine.Random.Range(0, 5);
        int i = 0;
        foreach (var item in floorSpikesPreparation)
        {
            if(random != i)  item.gameObject.SetActive(true);
            i++;
        }
        i = 0;
        yield return new WaitForSeconds(1.3f);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, floorSpikesSFX, Character_Movement.instance.transform);
        foreach (var item in floorSpikes)
        {
            if(random != i) item.gameObject.SetActive(true);
            i++;
        }
        yield return new WaitForSeconds(1.5f);
        foreach (var item in floorSpikes)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void MidAttack()
    {
        StartCoroutine(MidTimer());
    }

    IEnumerator MidTimer()
    {
        FinishAttack(timeExplosion);
        midPreparation.SetActive(true);
        myAnim.SetTrigger("exit");
        yield return new WaitForSeconds(timePreparation); 
        midExplosion.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX,midExplosionClip, Character_Movement.instance.transform);
    }

    #endregion
    public override void SelectAttack()
    {
        currentAttack = (IceBossAttacks)UnityEngine.Random.Range(0, enrage? Enum.GetValues(typeof(IceBossAttacks)).Length : 3);
        canAttack = true;
    }
    public override void FinishAttack(float time)
    {
        float enrageThresholdHP = myHealth.maxHP / 1.5f;
        float t = 1f - (myHealth.currentHP / enrageThresholdHP);
        t = Mathf.Clamp01(t);
        float dynamicDivisor = Mathf.Lerp(3.0f, 4.5f, t);

        isResting = true;
        canAttack = false;
        currentTimer = enrage? delayAttacks / dynamicDivisor : delayAttacks + time;
    }

    public override void StartingFight()
    {
        isFighting = true;
        canMove = true;
        isResting = false;
        SelectAttack();
    }
    
    public override void Respawn()
    {
        isResting = true;
        canAttack = false;
        isFighting = false;
        canMove = false;
        myAnim.SetTrigger("exit");
        StopAllCoroutines();
        myHealth.currentHP = myHealth.maxHP;
        GetComponent<Boss_Health>().WeakPointColor();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, SoundManager.instance.bossFightMusic, transform);
    }

    private void BossScream()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myBossScream, GameManager.instance.player.transform);
        myScream.SetActive(true);
    }

    public override void StopMovement()
    {
        canMove = false;
        myAnim.SetFloat("animatorSpeed", 0);
        ParticleSystem ps = myEyes.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpeed = 0;
    }

    public override void ResumeMovement()
    {
        canMove = true;
        myAnim.SetFloat("animatorSpeed", 1);
        ParticleSystem ps = myEyes.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpeed = 1;
    }
}
