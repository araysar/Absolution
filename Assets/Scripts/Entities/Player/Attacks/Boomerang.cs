using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Boomerang_Attack myAttack;
    public bool isBacking = false;
    public bool isFacingRight = true;
    private List<IDamageable> myTargets = new List<IDamageable>();

    void Update()
    {
        if(!isBacking)
        {
            transform.Translate(new Vector2(isFacingRight? myAttack.primarySpeed * Time.deltaTime :
                -myAttack.primarySpeed * Time.deltaTime, 0));
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, myAttack.transform.position, myAttack.primarySpeed * Time.deltaTime);
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
    }

    public IEnumerator TimeToBack()
    {
        yield return new WaitForSeconds(myAttack.backTime);
        myTargets.Clear();
        isBacking = true;
    }

    public void Flip()
    {
        if (myAttack.player.isFacingRight)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            isFacingRight = true;
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
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
                target.TakeDamage(myAttack.damage);
                myTargets.Add(target);
            }
        }
        else if(collision.gameObject.layer == 3 && !isBacking)
        {
            isBacking = true;
        }
    }
}
