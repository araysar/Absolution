using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Character_Movement.PowerUp myPower;
    private Coroutine coroutineGetSkill;
    [SerializeField] private GameObject uiMessage;
    [SerializeField] private float timeUntilFade;
    private Animator myAnim;

    private void Start()
    {
        myAnim = uiMessage.GetComponent<Animator>();
        myAnim.SetBool("exit", false);
        if (FindObjectOfType<Character_Movement>().myUpgrades.Contains(myPower))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Movement>() != null)
        {
            if (coroutineGetSkill != null) return;

            coroutineGetSkill = StartCoroutine(GetSkillCoroutine());
        }
    }

    private IEnumerator GetSkillCoroutine()
    {
        GameManager.instance.Pause();
        uiMessage.SetActive(true);
        yield return new WaitForSecondsRealtime(timeUntilFade);
        myAnim.SetBool("exit", true);
    }

    public void ExitAnimation()
    {
        GameManager.instance.UnPause();
        uiMessage.SetActive(false);
        gameObject.SetActive(false);
    }
}
