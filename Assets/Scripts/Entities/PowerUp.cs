using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Character_Movement.PowerUp myPower;
    private Character_Movement myChar;
    private bool isGrabed = false;
    [SerializeField] private GameObject uiMessage;
    private Animator myAnim;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("exit", false);
        myChar = FindObjectOfType<Character_Movement>();
        myChar.myUpgrades.Add(myPower);

        if (myChar.myUpgrades.Contains(myPower))
        {
            isGrabed = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Character_Movement>() != null && !isGrabed)
        {
            if(!isGrabed) isGrabed = true;

            GameManager.instance.Pause();
            uiMessage.SetActive(true);
            myAnim.SetBool("enter", true);
        }
    }

    public void BTN_Exit()
    {
        myAnim.SetBool("enter", false);
        myAnim.SetBool("exit", true);
    }

    public void ExitAnimation()
    {
        GameManager.instance.UnPause();
        myChar.myUpgrades.Add(myPower);
        uiMessage.SetActive(false);
        gameObject.SetActive(false);
    }
}
