using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Attack : Attack_Type
{
    [HideInInspector]public Ball_Explosion myExplosion;
    public Ball_Explosion ballPrefab;

    public override void CreateResource()
    {
        myExplosion = Instantiate(ballPrefab);
        myExplosion.myAttack = this;
        myExplosion.gameObject.SetActive(false);
    }

    public override void EndAttack()
    {
        myAttack.myCube.currentAngle = 0;
        myAttack.myCube.astralBallActivated = false;
    }

    public override void EnteringMode()
    {
        if(myExplosion == null) CreateResource();
        myAttack.myCube.astralBallActivated = true;
    }

    public override void Interrupt()
    {

    }

    public override void PrimaryAttack()
    {
        base.PrimaryAttack();
        if (myExplosion == null) CreateResource();
        myExplosion.transform.position = myAttack.myCube.transform.position;
        myExplosion.gameObject.SetActive(true);
        isAttacking = true;
    }

    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {

    } 
}
