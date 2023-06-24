using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : Health
{
    public Animator myAnim;
    public BossFightManager myManager;
    public Color fullHealthColor;
    public Color dangerHealthColor;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        fullHealthColor = myRenderer.color;
        GameManager.instance.ResetBossBattleEvent += HealEnemy;
    }

    public override void Death()
    {
        base.Death();
        myAnim.SetTrigger("death");
        GameManager.instance.iceBossDead = true;
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.SaveData);
        GameManager.instance.ResetBossBattleEvent -= HealEnemy;
        SoundManager.instance.StopSong();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, SoundManager.instance.winMusic, GameManager.instance.player.transform);
    }

    private void WeakPointColor()
    {
        myRenderer.color = Color.Lerp(dangerHealthColor, fullHealthColor, currentHP / maxHP);
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        WeakPointColor();
        if (flashCoroutine == null) flashCoroutine = StartCoroutine(Flashing(1, 0.10f));
    }

    private void Drop()
    {
        Instantiate(deathVfx, transform.position, Quaternion.identity);
        Instantiate(myDrop, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    private void HealEnemy()
    {
        currentHP = maxHP;
        transform.position = initialPosition;
    }
}
