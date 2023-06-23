using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public Animator myAnim;
    protected Rigidbody2D myRb;
    [SerializeField] protected Health myHealth;
    protected BossFightManager myManager;
    
    void Awake()
    {
        myAnim = GetComponent<Animator>();
        myRb = GetComponent<Rigidbody2D>();
        if(myHealth == null) myHealth = GetComponent<Health>();
        myManager = FindObjectOfType<BossFightManager>();
    }

    public abstract void StartingFight();
    public abstract void GenerateShield();
    public abstract void SelectAttack();
    public abstract void FinishAttack(float time);
    public abstract void Respawn();
    public abstract void StopMovement();
    public abstract void ResumeMovement();
}
