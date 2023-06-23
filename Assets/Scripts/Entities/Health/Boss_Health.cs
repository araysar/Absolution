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

    public override void Death()
    {
        base.Death();
        myAnim.SetTrigger("death");
        SoundManager.instance.StopSong();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, SoundManager.instance.winMusic, GameManager.instance.player.transform);
    }
    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }
}
