using System.Collections.Generic;
using UnityEngine;

public class Bomb_Explosion : MonoBehaviour
{
    public Bomb myBomb;
    List<IDamageable> myTargets = new List<IDamageable>();

    private void OnParticleSystemStopped()
    {
        myTargets.Clear();
        myBomb.myAttack.isAttacking = false;
        myBomb.myAttack.myAttack.AttackCube(true);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable myTarget = collision.GetComponent<IDamageable>();

        if(myTarget != null && collision.gameObject.layer != myBomb.player.gameObject.layer)
        {
            if(!myTargets.Contains(myTarget))
            {
                myTarget.TakeDamage(myBomb.myAttack.damage);
                myTargets.Add(myTarget);
            }
        }
    }
}
