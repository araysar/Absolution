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
    public float timeToAttack = 0.25f;

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
        StartCoroutine(PrimaryCooldown());
    }

    public override void SecondaryAttack()
    {

    }

    private IEnumerator PrimaryCooldown()
    {
        player.myAnim.SetBool("isAttacking", true);
        myAttack.AttackCube(false);
        isAttacking = true;
        myBoomerang.gameObject.SetActive(true);
        myBoomerang.Flip();
        myBoomerang.Timer();
        myBoomerang.isBacking = false;
        myBoomerang.transform.position = transform.position;
        player.myAnim.SetTrigger("primaryBoomerang");
        yield return new WaitForSeconds(timeToAttack);
        player.myAnim.SetBool("isAttacking", false);
    }

    public override void Setup()
    {
        myBoomerang.myAttack = this;
        myBoomerang.gameObject.SetActive(false);
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }

    public override void CreateResource()
    {
        myBoomerang = Instantiate(boomerangPrefab);
        Setup();
    }
}
