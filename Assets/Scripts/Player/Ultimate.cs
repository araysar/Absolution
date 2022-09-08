using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ultimate: MonoBehaviour
{
    public bool canUse = true;
    public float cooldownTime = 10;
    public float cooldownLive = 0;
    public float chargingTime = 1.5f;
    public float skillTime = 2.5f;

    [SerializeField] private GameObject chargeEffect;
    [SerializeField] private GameObject skillEffect;

    [SerializeField] private TMP_Text uiText;
    [SerializeField] private Image uiImage;

    private Character_Movement charMove;
    void Start()
    {
        charMove = GetComponentInParent<Character_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldownLive > 0)
        {
            if(!uiImage.gameObject.activeInHierarchy)
            {
                uiImage.gameObject.SetActive(true);
            }
            cooldownLive -= Time.deltaTime;
            uiText.text = Mathf.RoundToInt(cooldownLive).ToString();
            uiImage.fillAmount = cooldownLive / cooldownTime;
        }
        else
        {
            if (uiImage.gameObject.activeInHierarchy)
            {
                uiImage.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateUltimate()
    {
        if(canUse)
        {
            if (!charMove.disableInputs)
            {
                if (cooldownLive <= 0 && GetComponent<Animator>().GetBool("isAttacking") == false)
                {
                    StartCoroutine(UsingUltimate());
                }
            }
        }
    }

    private IEnumerator UsingUltimate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().enabled = false;
        charMove.disableInputs = true;
        charMove.isCharging = true;
        rb.velocity = Vector2.zero;
        float gravity = rb.gravityScale;
        rb.gravityScale = 0;
        chargeEffect.SetActive(true);
        cooldownLive = cooldownTime;
        charMove.ControlAnimations();

        yield return new WaitForSeconds(chargingTime);

        charMove.isCharging = false;
        charMove.isUlting = true;
        skillEffect.SetActive(true);
        chargeEffect.SetActive(false);
        charMove.ControlAnimations();

        yield return new WaitForSeconds(skillTime);

        charMove.isUlting = false;
        charMove.disableInputs = false;
        skillEffect.SetActive(false);
        rb.gravityScale = gravity;
        GetComponent<Collider2D>().enabled = true;
    }


}
