using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : Health
{
    // Start is called before the first frame update
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

        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, null);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, SoundManager.instance.winMusic);
    }
    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }
}
