using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    public Cross_Attack myAttack;
    public List<IDamageable> myTargets = new List<IDamageable>();

    private void Start()
    {
        SoundManager.instance.audioSources.Add(GetComponent<AudioSource>());
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
                myTargets.Add(newTarget);
                newTarget.TakeDamage(myAttack.damage);
            }
        }
    }
}
