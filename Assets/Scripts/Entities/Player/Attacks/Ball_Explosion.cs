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

            float energyPercentage = myAttack.currentEnergy / myAttack.maxEnergy;

            float outputDamage = Mathf.Lerp(myAttack.damage / 3f, myAttack.damage * 1.25f, energyPercentage);

            myTargets.Add(myTarget);
            myTarget.TakeDamage(outputDamage);
        }

    }
}
