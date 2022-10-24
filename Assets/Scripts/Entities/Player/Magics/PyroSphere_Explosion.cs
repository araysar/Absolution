using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyroSphere_Explosion : MonoBehaviour
{
    [HideInInspector] public Character_Movement myChar;
    [HideInInspector] public Action_Shoot myShooter;
    List<Collider2D> damagedEnemies = new List<Collider2D>();
    [SerializeField] private float damage;
    void Awake()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myShooter = myChar.GetComponent<Action_Shoot>();
    }

    public void OnParticleSystemStopped()
    {
        myShooter.pyroReady = true;
        myShooter.pyroAnimator.SetTrigger("ready");
        damagedEnemies.Clear();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null && collision.tag != "Player")
        {
            if(!damagedEnemies.Contains(collision))
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage);
                if(collision.GetComponent<Health>().currentHP <= 0)
                {
                    if (!myChar.ulti1.ultiReady)
                    {
                        myChar.ulti1Stacks += collision.GetComponent<Health>().ulti1Stacks;
                    }
                    myChar.ulti1.RefreshStacks();
                }
                damagedEnemies.Add(collision);
            }
        }
    }
}
