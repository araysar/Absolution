using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Life")]
    public float maxHP = 10;
    public float currentHP;
    [SerializeField] private GameObject damagedEffect;
    [SerializeField] private GameObject deathEffect;

    [Space, Header("UI")]
    public Image lifeBar;

    [Space, Header("Invulnerability")]
    [SerializeField] private bool invulnerability = false;
    [SerializeField] private float flashTimes = 3;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;
    [HideInInspector] public Coroutine invulnerableCoroutine;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float dmg)
    {
        if (invulnerableCoroutine != null) return;

        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;
        damagedEffect.SetActive(false);
        damagedEffect.SetActive(true);
        if (lifeBar != null) RefreshLifeBar();

        if (currentHP <= 0) Death();

        if (invulnerability)
        {
            invulnerableCoroutine = StartCoroutine(Invulnerable());
        }
    }
        
    public void RefreshLifeBar()
    {
        lifeBar.fillAmount = currentHP / maxHP;
    }

    public void Death()
    {
        if(deathEffect != null) deathEffect.SetActive(true);

        if(GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            if(GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().enabled = false;
            }
            if(GetComponent<Rigidbody2D>() != null)
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }

            if(GetComponent<Character_Movement>() != null)
            {
                GameManager.instance.RestartGame();
            }
        }
        else
        {
            Destroy(gameObject);
        }   
    }

    private IEnumerator Invulnerable()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color rendererColor = renderer.color;
        int count = 0;
        while(count < flashTimes)
        {
            renderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            renderer.color = rendererColor;
            yield return new WaitForSeconds(flashDuration);
            count++;
        }

        renderer.color = rendererColor;
        invulnerableCoroutine = null;
    }

    
}
