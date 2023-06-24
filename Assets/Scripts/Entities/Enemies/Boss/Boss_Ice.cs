using System;
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
    [SerializeField] private bool enrage = false;
    [SerializeField] private IceBossAttacks currentAttack = IceBossAttacks.TopTornado;
    [SerializeField] private List<IceSpikes> roofSpikes;
    [SerializeField] private List<IceSpikes> topSpikes;
    [SerializeField] private List<IceSpikes> botSpikes;
    [SerializeField] private AudioClip myBossScream;
    [SerializeField] private GameObject myScream;
    [SerializeField] private GameObject myEyes;
    
    public enum IceBossAttacks
    {
        IceSpikes,
        TopTornado,
        BotAttack,
        AoEAttack,
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
                    case IceBossAttacks.TopTornado:
                        myAnim.SetTrigger("topAttack");
                        break;
                    case IceBossAttacks.BotAttack:
                        myAnim.SetTrigger("botAttack");
                        break;
                    case IceBossAttacks.AoEAttack:
                        if (enrage) myAnim.SetTrigger("attackAoEAttack");
                        else SelectAttack();
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

    public void AoEAttack()
    {

    }

    #endregion
    public override void SelectAttack()
    {
        currentAttack = (IceBossAttacks)UnityEngine.Random.Range(0, enrage? Enum.GetValues(typeof(IceBossAttacks)).Length : 3);
        canAttack = true;
    }
    public override void FinishAttack(float time)
    {
        isResting = true;
        canAttack = false;
        currentTimer = delayAttacks + time;
    }

    public override void StartingFight()
    {
        isFighting = true;
        canMove = true;
        canAttack = true;
        isResting = false;
    }
    
    public override void Respawn()
    {
        isResting = true;
        canAttack = false;
        isFighting = false;
        canMove = false;
        myAnim.SetTrigger("exit");
        myHealth.currentHP = myHealth.maxHP;
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
