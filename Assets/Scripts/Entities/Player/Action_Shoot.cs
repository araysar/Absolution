using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Action_Shoot : MonoBehaviour
{
    [SerializeField] private GameObject myBullet;
    [SerializeField] private int startingBullets = 3;
    [SerializeField] private Transform shootingPoint;
    private Animator myAnim;
    private Character_Movement myChar;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
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
        myAnim = GetComponent<Animator>();
        myChar = GetComponent<Character_Movement>();
        GrowPool(startingBullets);

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
        availableObjects.Enqueue(instance);
    }

    public void GetFromPool()
    {
        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, daggerLaunchSfx);
        instance.transform.position = shootingPoint.position;
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
        pyroSphere.transform.position = shootingPoint.transform.position;
    }

    #endregion

    public void FinishShooting(string animName)
    {
        myAnim.SetBool(animName, false);
        isAttacking = false;
    }
}
