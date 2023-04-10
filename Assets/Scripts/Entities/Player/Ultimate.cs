using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ultimate: MonoBehaviour
{
    public bool canUse = true;
    public float chargingTime = 1f;
    public float skillTime = 2.5f;
    private float defenseBeforeUltimate;
    [SerializeField] private float regenerationDelay = 0.5f;
    [SerializeField] private float regenerationPerTick = 1;
    [SerializeField] private string inputName = "Ultimate1";
    [SerializeField] private Animator ultimateAnimator;

    [SerializeField] private Image uiImage;
    [SerializeField] private Image uiWhiteImage;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject readyVfx;

    [SerializeField] private AudioClip chargeSfx;
    [SerializeField] private AudioClip launchSfx;
    [SerializeField] private AudioClip castVoice;
    [SerializeField] private AudioClip launchVoice;
    Coroutine regenerationAction;

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

        if(ultiReady && myChar.ulti1Stacks >= myChar.ulti1Required)
        {
            ultimateAnimator.SetBool("isFull", ultiReady);
        }
    }

    private void Update()
    {
        if (regenerationAction == null)
        {
            regenerationAction = StartCoroutine(Regeneration());
        }
    }

    public void RefreshStacks(bool changeState)
    {
        if(myChar.ulti1Stacks >= myChar.ulti1Required && !ultiReady)
        {
            myChar.ulti1Stacks = myChar.ulti1Required;
            uiWhiteImage.fillAmount = 1;
            uiImage.fillAmount = 1;
            ultiReady = true;
            readyText.SetActive(true);
            ultimateAnimator.SetTrigger("ready");
            ultimateAnimator.SetBool("isFull", ultiReady);
        }
        else if(myChar.ulti1Stacks <= myChar.ulti1Required && !ultiReady)
        {
            ultimateAnimator.SetBool("isFull", ultiReady);
            readyText.SetActive(false);
            uiImage.fillAmount = myChar.ulti1Stacks / myChar.ulti1Required;
            uiWhiteImage.fillAmount = uiImage.fillAmount;
            if(changeState) ultimateAnimator.SetTrigger("notReady");
        }
    }

    public void ActivateUltimate()
    {
        if(canUse && ultiReady)
        {
            myChar.isCharging = true;
            ultiReady = false;
            GameManager.instance.TriggerAction(GameManager.ExecuteAction.StopMovementEvent);
            myChar.ControlAnimations();
        }
    }

    IEnumerator Regeneration()
    {
        yield return new WaitForSeconds(regenerationDelay);
        
        if (!myChar.ulti1.ultiReady)
        {
            myChar.ulti1Stacks += regenerationPerTick;
        }
        myChar.ulti1.RefreshStacks(false);
        regenerationAction = null;

    }
    private void ChargingUltimate()
    {
        defenseBeforeUltimate = myChar.myHealth.defense;
        myChar.myHealth.defense = 9999;
        myChar.ulti1Stacks = 0;
        RefreshStacks(true);
        ultimateAnimator.SetTrigger("notReady");
        readyText.SetActive(false);
        if (chargeSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, chargeSfx);
        if (castVoice != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, castVoice);
    }
    private void UsingUltimate()
    {
        if (launchSfx != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, launchSfx);
        if (castVoice != null) SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, launchVoice);
        myChar.isCharging = false;
        myChar.isUlting = true;
        myChar.ControlAnimations();
    }
    private void EndUltimate()
    {
        myChar.isUlting = false;

        myChar.myHealth.defense = defenseBeforeUltimate;
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.ResumeMovementEvent);
    }
}
