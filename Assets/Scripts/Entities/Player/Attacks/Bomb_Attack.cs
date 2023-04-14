using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public Bomb bombPrefab;
    private Bomb myBomb;
    public Vector2 addForce;
    public Vector2 addMovingForce;
    public float explodeTime = 1.25f;
    public float cooldown = 1.25f;

    //[Header("Secondary Attack")]

    private void Start()
    {
        myBomb = Instantiate(bombPrefab);
        Setup();
    }

    public override void EnteringMode()
    {
        throw new System.NotImplementedException();
    }

    public override void EndAttack()
    {
        isAttacking = false;
    }

    public override void PrimaryAttack()
    {
        if (myBomb == null)
        {
            myBomb = Instantiate(bombPrefab);
            Setup();
        }
        myAttack.AttackCube(false);
        isAttacking = true;
        myBomb.gameObject.SetActive(true);
        myBomb.isFacingRight = player.isFacingRight;
        myBomb.Flip();
        myBomb.Preparation();
        myBomb.transform.position = transform.position;
        player.myAnim.SetTrigger("primaryBomb");
    }

    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {
        myBomb.myAttack = this;
        myBomb.player = player;
        myBomb.gameObject.SetActive(false);
    }
}

