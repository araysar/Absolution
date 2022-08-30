using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    [SerializeField] private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField] private Transform attack1HitBoxPos;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private GameObject hitEffect;
    
    private bool gotInput = false;
    private bool isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;
    private Animator myAnim;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckAttacks();
    }

    public void CheckCombatInput()
    {
        if (combatEnabled)
        {
            gotInput = true;
            lastInputTime = Time.time;
        }

    }

    private void CheckAttacks()
    {
        if(gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                myAnim.SetBool("attack1", true);
                myAnim.SetBool("firstAttack", isFirstAttack);
                myAnim.SetBool("isAttacking", isAttacking);
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    public void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);
        hitEffect.SetActive(false);
        hitEffect.SetActive(true);
        foreach (Collider2D item in detectedObjects)
        {
            item.GetComponent<IDamageable>().TakeDamage(attack1Damage);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
        isFirstAttack = !isFirstAttack;
        myAnim.SetBool("isAttacking", isAttacking);
        myAnim.SetBool("attack1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
        Gizmos.color = Color.green;
    }
}
