using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    public Cross_Attack myAttack;
    public List<IDamageable> myTargets = new List<IDamageable>();

    private void Start()
    {

    }
    private void OnEnable()
    {
        myTargets.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable newTarget = collision.GetComponent<IDamageable>();
        if(newTarget != null)
        {
            if(!myTargets.Contains(newTarget) && collision.gameObject.tag != "Player")
            {
                float energyPercentage = myAttack.currentEnergy / myAttack.maxEnergy;

                float outputDamage = Mathf.Lerp(myAttack.damage / 4f, myAttack.damage * 1.25f, energyPercentage);
                myTargets.Add(newTarget);
                newTarget.TakeDamage(outputDamage);
            }
        }
    }
}
