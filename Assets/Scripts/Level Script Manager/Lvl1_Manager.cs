using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl1_Manager : MonoBehaviour
{
    Character_Movement player;
    public dialogue_trigger dialogue1;
    public AudioClip myClip;
    public GameObject myEffect;

    private void Awake()
    {
        
        player = FindObjectOfType<Character_Movement>();

        dialogue1.timeToTrigger = 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.ui.SetActive(false);
        myEffect.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, myClip, transform);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        GameManager.instance.isBusy = true;
        player.disableInputs = true;
    }
}
