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
    public GameObject[] uiPannels;

    [HideInInspector] public int index;

    public void BTN_TalentEntry()
    {

        GameManager.instance.onPause = true;
        Time.timeScale = 0;
        uiShards.text = "x " + player.currentShards.ToString();
        currentMusic = SoundManager.instance.CurrentSong();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, music, transform);
        talentPanel.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            uiPannels[i].SetActive(false);
        }
    }

    public void BTN_TalentExit()
    {
        GameManager.instance.UnPause();
        for (int i = 0; i < 4; i++)
        {
            uiPannels[i].SetActive(false);
        }
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.clickSfx, transform);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, currentMusic, transform); 
        GameManager.instance.onPause = false;
        Time.timeScale = 1;
        talentPanel.SetActive(false);
    }

    public void ActiveInfo(int indexUI)
    {
        index = indexUI;
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, SoundManager.instance.clickSfx, transform);

        for (int i = 0; i < uiPannels.Length; i++)
        {
            if (i != indexUI)
            {
                uiPannels[i].SetActive(false);
            }
            else
            {
                if(uiPannels[i].activeSelf)
                {
                    uiPannels[i].SetActive(false);
                }
                else
                {
                    uiPannels[i].SetActive(true);
                }
            }
        }
    }

    public void BTN_CheckUpdate(int myUpgrade)
    {
        switch(myUpgrade)
        {
            case 0:
                Upgrade(Character_Attack.Talents.AtkSpeed);
                break;
            case 1:
                Upgrade(Character_Attack.Talents.Damage);
                break;
            case 2:
                Upgrade(Character_Attack.Talents.Defense);
                break;
            case 3:
                Upgrade(Character_Attack.Talents.Revive);
                break;
            default:
                break;
        }
    }

    public void Upgrade(Character_Attack.Talents upgrade)
    {
        if (!player.myUpgrades.Contains(upgrade) && player.currentShards >= 4)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.Unscalled, learnSfx, transform);
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
            uiShards.text = "x " + player.currentShards.ToString();
            uiButton[index].color = Color.white;
        }
    }
}