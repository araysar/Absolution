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
    [SerializeField] private IceBossAttacks currentAttack = IceBossAttacks.TopAttack;
    [SerializeField] private List<IceSpikes> roofSpikes;
    [SerializeField] private List<IceSpikes> topSpikes;
    [SerializeField] private List<IceSpikes> botSpikes;
    [SerializeField] private AudioSource myVoice;

    public enum IceBossAttacks
    {
        IceSpikes,
        TopAttack,
        BotAttack,
        AoEAttack,
    };

    void Start()
    {
        myManager.StartFightEvent += StartingFight;
        myVoice = gameObject.AddComponent<AudioSource>();

        GameManager.instance.AllwaysRespawnEvent += Respawn;
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
                    case IceBossAttacks.TopAttack:
                        myAnim.SetTrigger("topAttack");
                        break;
                    case IceBossAttacks.BotAttack:
                        myAnim.SetTrigger("botAttack");
                        break;
                    case IceBossAttacks.AoEAttack:
                        if(enrage) myAnim.SetTrigger("attackAoEAttack");
                        break;
                    default:
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

    public void TopAttack()
    {

    }

    public void BotAttack()
    {

    }
    public void AoEAttack()
    {

    }
    #endregion
    public override void SelectAttack()
    {
        currentAttack = (IceBossAttacks)UnityEngine.Random.Range(0, enrage? Enum.GetValues(typeof(IceBossAttacks)).Length : 1);
        canAttack = true;
    }
    public override void FinishAttack()
    {
        isResting = true;
        canAttack = false;
        currentTimer = delayAttacks;
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
        isResting = false;
        canAttack = false;
        isFighting = false;
        canMove = false;
        myHealth.currentHP = myHealth.maxHP;
    }

    public override void StopMovement()
    {
        canMove = false;
        myAnim.SetFloat("animatorSpeed", 0);
    }

    public override void ResumeMovement()
    {
        canMove = true;
        myAnim.SetFloat("animatorSpeed", 1);
    }
}
