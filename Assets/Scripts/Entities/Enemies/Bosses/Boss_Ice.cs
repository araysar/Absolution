using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    }


    void Update()
    {
        if(isFighting && canMove)
        {
            if (myHealth.currentHP > 0 && canAttack)
            {
                canAttack = false;
                switch (currentAttack)
                {
                    case IceBossAttacks.IceSpikes:
                        myAnim.SetTrigger("attackIceSpikes");
                        break;
                    case IceBossAttacks.TopAttack:
                        myAnim.SetTrigger("attackTopAttack");
                        break;
                    case IceBossAttacks.BotAttack:
                        myAnim.SetTrigger("attackBotAttack");
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
        currentAttack = (IceBossAttacks)UnityEngine.Random.Range(0, enrage? Enum.GetValues(typeof(IceBossAttacks)).Length : 2);
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
        canAttack = false;
        isResting = true;
        isFighting = true;
        canMove = true;
    }
    
    public override void Respawn()
    {
        isResting = true;
        canAttack = false;
        isFighting = false;
        canMove = true;
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
