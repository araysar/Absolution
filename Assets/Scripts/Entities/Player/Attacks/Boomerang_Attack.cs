using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public Boomerang boomerangPrefab;
    private Boomerang myBoomerang;
    public float primarySpeed;
    public float backTime = 0.75f;

    //[Header("Secondary Attack")]

    private void Start()
    {
        myBoomerang = Instantiate(boomerangPrefab);
        Setup();
    }

    public override void EnteringMode()
    {
        throw new System.NotImplementedException();
    }

    public override void EndAttack()
    {
        isAttacking = false;
        if(myBoomerang != null)
            myBoomerang.gameObject.SetActive(false);
    }

    public override void PrimaryAttack()
    {
        if(myBoomerang == null)
        {
            myBoomerang = Instantiate(boomerangPrefab);
            Setup();
        }
    }

    public override void SecondaryAttack()
    {

    }

    private IEnumerator PrimaryCooldown()
    {
        player.myAnim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.3f);
        myAttack.AttackCube(false);
        isAttacking = true;
        myBoomerang.gameObject.SetActive(true);
        myBoomerang.Timer();
        myBoomerang.isBacking = false;
        myBoomerang.isFacingRight = player.isFacingRight;
        myBoomerang.Flip();
        myBoomerang.transform.position = player.transform.position;
        player.myAnim.SetTrigger("primaryBoomerang");
        yield return new WaitForSeconds(0.3f);
        player.myAnim.SetBool("isAttacking", false);
    }

    public override void Setup()
    {
        myBoomerang.speed = primarySpeed;
        myBoomerang.damage = damage;
        myBoomerang.backTime = backTime;
        myBoomerang.myAttack = this;
        myBoomerang.player = player;
    }
}
