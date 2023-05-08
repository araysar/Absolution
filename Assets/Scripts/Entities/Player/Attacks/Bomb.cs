using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour, IProjectile
{
    public Bomb_Attack myAttack;
    public Rigidbody2D myRb;
    public Bomb_Explosion myExplosionPrefab;
    private Bomb_Explosion myExplosion;
    public AudioClip myClip;

    private void Start()
    {
        myExplosion = Instantiate(myExplosionPrefab);
        myExplosion.myBomb = this;
        myExplosion.gameObject.SetActive(false);
    }

    public void Preparation()
    {
        if(!myAttack.player.isMoving)
        {
            myRb.AddForce(myAttack.player.isFacingRight? myAttack.addForce :
                new Vector2(-myAttack.addForce.x, myAttack.addForce.y), ForceMode2D.Impulse);
        }
        else
        {
            myRb.AddForce(myAttack.player.isFacingRight? myAttack.addMovingForce:
                new Vector2(-myAttack.addMovingForce.x, myAttack.addMovingForce.y), ForceMode2D.Impulse);
        }
        StartCoroutine(TimeToExplode());
    }

    public IEnumerator TimeToExplode()
    {
        yield return new WaitForSeconds(myAttack.explodeTime);
        Return();
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

    public void Return()
    {
        myExplosion.transform.position = transform.position;
        myExplosion.gameObject.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip);
        myAttack.myAttack.myCube.transform.position = transform.position;
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable myTarget = collision.gameObject.GetComponent<IDamageable>();
        if (myTarget != null)
        {
            Return();
        }
    }
}
