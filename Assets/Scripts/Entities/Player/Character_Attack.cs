using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Character_Attack : MonoBehaviour
{
    private Character_Movement player;
    public Animator myUIAnim;
    public bool canAttack = true;
    public Cube myCubePrefab;
    public Cube myCube; 
    public List<Transform> animationPositions = new List<Transform>();

    [Header("Attack System")]
    public AudioClip changeSfx;
    public ParticleSystem changeVfx;
    public Transform cubeTransform;
    public int firstWeapon;
    public Attack_Type[] myAttacks;
    public Attack_Type currentAttack;
    public float timeToShuffle = 30;
    public float currentTime;
    public bool shuffleActivated = true;
    public Image timerUI;
    public TMP_Text nameText;
    public Image uiImage;
    public Color emptyColor;
    public Color fullColor;

    //[Header("Actions")]

    [Header("Talents System")]
    public int currentShards = 0;
    public int requiredShards = 4;
    public Shards_System shardsSystem;
    public List<int> myShards = new List<int>();
    public List<Upgrades> myUpgrades = new List<Upgrades>();

    [Header("Upgrades")]
    public bool cooldownUpgrade = false;
    public bool damageUpgrade = false;
    public bool defenseUpgrade = false;
    public bool reviveUpgrade = false;


    public enum Upgrades
    {
        AtkSpeed,
        Damage,
        Defense,
        Revive,
    };

    public void ActivateShuffle()
    {
        shuffleActivated = true;
    }

    public void DisableShuffle()
    {
        shuffleActivated = false;
    }

    public void ChangeWeapon()
    {
        int nextWeapon = Random.Range(0, myAttacks.Length);

        if (myAttacks[nextWeapon] == currentAttack)
        {
            ChangeWeapon();
        }
        else
        {
            currentAttack.EndAttack();
            currentAttack = myAttacks[nextWeapon];
            currentTime = 0; 
            AttackCube(true);
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, changeSfx);
            changeVfx.startColor = currentAttack.myColor;
            changeVfx.gameObject.SetActive(true);
            uiImage.sprite = currentAttack.myImage;
        }
    }

    private void Awake()
    {
        CreateCube();
        player = GetComponent<Character_Movement>();
        currentTime = 0;
        currentAttack = myAttacks[firstWeapon];// myAttacks[Random.Range(0, myAttacks.Length)];
        uiImage.sprite = currentAttack.myImage;
        shardsSystem = GetComponentInChildren<Shards_System>();
    }

    public void EndAttackAnimation()
    {
        player.myAnim.SetBool("isAttacking", false);
    }

    public void CreateCube()
    {
        if(myCube == null)
        {
            myCube = Instantiate(myCubePrefab);
            myCube.animationPositions = animationPositions;
            myCube.gameObject.transform.position = cubeTransform.position;
            myCube.myDestination = cubeTransform;
        }
    }

    private void FixedUpdate()
    {
        if(!GameManager.instance.onPause && player.myHealth.currentHP > 0)
        {
            if (!player.disableInputs && canAttack)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (!currentAttack.isAttacking)
                    {
                        currentAttack.PrimaryAttack();
                    }
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    if (!currentAttack.isAttacking)
                    {
                        currentAttack.SecondaryAttack();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    shardsSystem.uiButton[0].sprite = shardsSystem.spriteButton[0];
                    cooldownUpgrade = true;
                    shardsSystem.uiButton[1].sprite = shardsSystem.spriteButton[1];
                    damageUpgrade = true;
                    shardsSystem.uiButton[2].sprite = shardsSystem.spriteButton[2];
                    defenseUpgrade = true;
                    shardsSystem.uiButton[3].sprite = shardsSystem.spriteButton[3];
                    reviveUpgrade = true;
                }

                {
                    if (!currentAttack.isAttacking)
                    {
                        currentAttack.SecondaryAttack();
                    }
                }
            }
            if(shuffleActivated)
            {
                currentTime += Time.fixedDeltaTime;
                TimerUI();
            }
            if(currentTime >= timeToShuffle && shuffleActivated)
            {
                ChangeWeapon();
            }
        }
    }

    public void AttackCube(bool value)
    {
        myCube.gameObject.SetActive(value);
        myCube.transform.position = cubeTransform.position;
    }

    private void TimerUI()
    {
        timerUI.fillAmount = currentTime / timeToShuffle;
        timerUI.color = Color.Lerp(emptyColor, fullColor, timerUI.fillAmount);

        if(currentTime >= timeToShuffle - 5)
        {
            myUIAnim.SetBool("loop", true);
        }
        else
        {
            myUIAnim.SetBool("loop", false);
        }
    }

    public void AddShard(int number)
    {
        myShards.Add(number);
        currentShards++;
        shardsSystem.uiShards.text = currentShards.ToString();
    }
}
