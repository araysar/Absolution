using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBounds : MonoBehaviour
{
    List<IDamageable> myTargets = new List<IDamageable>();
    public Melee_Attack myAttack;

    private void OnEnable()
    {
        myTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable myTarget = collision.GetComponent<IDamageable>();
        if(myTarget != null)
        {
            if(collision.gameObject.layer != myAttack.player.gameObject.layer && !myTargets.Contains(myTarget))
            {
                myTargets.Add(myTarget);
                myTarget.TakeDamage(myAttack.damage);
            }
        }
    }
}
