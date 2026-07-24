using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shards_System : MonoBehaviour
{
    public Character_Attack player;
    public TMP_Text uiShards;
    public AudioClip learnSfx;
    public AudioClip music;
    private AudioClip currentMusic;
    public GameObject firstIcon;

    [Header("Talents UI")]
    public Image[] uiButton;
    public Sprite[] spriteButton;
    public GameObject[] despertarPanels;
    public GameObject talentPanel;
    [HideInInspector] public int index;

    private void Update()
    {
        if(Input.GetButtonDown("Menu") && GameManager.instance.onShards && GameManager.instance.onPause == true)
        {
            Exit();
        }
    }
    private void FirstIcon()
    {
        EventSystem.current.SetSelectedGameObject(null); // Limpiamos selección anterior
        EventSystem.current.SetSelectedGameObject(firstIcon);
    }

    public void BTN_TalentEntry()
    {

        GameManager.instance.onPause = true;
        GameManager.instance.onShards = true;
        Time.timeScale = 0;
        FirstIcon();
        talentPanel.SetActive(true);
        uiShards.text = player.currentShards.ToString();
        currentMusic = SoundManager.instance.CurrentSong();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, music, transform);
    }

    public void Exit()
    {
        GameManager.instance.UnPause();
        StartCoroutine(DelayExit());
        talentPanel.gameObject.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.Music, currentMusic, transform);
        GameManager.instance.onPause = false;
        Time.timeScale = 1;
    }

    public void BTN_TalentExit()
    {
        SoundManager.instance.sfxAudioSource.PlayOneShot(SoundManager.instance.clickSfx);
        Exit();
    }

    IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.onShards = false;
    }
    public void ActiveInfo(int indexUI)
    {
        index = indexUI;
        SoundManager.instance.sfxAudioSource.PlayOneShot(SoundManager.instance.clickSfx);

        BTN_CheckUpdate(indexUI);
    }

    public void BTN_CheckUpdate(int myUpgrade)
    {
        switch(myUpgrade)
        {
            case 0:
                Upgrade(Character_Attack.Talents.Damage);
                break;
            case 1:
                Upgrade(Character_Attack.Talents.Defense);
                break;
            case 2:
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
            SoundManager.instance.sfxAudioSource.PlayOneShot(learnSfx);
            player.myUpgrades.Add(upgrade);
            uiButton[index].sprite = spriteButton[index];
            despertarPanels[index].gameObject.SetActive(false);
            switch (upgrade)
            {
                case Character_Attack.Talents.Damage:
                    player.damageUpgrade = true;
                    if (player.currentAttack.weaponName == "AstralBall") player.currentAttack.EnteringMode();
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
            Character_Movement.instance.TalentCheck();
        }
    }


}