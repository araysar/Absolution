using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : Health
{
    public Animator myAnim;
    public Boss_Ice myBoss;
    public BossFightManager myManager;
    public Color fullHealthColor;
    public Color dangerHealthColor;
    public GameObject enrageVFX;
    public AudioClip enrageSFX;
    public Color spriteBossEnrage;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        myBoss = GetComponent<Boss_Ice>();
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

    public void WeakPointColor()
    {
        myRenderer.color = Color.Lerp(dangerHealthColor, fullHealthColor, currentHP / maxHP);
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        WeakPointColor();
        if (currentHP <= maxHP / 2.5f && !myBoss.enrage) StartEnrage();
        if (flashCoroutine == null) flashCoroutine = StartCoroutine(Flashing(1, 0.10f));
    }

    public void StartEnrage()
    {
        enrageVFX.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, enrageSFX, Character_Movement.instance.transform);
        myBoss.enrage = true;
        myRenderer.color = spriteBossEnrage;
    }

    public void EndEnrage()
    {
        enrageVFX.SetActive(false); 
        myBoss.enrage = false;
        myRenderer.color = Color.white;
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
        EndEnrage();
        transform.position = initialPosition;
    }
}
