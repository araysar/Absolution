using System.Collections;
using UnityEngine;

public class Dagger : Projectile
{
    [SerializeField] private Animator myAnim;
    private Rigidbody2D myRb;
    private Character_Movement myChar;
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
        myRb.velocity = new Vector2(myChar.isFacingRight? 1 * speed : -1 * speed, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            myPool.AddToPool(gameObject);
        }
        else if ((collision.gameObject.layer == 3 || collision.gameObject.layer == 11) && !collision.GetComponent<Health>())
        {
            Impact(Health.ArmorType.wall);
            myPool.AddToPool(gameObject);
        }
        else if(collision.GetComponent<BlockDamage>() != null)
        {
            Impact(Health.ArmorType.shield);
        }
        else if (collision.GetComponent<IDamageable>() != null && collision.tag != "Player")
        {
            Impact(collision.GetComponent<Health>().myArmor);

            collision.GetComponent<IDamageable>().TakeDamage(damage);

            if (collision.GetComponent<Health>().currentHP <= 0)
            {
                if(!myChar.ulti1.ultiReady)
                {
                    myChar.ulti1Stacks += collision.GetComponent<Health>().ulti1Stacks;
                }
                myChar.ulti1.RefreshStacks();
            }
            myPool.AddToPool(gameObject);
        }
    }

    public override void Impact(Health.ArmorType armorType)
    {
        switch (armorType)
        {
            case Health.ArmorType.flesh:
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, Character_Movement.instance.impactFleshSfx);
                break;
            case Health.ArmorType.metal:
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, Character_Movement.instance.impactMetalSfx);
                break;
            case Health.ArmorType.shield:
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, Character_Movement.instance.impactShieldSfx);
                Instantiate(myChar.blockedWallImpactEffect, transform.position, Quaternion.identity);
                break;
            case Health.ArmorType.wall:
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, Character_Movement.instance.blockedWallImpactSfx);
                Instantiate(myChar.blockedWallImpactEffect, transform.position, Quaternion.identity);
                break;
            default:
                break;
        }
    }
}