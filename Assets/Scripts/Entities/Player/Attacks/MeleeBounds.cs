using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBounds : MonoBehaviour
{
    List<IDamageable> myTargets = new List<IDamageable>();
    public Melee_Attack myAttack;
    public AudioClip[] myClips;

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
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClips[Random.Range(0, myClips.Length)], transform);
                float totalDamage = 0;
                if (myAttack.myAttack.damageUpgrade) totalDamage = myAttack.damage * 1.5f;
                else totalDamage += myAttack.damage;

                if (collision.gameObject.tag == "Boss") totalDamage += 10;

                myTarget.TakeDamage(totalDamage);
            }
        }
    }
}
