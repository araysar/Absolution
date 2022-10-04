using System.Collections;
using UnityEngine;

public class Dagger : Projectile
{
    [SerializeField] private Animator myAnim;
    private Rigidbody2D myRb;
    private Character_Movement myChar;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject blockedImpactEffect;
    [SerializeField] private AudioClip soundLaunch;
    public bool isRolling = true;
    public float damage = 5;
    public float speed = 3;

    private void Awake()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myAnim = GetComponent<Animator>();
        myRb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        myAnim.SetBool("isRolling", isRolling);
        myAnim.SetBool("isFacingRight", myChar.isFacingRight);
        myRb.velocity = new Vector2(myChar.isFacingRight && !myChar.isWallSliding? 1 * speed : -1 * speed, 0);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 10)
        {
            myPool.AddToPool(gameObject);
        }
        else if (collision.GetComponent<IDamageable>() != null && collision.tag != "Player")
        {
            Impact(false);
            collision.GetComponent<IDamageable>().TakeDamage(damage);

            if (collision.GetComponent<Health>().currentHP <= 0)
            {
                myChar.ulti1Stacks += collision.GetComponent<Health>().ulti1Stacks;
            }
            myPool.AddToPool(gameObject);
        }
    }

    public override void Impact(bool isBlocked)
    {
        if (!isBlocked)
        {
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
        }
        else
        {
            if(blockedImpactEffect != null)
            {
                Instantiate(blockedImpactEffect, transform.position, Quaternion.identity);
            }
        }
    }
}
