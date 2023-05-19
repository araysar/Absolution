using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IProjectile
{
    public Rifle_Attack myAttack;
    public GameObject body;
    public GameObject myHitEffect;
    public AudioClip hitSFX;
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private Rigidbody2D myRb;


    public void Iniciate()
    {
        body.SetActive(true);
        Flip();
        transform.position = myAttack.transform.position;
        myCollider.enabled = true;
        myRb.velocity = new Vector2((myAttack.player.isFacingRight? myAttack.primarySpeed: 
            -myAttack.primarySpeed), 0);
    }

    public void Flip()
    {
        if (myAttack.player.isFacingRight)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if(collision.GetComponent<BlockDamage>() != null)
        {
            Return();
        }
        else if (target != null && collision.gameObject.layer != myAttack.player.gameObject.layer)
        {
            target.TakeDamage(myAttack.myAttack.damageUpgrade ? myAttack.damage * 1.5f : myAttack.damage);
            Return();
        }
        else if (collision.gameObject.layer == 3)
        {
            Return();
        }
        else if(collision.gameObject.layer == 10)
        {
            myCollider.enabled = false;
            myRb.velocity = Vector2.zero;
            body.SetActive(false);
        }
    }

    public void Return()
    {
        myHitEffect.SetActive(true);
        myCollider.enabled = false;
        myRb.velocity = Vector2.zero;
        body.SetActive(false);
        if (hitSFX != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, hitSFX, transform);
    }
}
