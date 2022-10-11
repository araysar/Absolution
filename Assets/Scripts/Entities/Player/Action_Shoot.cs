using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Action_Shoot : MonoBehaviour
{
    [SerializeField] private GameObject myBullet;
    [SerializeField] private Transform shootingPoint;
    private Animator myAnim;
    private Character_Movement myChar;
    private List<GameObject> availableObjects = new List<GameObject>();
    public bool canShoot = true;
    [HideInInspector] public bool isShooting = false;
    GameObject pool;
    private bool isAttacking = false;
    public AttackType currentAttack;

    [Header("PyroSphere")]
    [SerializeField] private GameObject pyroPrefab;
    public Animator pyroAnimator;
    private GameObject pyroSphere;
    public bool pyroReady = true;
    [SerializeField] private float pyroEnergy = 30;


    [Header("SFX")]
    [SerializeField] private AudioClip daggerLaunchSfx;
    [SerializeField] private AudioClip pyroLaunchSfx;

    public enum AttackType
    {
        FrozenDaggers,
        PyroSphere,

        
    }

    private void Awake()
    {
        pool = new GameObject("Projectile Pool");
        DontDestroyOnLoad(pool);
        myAnim = GetComponent<Animator>();
        myChar = GetComponent<Character_Movement>();
        GrowPool(5);

        pyroSphere = Instantiate(pyroPrefab, shootingPoint.transform.position, Quaternion.identity);
        pyroSphere.GetComponent<PyroSphere_Launch>().myShooter = this;
        pyroSphere.SetActive(false);
    }

    private void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if (!isAttacking)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    ShootDagger();
                }
                else if (Input.GetButtonDown("Fire2"))
                {
                    switch (currentAttack)
                    {
                        case AttackType.FrozenDaggers:
                            break;
                        case AttackType.PyroSphere:
                            ShootPyroSphere();
                            break;
                        default:
                            break;
                    }
                }
            }
            AnimationControl();
        }
    }

    private void AnimationControl()
    {
        myAnim.SetBool("canAttack", isAttacking);
        myAnim.SetBool("isAttacking", isAttacking);
    }

    #region Normal Dagger
    public void GrowPool(int daggers)
    {
        for (int i = 0; i < daggers; i++)
        {
            var instanceToAdd = Instantiate(myBullet);
            instanceToAdd.GetComponent<Projectile>().myPool = this;
            instanceToAdd.transform.SetParent(pool.transform);
            AddToPool(instanceToAdd);
        }
        
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Add(instance);
    }

    public void GetFromPool()
    {
        if (availableObjects.Count > 0)
        {
            var instance = availableObjects[0];
            availableObjects.RemoveAt(0);
            instance.SetActive(true);
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, daggerLaunchSfx);
            instance.transform.position = shootingPoint.position;
        }

        else
        {
            GrowPool(1);
            GetFromPool();
        }
    }

    private void ShootDagger()
    {
        if (!canShoot) return;

        if (availableObjects.Count <= 0) return;

        if (isAttacking) return;

        myChar.StopDash();
        myAnim.SetBool("attack1", true);
        isAttacking = true;
    }

    #endregion

    #region PyroSphere

    private void ShootPyroSphere()
    {
        if (!canShoot) return;

        if (!myChar.myUpgrades.Contains(Character_Movement.PowerUp.Fire)) return;

        if (!pyroReady) return;

        if (myChar.myEnergy.currentEnergy < pyroEnergy) return;

        if (isAttacking) return;

        myChar.StopDash();
        pyroReady = false;
        pyroAnimator.SetTrigger("notReady");
        myAnim.SetBool("attackFire", true);
        isAttacking = true;
    }

    public void LaunchPyroSphere()
    {
        pyroSphere.SetActive(true);
        myChar.myEnergy.currentEnergy -= pyroEnergy;
        myChar.myEnergy.ReloadEnergy();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, pyroLaunchSfx);
        pyroSphere.transform.position = shootingPoint.transform.position;
    }

    #endregion

    public void FinishShooting(string animName)
    {
        myAnim.SetBool(animName, false);
        isAttacking = false;
    }
}
