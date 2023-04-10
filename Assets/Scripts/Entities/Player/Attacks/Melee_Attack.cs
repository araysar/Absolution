using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public float radius = 1;
    public float speed;
    public float attackCooldown = 0.5f;

    [Header("Secondary Attack")]
    public Transform secondaryCenter;
    public float preparationTime = 1;

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
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, radius);
            player.myAnim.SetTrigger("primaryKatana");
            foreach (Collider2D item in collisions)
            {
                if (item.GetComponent<IDamageable>() != null && item.gameObject.layer != player.gameObject.layer)
                {
                    item.GetComponent<IDamageable>().TakeDamage(damage);
                }
            }
        }
    }

    public override void SecondaryAttack()
    {
        if(!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(SecondaryPreparation());
        }
    }

    public override void Setup()
    {
        StartCoroutine(AttackDelay());
    }

    public IEnumerator AttackDelay()
    {
        myAttack.AttackCube(false); 
        player.myAnim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackCooldown);
        myAttack.AttackCube(true);
        isAttacking = false;
        player.myAnim.SetBool("isAttacking", false);
        player.ResumeMovement();
        player.EnableFlip();
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
