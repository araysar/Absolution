using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Character_Movement player;
    public Bomb_Attack myAttack;
    public Rigidbody2D myRb;
    public Bomb_Explosion myExplosionPrefab;
    private Bomb_Explosion myExplosion;

    public bool isFacingRight = true;

    private void Start()
    {
        myExplosion = Instantiate(myExplosionPrefab);
        myExplosion.myBomb = this;
        myExplosion.gameObject.SetActive(false);
    }

    public void Preparation()
    {
        if(!player.isMoving)
        {
            myRb.AddForce(isFacingRight? myAttack.addForce :
                new Vector2(-myAttack.addForce.x, myAttack.addForce.y), ForceMode2D.Impulse);
        }
        else
        {
            myRb.AddForce(isFacingRight? myAttack.addMovingForce:
                new Vector2(-myAttack.addMovingForce.x, myAttack.addMovingForce.y), ForceMode2D.Impulse);
        }
        StartCoroutine(TimeToExplode());
    }

    public IEnumerator TimeToExplode()
    {
        yield return new WaitForSeconds(myAttack.explodeTime);
        Explode();
    }

    public void Flip()
    {
        if (isFacingRight)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            isFacingRight = true;
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            isFacingRight = false;
        }
    }

    public void Explode()
    {
        myExplosion.transform.position = transform.position;
        myExplosion.gameObject.SetActive(true);
        myAttack.myAttack.myCube.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable myTarget = collision.gameObject.GetComponent<IDamageable>();
        if (myTarget != null)
        {
            Explode();
        }
    }
}
