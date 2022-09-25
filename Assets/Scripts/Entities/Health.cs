using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Life")]
    public float maxHP = 10;
    public float currentHP;
    public float defense = 0;
    [SerializeField] private GameObject damagedEffect;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private bool invulnerability = false;
    private bool recovering = false;

    [Space, Header("UI")]
    public Image lifeBar;

    [Space, Header("Flash")]
    [SerializeField] private float flashTimes = 3;
    [SerializeField] private float flash1Duration = 0.2f;
    [SerializeField] private Material paintableMaterial;
    [SerializeField] private Color flashColor = Color.red;
    private SpriteRenderer myRenderer;
    private Coroutine flashCoroutine;


    private void Start()
    {
        if (!invulnerability) flashTimes = 1;
        myRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    public void TakeDamage(float dmg)
    {
        if (recovering) return;

        float totalDamage = dmg - defense;
        if(totalDamage > 0)
        {
            currentHP -= totalDamage;
            if (currentHP < 0) currentHP = 0;
            if(damagedEffect != null)
            {
                damagedEffect.SetActive(false);
                damagedEffect.SetActive(true);
            }
            
            if (lifeBar != null) RefreshLifeBar();

            if (currentHP <= 0) Death();

            if (flashCoroutine != null) return;

            flashCoroutine = StartCoroutine(Flashing());
        }
        else
        {
            //no me hace daño
        }
        
    }
        
    public void RefreshLifeBar()
    {
        lifeBar.fillAmount = currentHP / maxHP;
    }

    public void Death()
    {
        if(deathEffect != null) deathEffect.SetActive(true);

        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        if (GetComponent<Character_Movement>() != null)
        {
            GameManager.instance.RestartGame();
        }

            GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator Flashing()
    {
        if (invulnerability) recovering = true;
        int count = 0;

        Material myMaterial = myRenderer.material;
        Color myColor = myRenderer.color;

        while(count < flashTimes)
        {
            myRenderer.material = paintableMaterial;
            myRenderer.color = flashColor;
            yield return new WaitForSeconds(flash1Duration);
            myRenderer.material = myMaterial;
            myRenderer.color = myColor;
            yield return new WaitForSeconds(flash1Duration);
            count++;
        }
        if (invulnerability) recovering = false;
        flashCoroutine = null;
    }
}
