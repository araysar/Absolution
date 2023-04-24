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

    [Header("Talents UI")]
    public Image cooldownUI;
    public Image damageUI;
    public Image defenseUI;
    public Image reviveUI;
    public Sprite cooldownSprite;
    public Sprite damageSprite;
    public Sprite defenseSprite;
    public Sprite reviveSprite;

    public void BTN_TalentEntry()
    {
        GameManager.instance.Pause();
        talentBTN.interactable = false;
        uiShards.text = player.currentShards.ToString();
        talentPanel.SetActive(true);
    }

    public void BTN_TalentExit()
    {
        GameManager.instance.UnPause();
        talentBTN.interactable = true;
        talentPanel.SetActive(false);
    }

    public void BTN_Upgrade(int upgrade)
    {
        Character_Attack.Upgrades myUpgrade;
        switch (upgrade)
        {
            case 0:
                myUpgrade = Character_Attack.Upgrades.Cooldown;
                break;
            case 1:
                myUpgrade = Character_Attack.Upgrades.Damage;
                break;
            case 2:
                myUpgrade = Character_Attack.Upgrades.Defense;
                break;
            case 3:
                myUpgrade = Character_Attack.Upgrades.Revive;
                break;
            default:
                myUpgrade = Character_Attack.Upgrades.Cooldown;
                break;
        }
        Upgrade(myUpgrade);
    }

    public void Upgrade(Character_Attack.Upgrades upgrade)
    {
        if(!player.myUpgrades.Contains(upgrade) && player.currentShards >= 4)
        {
            player.myUpgrades.Add(upgrade);
            switch (upgrade)
            {
                case Character_Attack.Upgrades.Cooldown:
                    cooldownUI.sprite = cooldownSprite;
                    break;
                case Character_Attack.Upgrades.Damage:
                    damageUI.sprite = damageSprite;
                    break;
                case Character_Attack.Upgrades.Defense:
                    defenseUI.sprite = defenseSprite;
                    break;
                case Character_Attack.Upgrades.Revive:
                    reviveUI.sprite = reviveSprite;
                    break;
            }
            player.currentShards -= 4;
            uiShards.text = player.currentShards.ToString();
        }
    }
}