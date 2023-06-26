using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour, IProjectile
{
    public Boomerang_Attack myAttack;
    public bool isBacking = false;
    public bool isFacingRight = true;
    private List<IDamageable> myTargets = new List<IDamageable>();
    private Collider2D myCollider;
    public AudioSource myAudio;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        myAudio = GetComponent<AudioSource>();
    }
    private void Start()
    {
        myAudio.volume = SoundManager.instance.sfxVolume;
    }

    void Update()
    {
        if(!isBacking)
        {
            transform.Translate(new Vector2(myAttack.myAttack.cooldownUpgrade? myAttack.primarySpeed * 1.5f * Time.deltaTime
                                           : myAttack.primarySpeed * Time.deltaTime, 0));
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, myAttack.transform.position,
            myAttack.myAttack.cooldownUpgrade ? myAttack.primarySpeed * 1.5f * Time.deltaTime : myAttack.primarySpeed * Time.deltaTime);
            if((myAttack.transform.position - transform.position).magnitude < 0.1f)
            {
                StopAllCoroutines();
                myTargets.Clear();
                myAttack.myAttack.AttackCube(true);
                myAttack.EndAttack();
            }
        }
    }

    public void Timer()
    {
        StartCoroutine(TimeToBack());
        myAudio.Play();
    }

    public IEnumerator TimeToBack()
    {
        yield return new WaitForSeconds(myAttack.myAttack.cooldownUpgrade ? myAttack.backTime / 1.5f : myAttack.backTime);
        Return();
    }

    public void Flip()
    {
        if (myAttack.player.isFacingRight)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if(target != null && collision.gameObject.layer != myAttack.player.gameObject.layer)
        {
            if(!myTargets.Contains(target))
            {
                target.TakeDamage(myAttack.myAttack.damageUpgrade ? myAttack.damage * 1.5f : myAttack.damage);
                myTargets.Add(target);
            }
        }
        else if(collision.gameObject.layer == 3 && !isBacking)
        {
            Return();
        }
    }

    public void Return()
    {
        StopAllCoroutines();
        isBacking = true;
        myTargets.Clear();
        myCollider.enabled = false;
        myCollider.enabled = true;
        myAudio.Stop();
        myAudio.Play();
    }
}
