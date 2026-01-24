using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character_Attack : MonoBehaviour
{
    private Character_Movement player;
    public Animator myUIAnim;
    public bool canAttack = true;
    private bool overcharged;
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
    private Coroutine overchargeCoroutine;


    [Header("Freeze Charge")]
    public bool weaponFrozen;
    public GameObject freezeUI;
    public AudioClip freezeWeaponSfx;

    [Header("Talents System")]
    public int currentShards = 0;
    public int requiredShards = 4;
    public Shards_System shardsSystem;
    public List<Talents> myUpgrades = new List<Talents>();

    [Header("Upgrades")]
    public bool cooldownUpgrade = false;
    public bool damageUpgrade = false;
    public bool defenseUpgrade = false;
    public bool reviveUpgrade = false;

    [Header("Count Animation")]
    public TMP_Text countdownNumber;
    private Coroutine _currentRoutine;
    public int secondsToChange = 4;
    private bool isCountingDown = false;

    public enum Talents
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

    public void FreezeWeapon()
    {
        ChangeWeapon();
        freezeUI.SetActive(true);
        weaponFrozen = true;
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, freezeWeaponSfx, transform);
    }

    public void UnFreezeWeapon()
    {
        if(weaponFrozen)
        {
            ChangeWeapon();
            freezeUI.SetActive(false);
            weaponFrozen = false;
        }
    }

    public void ChangeWeapon()
    {
        int nextWeapon = Random.Range(0, myAttacks.Length);
        if (myAttacks[nextWeapon] == currentAttack && myAttacks.Length > 1)
        {
            ChangeWeapon();
        }
        else
        {
            currentAttack.EndAttack();
            currentAttack = myAttacks[nextWeapon];
            currentTime = 0;
            AttackCube(true);
            overcharged = false;
            myUIAnim.SetBool("loop", false);
            timerUI.color = emptyColor;
            currentAttack.EnteringMode();
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, changeSfx, transform);
            changeVfx.startColor = currentAttack.myColor;
            changeVfx.gameObject.SetActive(true);
            uiImage.sprite = currentAttack.myImage;
        }
    }

    private void Start()
    {
        CreateCube();
        player = GetComponent<Character_Movement>();
        if (countdownNumber) countdownNumber.gameObject.SetActive(false);
        currentTime = 0;
        currentAttack = myAttacks[Random.Range(0, myAttacks.Length)];
        AttackCube(true);
        if (damageUpgrade) myCube.overchargeEffect.SetActive(true);
        currentAttack.EnteringMode();
        overcharged = false;
        myUIAnim.SetBool("loop", false);
        timerUI.color = emptyColor;

        uiImage.sprite = currentAttack.myImage;
        shardsSystem = GetComponentInChildren<Shards_System>();
        GameManager.instance.SetupPlayerAttacks += CreateAttacks;
        GameManager.instance.SetupPlayerAttacks += CreateCube;
    }

    IEnumerator CountdownSequence()
    {
        countdownNumber.gameObject.SetActive(true);

        for (int i = secondsToChange; i > 0; i--)
        {
            countdownNumber.text = i.ToString();
            // Opcional: Aquí podrías reproducir un pequeño sonido "tic"
            yield return new WaitForSeconds(1f);
        }
        ChangeWeapon();

        // ¡Cambio! Apagamos el texto.
        countdownNumber.gameObject.SetActive(false);
        isCountingDown = false;
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
            overcharged = false; 
            myCube.animationPositions = animationPositions;
            myCube.myDestination = cubeTransform;
        }
        myCube.gameObject.transform.position = cubeTransform.position;
    }

    public void CreateAttacks()
    {
        for (int i = 0; i < myAttacks.Length; i++)
        {
            myAttacks[i].CreateResource();
        }
    }

    private void Update()
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
                if (Input.GetButtonUp("Fire1"))
                {
                    if (currentAttack.isAttacking)
                    {
                        currentAttack.Interrupt();
                    }
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    currentShards = 100;
                    shardsSystem.uiShards.text = "x " + currentShards.ToString();
                }
            }
            if(!weaponFrozen)
            {
                if (shuffleActivated)
                {
                    currentTime += Time.deltaTime;
                    if (timeToShuffle - currentTime <= secondsToChange && !isCountingDown)
                    {
                        isCountingDown = true;
                        StartCoroutine(CountdownSequence());
                    }

                    TimerUI();
                }
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
        
        if(!overcharged)
        {
            if (currentTime >= timeToShuffle - 5)
            {
                overcharged = true;
                myUIAnim.SetBool("loop", true);
                timerUI.color = fullColor;
            }
        }
    }

    public void AddShard(int number)
    {
        GameManager.instance.saveManager.shards.Add(number);
        currentShards++;
    }

    public void OverchargeEffect(bool ui)
    {
        if(ui)
        {

        }
        else
        {
            if(myCube == null)
            {
                CreateCube();
            }
            else
            {

            }
        }
    }
}
