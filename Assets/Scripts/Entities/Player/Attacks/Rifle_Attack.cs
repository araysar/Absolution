using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public Rifle_Shot bulletPrefab;
    private List<Rifle_Shot> myBullets = new List<Rifle_Shot>();
    public float primarySpeed;
    public float timeToAttack = 0.5f;
    public AudioClip myClip;

    //[Header("Secondary Attack")]

    private void Start()
    {
        Setup();
    }

    public override void EnteringMode()
    {

    }

    public override void EndAttack()
    {
        isAttacking = false;

    }

    public override void PrimaryAttack()
    {
        isAttacking = true;
        player.myAnim.SetBool("isAttacking", true);
        StartCoroutine(PrimaryCooldown());
    }

    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {
        myBullets = new List<Rifle_Shot>();
        for (int i = 0; i < 5; i++)
        {
            Rifle_Shot newBullet = Instantiate(bulletPrefab);
            newBullet.myAttack = this;
            newBullet.gameObject.SetActive(false);
            myBullets.Add(newBullet);
        }
    }

    private IEnumerator PrimaryCooldown()
    {
        player.myAnim.SetTrigger("primaryRifle");
        myAttack.AttackCube(false);
        GetBullet().Iniciate();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip, transform);
        yield return new WaitForSeconds(myAttack.cooldownUpgrade? timeToAttack / 1.5f : timeToAttack);
        player.myAnim.SetBool("isAttacking", false);
        myAttack.AttackCube(true);
        isAttacking = false;
    }

    public Rifle_Shot GetBullet()
    {
        for (int i = 0; i < myBullets.Count; i++)
        {
            if(!myBullets[i].gameObject.activeSelf)
            {
                myBullets[i].gameObject.SetActive(true);
                return myBullets[i];
            }
        }
        Setup();
        myBullets[myBullets.Count - 1].gameObject.SetActive(true);
        return myBullets[myBullets.Count - 1];
    }

    public override void Interrupt()
    {

    }

    public override void CreateResource()
    {
        Setup();
    }
}
