using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpikes : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private float preparationTime;
    [SerializeField] private string myEnemyTag;
    [SerializeField] private float myDamage;
    [SerializeField] private GameObject impactEffect;
    private Rigidbody2D myRb;
    public Animator myAnim;
    [HideInInspector] public Vector2 initialPosition;
    [SerializeField] private AudioClip destroySfx;


    void Start()
    {
        initialPosition = transform.position;
        myRb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        GameManager.instance.StopMovementEvent += StopMovement;
        GameManager.instance.ResumeMovementEvent += ResumeMovement;
        gameObject.SetActive(false);
    }

    private void StopMovement()
    {
        myRb.velocity = Vector2.zero;
        myAnim.SetFloat("speed", 0);
    }

    private void ResumeMovement()
    {
        myRb.velocity = speed;
        myAnim.SetFloat("speed", 1);
    }
    private void Move()
    {
        transform.localScale = new Vector2(0.5f, 0.75f);
        myRb.velocity = speed;
    }

    private void ExitAnimation()
    {
        myAnim.SetTrigger("exit");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == myEnemyTag || collision.gameObject.layer == 3)
        {
            if(collision.GetComponent<Health>() != null)
                collision.GetComponent<Health>().TakeDamage(myDamage);

            if(destroySfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, destroySfx);
            Instantiate(impactEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.StopMovementEvent -= StopMovement;
        GameManager.instance.ResumeMovementEvent -= ResumeMovement;
    }
}
