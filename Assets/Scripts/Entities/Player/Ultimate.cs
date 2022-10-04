using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ultimate: MonoBehaviour
{
    public bool canUse = true;
    public float chargingTime = 1f;
    public float skillTime = 2.5f;

    [SerializeField] private GameObject chargeEffect;
    [SerializeField] private GameObject skillEffect;
    [SerializeField] private string inputName = "Ultimate1";

    [SerializeField] private Image uiImage;
    [SerializeField] private GameObject readyText;

    private Character_Movement myChar;
    void Start()
    {
        myChar = GetComponentInParent<Character_Movement>();
        RefreshStacks();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if(!myChar.disableInputs)
            {
                if (Input.GetButtonDown(inputName))
                {
                    ActivateUltimate();
                }
            }
        }
    }

    public void RefreshStacks()
    {
        if(myChar.ulti1Stacks >= myChar.ulti1Required)
        {
            myChar.ulti1Stacks = myChar.ulti1Required;
            if(uiImage.gameObject.activeInHierarchy) uiImage.gameObject.SetActive(false);
            if(!readyText.activeInHierarchy) readyText.SetActive(true);
        }
        else
        {
            if(!uiImage.gameObject.activeInHierarchy) uiImage.gameObject.SetActive(true);
            if (readyText.activeInHierarchy) readyText.SetActive(false);
            float alf = myChar.ulti1Stacks / myChar.ulti1Required;
            uiImage.fillAmount = 1 - alf;
        }
        
    }

    public void ActivateUltimate()
    {
        if(canUse && myChar.ulti1Stacks >= myChar.ulti1Required)
        {
            if (GetComponent<Animator>().GetBool("isAttacking") == false)
            {
                StartCoroutine(UsingUltimate());
            }
        }
    }

    private IEnumerator UsingUltimate()
    {
        myChar.StopDash();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float playerDefense = GetComponent<Health>().defense;
        GetComponent<Health>().defense = 9999;
        myChar.ulti1Stacks = 0;
        RefreshStacks();
        myChar.isJumping = false;
        myChar.disableInputs = true;
        myChar.isCharging = true;
        rb.velocity = Vector2.zero;
        float gravity = rb.gravityScale;
        rb.gravityScale = 0;
        chargeEffect.SetActive(true);
        myChar.ControlAnimations();

        yield return new WaitForSeconds(chargingTime);

        myChar.isCharging = false;
        myChar.isUlting = true;
        skillEffect.SetActive(true);
        chargeEffect.SetActive(false);
        myChar.ControlAnimations();

        yield return new WaitForSeconds(skillTime);

        myChar.isUlting = false;
        myChar.disableInputs = false;
        skillEffect.SetActive(false);
        rb.gravityScale = gravity;
        GetComponent<Health>().defense = playerDefense;
    }


}
