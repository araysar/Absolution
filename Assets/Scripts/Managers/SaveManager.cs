using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    GameManager gM;
    [SerializeField] private Animator saveAnimator;

    [Header("Save")]
    //Player Stats
    private float ulti1Stacks;
    private float currentHp;
    private float maxHp;
    private int currentShards;

    public List<Character_Movement.PowerUp> playerUpgrades = new List<Character_Movement.PowerUp>();
    public List<int> shards = new List<int>();
    public List<int> saveShards = new List<int>();
    public List<int> dialogues = new List<int>();
    public List<int> savedDialogues = new List<int>();

    private void Start()
    {
        gM = GetComponent<GameManager>();
        gM.SaveDataEvent += SaveData;
        gM.LoadDataEvent += LoadData;
    }

    public void SaveData()
    {
        SavingAnimation();
        currentHp = gM.player.myHealth.maxHP;
        ulti1Stacks = gM.player.ulti1Stacks;
        playerUpgrades = new List<Character_Movement.PowerUp>(gM.player.myUpgrades);
        gM.player.myHealth.initialPosition = gM.player.transform.position;
        saveShards = new List<int>(shards);
        currentShards = gM.player.GetComponent<Character_Attack>().currentShards;
        savedDialogues = new List<int>(dialogues);
    }

    public void LoadData()
    {
        gM.player.myHealth.currentHP = currentHp;
        gM.player.ulti1Stacks = ulti1Stacks;
        gM.player.PowerUpErase();
        gM.player.myUpgrades = new List<Character_Movement.PowerUp>(playerUpgrades);
        gM.player.PowerUpGrab();
        gM.player.transform.position = gM.player.myHealth.initialPosition;
        gM.player.myHealth.RefreshLifeBar();
        gM.player.ulti1.RefreshStacks(false);
        gM.player.GetComponent<Character_Attack>().currentShards = currentShards;
        shards = new List<int>(saveShards);
        dialogues = new List<int>(savedDialogues);
    }

    public void SavingAnimation()
    {
        saveAnimator.SetTrigger("saving");
    }
}