using System.Collections.Generic;
using UnityEngine;

public class Action_Shoot : MonoBehaviour
{
    [SerializeField] private GameObject myBullet;
    [SerializeField] private Transform shootingPoint;
    private Animator myAnim;
    private Character_Movement myChar;
    private List<GameObject> availableObjects = new List<GameObject>();
    public bool canShoot = true;
    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isAttacking = false;
    GameObject pool;
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
        myAnim = GetComponent<Animator>();
        myChar = GetComponent<Character_Movement>();
        GrowPool(3);

        pyroSphere = Instantiate(pyroPrefab, shootingPoint.transform.position, Quaternion.identity);
        pyroSphere.GetComponent<PyroSphere_Launch>().myShooter = this;
        pyroSphere.SetActive(false);
    }

    private void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if (!isAttacking && !myChar.disableInputs)
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
        pool = new GameObject("Projectile Pool");
        availableObjects = new List<GameObject>();

        for (int i = 0; i < daggers; i++)
        {
            var instanceToAdd = Instantiate(myBullet);
            instanceToAdd.GetComponent<Projectile>().myPool = this;
            instanceToAdd.transform.parent = pool.transform;
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Add(instance);
    }

    public GameObject GetFromPool()
    {
        
        for (int i = 0; i < availableObjects.Count; i++)
        {
            if(!availableObjects[i].activeSelf)
            {
                availableObjects[i].SetActive(true);
                return availableObjects[i];
            }
        }
        return null;
    }


    private void ShootDagger()
    {
        if (!canShoot) return;

        if (isAttacking) return;

        if (pool == null) GrowPool(3);
        GameObject instance = GetFromPool();
        if (instance == null) return;

        else
        {
            myChar.StopDash();
            isAttacking = true;
            myAnim.SetBool("attack1", true);
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, daggerLaunchSfx);
            instance.transform.position = shootingPoint.position;
        }
    }

    #endregion

    #region PyroSphere

    private void ShootPyroSphere()
    {
        if (!canShoot) return;

        if (!myChar.myUpgrades.Contains(Character_Movement.PowerUp.Fire)) return;

        if (!pyroReady) return;

        if (!myChar.myEnergy.CanUse(pyroEnergy)) return;

        if (isAttacking) return;

        myChar.StopDash();
        pyroReady = false;
        pyroAnimator.SetTrigger("notReady");
        myAnim.SetBool("attackFire", true);
        isAttacking = true;
    }

    public void LaunchPyroSphere()
    {
        if(pyroSphere == null)
        {
            pyroSphere = Instantiate(pyroPrefab, shootingPoint.transform.position, Quaternion.identity);
            pyroSphere.GetComponent<PyroSphere_Launch>().myShooter = this;
        }
        pyroSphere.SetActive(true);
        myChar.myEnergy.currentEnergy -= pyroEnergy;
        myChar.myEnergy.ReloadEnergy();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, pyroLaunchSfx);
        pyroSphere.transform.position = shootingPoint.transform.position;
    }
    
    public void RecoverPyroSphere()
    {
        pyroReady = true;
        pyroAnimator.SetTrigger("ready");
    }

    #endregion

    public void FinishShooting()
    {

        myAnim.SetBool("attack1", false);
        myAnim.SetBool("attackFire", false);
        isAttacking = false;
    }
}
