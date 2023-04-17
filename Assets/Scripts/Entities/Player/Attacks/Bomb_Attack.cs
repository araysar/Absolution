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
    public float timeToAttack;

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
        StartCoroutine(PrimaryCooldown());
    }

    public override void SecondaryAttack()
    {

    }

    private IEnumerator PrimaryCooldown()
    {
        myAttack.AttackCube(false);
        isAttacking = true;
        player.myAnim.SetBool("isAttacking", true);
        player.myAnim.SetTrigger("primaryBomb");
        yield return new WaitForSeconds(timeToAttack / 2);
        myBomb.transform.position = transform.position;
        myBomb.gameObject.SetActive(true);
        myBomb.Flip();
        myBomb.Preparation();
        yield return new WaitForSeconds(timeToAttack / 2);
        player.myAnim.SetBool("isAttacking", false);
    }
    public override void Setup()
    {
        myBomb.myAttack = this;
        myBomb.gameObject.SetActive(false);
    }
}

