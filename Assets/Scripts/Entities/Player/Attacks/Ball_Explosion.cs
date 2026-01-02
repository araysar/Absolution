using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Explosion : MonoBehaviour
{
    public Ball_Attack myAttack;
    public Rigidbody2D myRb;
    private AudioSource myAudio;
    private List<IDamageable> myTargets = new List<IDamageable>();

    private void Start()
    {
        SoundManager.instance.audioSources.Add(myAudio);
    }

    private void OnEnable()
    {
        myTargets.Clear();
    }

    private void OnDisable()
    {
        myAttack.isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable myTarget = collision.gameObject.GetComponent<IDamageable>();
        if(myTarget != null && collision.gameObject.tag != "Player")
        {
            if (myTargets.Contains(myTarget)) return;

            myTargets.Add(myTarget);
            myTarget.TakeDamage(myAttack.damage);
        }

    }
}
