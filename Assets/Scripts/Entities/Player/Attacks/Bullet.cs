using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rifle_Attack myAttack;
    public GameObject body;
    public GameObject myHitEffect;
    public AudioClip hitSFX;
    private Collider2D myCollider;
    private Rigidbody2D myRb;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        myRb = GetComponent<Rigidbody2D>();
    } 

    private void Iniciate()
    {
        body.SetActive(true);
        Flip();
        myCollider.enabled = true;
        transform.Translate(new Vector2(myAttack.player.isFacingRight ? myAttack.primarySpeed * Time.deltaTime :
            -myAttack.primarySpeed * Time.deltaTime, 0));
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

    private void OnHit()
    {
        myHitEffect.SetActive(true);
        body.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null && collision.gameObject.layer != myAttack.player.gameObject.layer)
        {
            target.TakeDamage(myAttack.damage);
            OnHit();
        }
        else if (collision.gameObject.layer == 3)
        {
            OnHit();
        }
    }
}
