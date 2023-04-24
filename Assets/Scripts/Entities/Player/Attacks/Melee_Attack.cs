using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public float radius = 1;
    public float speed;
    public float primaryAnimationTime = 0.25f;
    public float attackCooldown = 0.5f;
    public MeleeBounds myBounds;

    [Header("Secondary Attack")]
    public Transform secondaryCenter;
    public float preparationTime = 1;

    private void Start()
    {
        myBounds.myAttack = this;
    }

    public override void EnteringMode()
    {
        isAttacking = false;
    }

    public override void EndAttack()
    {
        StopAllCoroutines();
        isAttacking = false;
    }

    public override void PrimaryAttack()
    {
        if(!isAttacking)
        {
            isAttacking = true;
            Setup();
        }
    }

    public override void SecondaryAttack()
    {
        //if(!isAttacking)
        //{
        //    isAttacking = true;
        //    StartCoroutine(SecondaryPreparation());
        //}
    }

    public override void Setup()
    {
        StartCoroutine(PrimaryDelay());
    }

    public IEnumerator PrimaryDelay()
    {
        myAttack.AttackCube(false);
        player.myAnim.SetTrigger("primaryKatana");
        player.myAnim.SetBool("isAttacking", true);
        myBounds.gameObject.SetActive(true);
        player.DisableFlip();
        yield return new WaitForSeconds(primaryAnimationTime);
        player.myAnim.SetBool("isAttacking", false);
        myAttack.AttackCube(true);
        myBounds.gameObject.SetActive(false);
        player.EnableFlip();
        yield return new WaitForSeconds(attackCooldown - primaryAnimationTime);
        isAttacking = false;
    }

    public IEnumerator SecondaryPreparation()
    {
        player.myAnim.SetTrigger("secondaryKatana");
        player.myAnim.SetBool("isAttacking", true);
        myAttack.AttackCube(false);
        yield return new WaitForSeconds(0.55f);
        Collider2D[] collisions = Physics2D.OverlapBoxAll(secondaryCenter.position, new Vector2(3, 0.5f), 0);
        foreach (Collider2D item in collisions)
        {
            if (item.GetComponent<IDamageable>() != null && item.gameObject.layer != player.gameObject.layer)
            {
                item.GetComponent<IDamageable>().TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        myAttack.AttackCube(true);
        player.myAnim.SetBool("isAttacking", false);
        player.ResumeMovement();
        player.EnableFlip();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(secondaryCenter.position, new Vector3(3, 0.5f, 0.1f));
    }
}
