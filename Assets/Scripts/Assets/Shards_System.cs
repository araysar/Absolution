using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shards_System : MonoBehaviour
{
    public Character_Attack player;
    public GameObject talentPanel;
    public Button talentBTN;
    public TMP_Text uiShards;
    public AudioClip learnSfx;
    public AudioClip music;
    private AudioClip currentMusic;

    [Header("Talents UI")]
    public Image[] uiButton;
    public Sprite[] spriteButton;
    public GameObject[] texts;
    public GameObject learnButton;

    [HideInInspector] public int index;

    public void BTN_TalentEntry()
    {

        GameManager.instance.onPause = true;
        Time.timeScale = 0;
        uiShards.text = "Shards: " + player.currentShards.ToString();
        currentMusic = SoundManager.instance.CurrentSong();
        learnButton.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, music);
        talentPanel.SetActive(true);
    }

    public void BTN_TalentExit()
    {
        GameManager.instance.UnPause();
        for (int i = 0; i < 4; i++)
        {
            texts[i].SetActive(false);
            uiButton[i].color = Color.white;
        }
        learnButton.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.clickSfx);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, currentMusic); 
        GameManager.instance.onPause = false;
        Time.timeScale = 1;
        talentPanel.SetActive(false);
    }

    public void ActiveInfo()
    {
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.clickSfx);
        if (!learnButton.activeSelf)
        {
            learnButton.SetActive(true);
        }
        for (int i = 0; i < 4; i++)
        {
            if (i != index)
            {
                texts[i].SetActive(false);
                uiButton[i].color = Color.white;
            }
            else
            {
                texts[i].SetActive(true);
                uiButton[i].color = Color.yellow;
            }
        }
    }

    public void BTN_AtkSpeed()
    {
        index = 0;
        ActiveInfo();
    }

    public void BTN_Damage()
    {
        index = 1;
        ActiveInfo();
    }

    public void BTN_Defense()
    {
        index = 2;
        ActiveInfo();
    }

    public void BTN_Revive()
    {
        index = 3;
        ActiveInfo();
    }

    public void BTN_Upgrade()
    {
        Character_Attack.Talents myTalent;
        switch (index)
        {
            case 0:
                myTalent = Character_Attack.Talents.AtkSpeed;
                break;
            case 1:
                myTalent = Character_Attack.Talents.Damage;
                break;
            case 2:
                myTalent = Character_Attack.Talents.Defense;
                break;
            case 3:
                myTalent = Character_Attack.Talents.Revive;
                break;
            default:
                myTalent = Character_Attack.Talents.AtkSpeed;
                break;
        }
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.clickSfx);
        Upgrade(myTalent);
    }

    public void Upgrade(Character_Attack.Talents upgrade)
    {
        if(!player.myUpgrades.Contains(upgrade) && player.currentShards >= 4)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, learnSfx);
            player.myUpgrades.Add(upgrade);
            uiButton[index].sprite = spriteButton[index];
            switch (upgrade)
            {
                case Character_Attack.Talents.AtkSpeed:
                    player.cooldownUpgrade = true;
                    break;
                case Character_Attack.Talents.Damage:
                    player.damageUpgrade = true;
                    break;
                case Character_Attack.Talents.Defense:
                    player.defenseUpgrade = true;
                    break;
                case Character_Attack.Talents.Revive:
                    player.reviveUpgrade = true;
                    break;
            }
            player.currentShards -= 4;
            uiShards.text = "Shards: " + player.currentShards.ToString();
            uiButton[index].color = Color.white;
        }
    }
}