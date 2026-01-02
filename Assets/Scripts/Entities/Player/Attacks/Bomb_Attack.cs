using System.Collections;
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
        CreateResource();
    }

    public override void EnteringMode()
    {

    }

    public override void EndAttack()
    {
        isAttacking = false;
        StopAllCoroutines();
    }

    public override void PrimaryAttack()
    {
        if(myBomb == null) CreateResource();

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
        myBomb.transform.position = transform.position;
        myBomb.gameObject.SetActive(true);
        myBomb.Flip();
        myBomb.Preparation();
        yield return new WaitForSeconds(timeToAttack);
        player.myAnim.SetBool("isAttacking", false);
    }

    public override void Setup()
    {
        myBomb.myAttack = this;
        myBomb.gameObject.SetActive(false);
    }

    public override void Interrupt()
    {

    }

    public override void CreateResource()
    {
        myBomb = Instantiate(bombPrefab);
        Setup();
    }
}

