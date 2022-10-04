using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PyroSphere_Explosion : MonoBehaviour
{
    [HideInInspector] public Character_Movement myChar;
    [HideInInspector] public Action_Shoot myShooter;
    List<Collider2D> damagedEnemies = new List<Collider2D>();
    [SerializeField] private float damage;
    void Start()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myShooter = myChar.GetComponent<Action_Shoot>();
    }

    public void OnParticleSystemStopped()
    {
        myShooter.pyroReady = true;
        damagedEnemies.Clear();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() != null)
        {
            if(!damagedEnemies.Contains(collision))
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage);
                damagedEnemies.Add(collision);
            }
        }
    }
}
