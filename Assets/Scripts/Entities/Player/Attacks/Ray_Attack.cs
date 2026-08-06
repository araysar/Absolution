using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Ray_Attack : Attack_Type
{

    List<Health> targets = new List<Health>();
    public float maxTime;
    public float timeSinceStart;
    public float maxDamage;
    public ParticleSystem myVFX;
    public ParticleSystem maxVFX;
    private float currentGravity;
    public float rayWidth;
    public float rayheight;
    private float ySpeed;
    private Collider2D myCollider;
    public LayerMask flyType;
    public LayerMask bossType;

    public override void EndAttack()
    {
        StopAllCoroutines();
        if (isAttacking) Interrupt();
        targets.Clear();
    }

    public override void EnteringMode()
    {

    }

    public override void PrimaryAttack()
    {
        ySpeed = player.rb.velocity.y;
        myAttack.AttackCube(false);
        player.myAnim.ResetTrigger("primaryRayEnd");
        player.myAnim.SetBool("isAttacking", true);
        player.myAnim.SetTrigger("primaryRay");
        if (ySpeed > 0) ySpeed = 0;
        player.rb.velocity = Vector2.zero;
        player.rb.gravityScale = 0;
        player.isChanneling = true;
        isAttacking = true;
        timeSinceStart = 0;
        myCollider.enabled = false;
        myCollider.enabled = true;
        myVFX.gameObject.SetActive(true);
        maxVFX.gameObject.SetActive(false);
    }
    public override void SecondaryAttack()
    {

    }

    public override void Setup()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        myVFX.gameObject.SetActive(false);
        currentGravity = player.rb.gravityScale;
        myCollider = GetComponent<Collider2D>();
    }

    public override void Interrupt()
    {
        myAttack.AttackCube(true);
        player.rb.velocity = new Vector2(0, ySpeed);
        player.rb.gravityScale = currentGravity;
        isAttacking = false;
        player.isChanneling = false;
        targets.Clear();
        player.myAnim.SetTrigger("primaryRayEnd");
        player.myAnim.SetBool("isAttacking", false); 
        maxVFX.gameObject.SetActive(false);
        myVFX.gameObject.SetActive(false);
    }

    public override void CreateResource()
    {

    }

    protected void Update()
    {
        base.Update();
        if (isAttacking)
        {
            if (timeSinceStart < maxTime)
            {
                timeSinceStart += Time.deltaTime;
            }
            else if (timeSinceStart > maxTime)
            {
                timeSinceStart = maxTime;
                maxVFX.gameObject.SetActive(true);
            }

            currentEnergy -= energyPerShot * Time.deltaTime;

            float energyPercentage = currentEnergy / maxEnergy;

            float outputDamage = Mathf.Lerp(damage / 4f, damage * 1.25f, energyPercentage);
            if (myAttack.damageUpgrade) outputDamage *= 1.5f;

            float outputDamageMax = Mathf.Lerp(maxDamage / 4f, maxDamage * 1.25f, energyPercentage);
            if (myAttack.damageUpgrade) outputDamageMax *= 1.5f;

            if (targets.Count > 0)
            {
                foreach (var target in targets)
                {
                    if(target.type == flyType)
                        target.TakeDamage(timeSinceStart == maxTime ? outputDamageMax * Time.deltaTime * 1.5f: outputDamage * Time.deltaTime * 2);
                    if(target.type == bossType)
                        target.TakeDamage(timeSinceStart == maxTime ? outputDamageMax * Time.deltaTime / 1.5f : outputDamage * Time.deltaTime / 2f);
                    else
                        target.TakeDamage(timeSinceStart == maxTime ? outputDamageMax * Time.deltaTime : outputDamage * Time.deltaTime);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health newTarget = collision.GetComponent<Health>();
        if (newTarget != null && collision.gameObject.layer != player.gameObject.layer)
        {
            if (!targets.Contains(newTarget)) targets.Add(newTarget);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Health newTarget = collision.GetComponent<Health>();
        if (newTarget != null && collision.gameObject.layer != player.gameObject.layer)
        {
            if (targets.Contains(newTarget)) targets.Remove(newTarget);
        }
    }
}
