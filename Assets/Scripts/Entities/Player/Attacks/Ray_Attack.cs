using System.Collections;
using UnityEngine;

public class Ray_Attack : Attack_Type
{
    public float preparationTime;
    public float castingTime;
    public float endDamage;
    private bool isCasting = false;

    public override void EndAttack()
    {
        isCasting = false;
        isAttacking = false;
        StopAllCoroutines();
    }

    public override void EnteringMode()
    {
        throw new System.NotImplementedException();
    }

    public override void PrimaryAttack()
    {
        if(!isCasting)
        {
            isAttacking = true;
            //StartCoroutine(());
        }
        else if(isCasting && !Input.GetButton("Fire1"))
        {
            EndAttack();
        }
    }

    private IEnumerator AttackTimer()
    {
        player.myAnim.SetTrigger("primaryRayPreparation");
        yield return new WaitForSeconds(preparationTime);

    }
    public override void SecondaryAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }

    public override void CreateResource()
    {
        throw new System.NotImplementedException();
    }
}
