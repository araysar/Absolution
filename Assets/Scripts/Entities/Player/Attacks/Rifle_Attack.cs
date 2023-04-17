using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle_Attack : Attack_Type
{
    [Header("Primary Attack")]
    public Bullet bulletPrefab;
    private List<Bullet> myBullets = new List<Bullet>();
    public float primarySpeed;
    public float timeToAttack = 0.5f;

    //[Header("Secondary Attack")]

    private void Start()
    {
        Setup();
    }

    public override void EnteringMode()
    {
        throw new System.NotImplementedException();
    }

    public override void EndAttack()
    {
        isAttacking = false;

    }

    public override void PrimaryAttack()
    {

        isAttacking = true;
        StartCoroutine(PrimaryCooldown());
    }

    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {
        for (int i = 0; i < 5; i++)
        {
            Bullet newBullet = Instantiate(bulletPrefab);
            newBullet.myAttack = this;
            newBullet.gameObject.SetActive(false);
            myBullets.Add(newBullet);
        }
    }

    private IEnumerator PrimaryCooldown()
    {
        yield return new WaitForSeconds(timeToAttack / 2);
        GetBullet().Iniciate();
        yield return new WaitForSeconds(timeToAttack / 2);
        isAttacking = false;
    }

    public Bullet GetBullet()
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
}
