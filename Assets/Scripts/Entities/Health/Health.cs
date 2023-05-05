using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Life")]
    public float maxHP = 10;
    public float currentHP;
    public bool invulnerable = false;
    protected bool recovering = false;
    public Animator myAnim;
    [HideInInspector] public Vector2 initialPosition;
    [SerializeField] private List<GameObject> disableAfterDeath;
    [SerializeField] private GameObject healVfx;
    [SerializeField] private AudioClip healSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private AudioClip damagedSfx;
    [SerializeField] private GameObject damagedVfx;

    [Space, Header("Flash")]
    [SerializeField] protected Material paintableMaterial;
    [SerializeField] protected Material commonMaterial;
    [SerializeField] private Color flashColor = Color.white;
    [HideInInspector] public SpriteRenderer myRenderer;
    protected Coroutine flashCoroutine;

    [Space, Header("Drop")]
    [SerializeField] private GameObject myDrop;


    private void Awake()
    {
        initialPosition = transform.position;
        myRenderer = GetComponent<SpriteRenderer>();
        if (myAnim == null) myAnim = GetComponent<Animator>();
        commonMaterial = myRenderer.material;
        currentHP = maxHP;
    }

    public virtual void TakeDamage(float dmg)
    {
        if (recovering || currentHP <= 0) return;

        float totalDamage = dmg;
        if (!invulnerable)
        {
            currentHP -= totalDamage;
            if (currentHP < 0) currentHP = 0;

            if (damagedVfx != null)
            {
                damagedVfx.SetActive(false);
                damagedVfx.SetActive(true);
            }

            if(myAnim.GetBool("damage") == true)
            {
                myAnim.SetBool("damaged", true);
            }

            if(currentHP > 0)
            {
                if (damagedSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, damagedSfx);
            }
            else
            {
                if (deathSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, deathSfx);
            }
        }
        else
        {
            Invulnerable();
        }
    }

    public virtual void Invulnerable()
    {

    }

    public virtual void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, healSfx);
        healVfx.SetActive(false);
        healVfx.SetActive(true);
    }

    public virtual void Death()
    {
        StopAllCoroutines();
        flashCoroutine = null;
        recovering = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        myRenderer.material = commonMaterial;
        myAnim.SetTrigger("death");
    }

    protected void Disable()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator Flashing(int times, float duration)
    {
        int count = 0;

        Material myMaterial = myRenderer.material;
        Color myColor = myRenderer.color;

        recovering = true;
        while(count < times)
        {
            myRenderer.material = paintableMaterial;
            myRenderer.color = flashColor;
            yield return new WaitForSeconds(duration);
            myRenderer.material = myMaterial;
            myRenderer.color = myColor;
            yield return new WaitForSeconds(duration);
            count++;
        }

        if (currentHP <= 0)
        {
            Death();
        }
        recovering = false;
        flashCoroutine = null;
    }
}