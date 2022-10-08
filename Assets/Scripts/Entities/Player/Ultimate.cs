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
    [SerializeField] private Animator ultimateAnimator;

    [SerializeField] private Image uiImage;
    [SerializeField] private Image uiWhiteImage;
    [SerializeField] private GameObject readyText;
    public bool ultiReady = true;

    private Character_Movement myChar;
    void Start()
    {
        myChar = GetComponentInParent<Character_Movement>();

        if (myChar.ulti1Stacks >= myChar.ulti1Required)
        {
            myChar.ulti1Stacks = myChar.ulti1Required;
            readyText.SetActive(true);
        }
        else
        {
            readyText.SetActive(false);
        }
        uiWhiteImage.fillAmount = myChar.ulti1Stacks / myChar.ulti1Required;
        uiImage.fillAmount = myChar.ulti1Stacks / myChar.ulti1Required;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if(!myChar.disableInputs)
            {
                if (Input.GetButtonDown(inputName) && myChar.myUpgrades.Contains(Character_Movement.PowerUp.Ulti1))
                {
                    ActivateUltimate();
                }
            }
        }
    }

    public void RefreshStacks()
    {
        if(myChar.ulti1Stacks >= myChar.ulti1Required && !ultiReady)
        {
            myChar.ulti1Stacks = myChar.ulti1Required;
            uiWhiteImage.fillAmount = 1;
            uiImage.fillAmount = 1;
            if (!ultiReady) ultiReady = true;
            readyText.SetActive(true);
            ultimateAnimator.SetTrigger("ready");
        }
        else if(myChar.ulti1Stacks <= myChar.ulti1Required && !ultiReady)
        {
            uiImage.fillAmount = myChar.ulti1Stacks / myChar.ulti1Required;
            uiWhiteImage.fillAmount = uiImage.fillAmount;
            ultimateAnimator.SetTrigger("notReady");
        }
    }

    public void ActivateUltimate()
    {
        if(canUse && ultiReady)
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
        myChar.isJumping = false;
        myChar.disableInputs = true;
        myChar.isCharging = true;
        rb.velocity = Vector2.zero;
        float gravity = rb.gravityScale;
        rb.gravityScale = 0;
        chargeEffect.SetActive(true);
        ultiReady = false;
        RefreshStacks();
        ultimateAnimator.SetTrigger("notReady");
        readyText.SetActive(false);
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
