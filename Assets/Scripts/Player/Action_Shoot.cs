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
    [SerializeField] private string animatorBoolName = "attack1";
    [SerializeField] private string inputName = "Fire1";
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    public bool canShoot = true;
    [HideInInspector] public bool isShooting = false;
    GameObject pool;
    private bool isAttacking = false;

    private void Start()
    {
        pool = new GameObject("Daggers");
        myAnim = GetComponent<Animator>();
        myChar = GetComponent<Character_Movement>();
        GrowPool(startingBullets);
    }

    private void Update()
    {
        if(Input.GetButtonDown(inputName))
        {
            ShootDagger();
        }
        AnimationControl();
    }
     private void AnimationControl()
    {
        myAnim.SetBool("canAttack", isAttacking);
        myAnim.SetBool(animatorBoolName, isAttacking);
        myAnim.SetBool("isAttacking", isAttacking);
    }

    private void GrowPool(int daggers)
    {
        for (int i = 0; i < daggers; i++)
        {
            var instanceToAdd = Instantiate(myBullet);
            instanceToAdd.GetComponent<Dagger>().myPool = this;
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
        instance.transform.position = shootingPoint.position;
    }

    private void ShootDagger()
    {
        if (!canShoot) return;

        if (availableObjects.Count <= 0) return;

        if (isAttacking) return;

        isAttacking = true;
    }

    public void FinishShooting()
    {
        isAttacking = false;
    }
}
