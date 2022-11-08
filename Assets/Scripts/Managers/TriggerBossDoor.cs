using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossDoor : MonoBehaviour
{
    [SerializeField] private BossFightManager myManager;

    private void Start()
    {
        myManager.ResetFightActionEvent += ResetPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            myManager.EnteringBossDoor();
            gameObject.SetActive(false);
        }
    }

    public void ResetPosition()
    {
        gameObject.SetActive(true);
    }
}
