using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : Health
{
    public Animator myAnim;
    void Start()
    {
        GameManager.instance.ResetBossBattleEvent += HealEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public override void Death()
    {
        base.Death();
        myAnim.SetTrigger("death");
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, null, transform);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, SoundManager.instance.winMusic, transform);
    }
    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }
}
