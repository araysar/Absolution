using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentStatue : MonoBehaviour
{
    public GameObject actionButton;
    private bool onRange = false;
    private Character_Attack player;

    void Start()
    {
        player = FindObjectOfType<Character_Attack>();
    }


    void Update()
    {
        if(onRange && Input.GetButtonDown("Action"))
        {
            if(!player.shardsSystem.talentPanel.activeSelf)
            {
                player.shardsSystem.BTN_TalentEntry();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            onRange = true;
            actionButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onRange = false;
            actionButton.SetActive(false);
        }
    }
}
