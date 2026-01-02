using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public Cross myCross;
    public float explodeTime = 1.25f;
    public float timeToAttack;
    private float ySpeed;
    private float currentGravity;

    //[Header("Secondary Attack")]

    private void Start()
    {
        currentGravity = player.rb.gravityScale;
    }

    public override void EnteringMode()
    {

    }

    public override void EndAttack()
    {
        Return();
        myAttack.myCube.move = true;
        isAttacking = false;
        player.rb.velocity = new Vector2(0, ySpeed);
        player.rb.gravityScale = currentGravity;
        player.isChanneling = false;
        myCross.gameObject.SetActive(false);
        player.myAnim.SetBool("isAttacking", false);
        player.myAnim.SetBool("primaryCross", false);
        StopAllCoroutines();
    }

    public override void PrimaryAttack()
    {
        StartCoroutine(PrimaryCooldown());
    }

    public override void SecondaryAttack()
    {

    }

    public void Return()
    {
        myCross.gameObject.SetActive(false);
    }

    private IEnumerator PrimaryCooldown()
    {
        ySpeed = player.rb.velocity.y;
        if (ySpeed > 0) ySpeed = 0;
        myAttack.myCube.transform.position = myCross.transform.position;
        myAttack.myCube.move = false;
        isAttacking = true;
        player.myAnim.SetBool("isAttacking", true);
        player.myAnim.SetBool("primaryCross", true);
        myCross.gameObject.SetActive(true);
        player.rb.velocity = Vector2.zero;
        player.rb.gravityScale = 0;
        player.isChanneling = true;
        yield return new WaitForSeconds(explodeTime / 2);
        myCross.myTargets.Clear();
        yield return new WaitForSeconds(explodeTime / 2);
        player.rb.velocity = new Vector2(0, ySpeed);
        player.rb.gravityScale = currentGravity;
        player.myAnim.SetBool("isAttacking", false);
        player.myAnim.SetBool("primaryCross", false);
        myCross.gameObject.SetActive(false);
        player.isChanneling = false;
        myAttack.myCube.move = true;
        yield return new WaitForSeconds(timeToAttack - explodeTime);
        isAttacking = false;
    }

    public override void Setup()
    {

    }

    public override void Interrupt()
    {

    }

    public override void CreateResource()
    {

    }
}
