using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossDoor : MonoBehaviour
{
    [SerializeField] private BossFightManager myManager;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {

        }
    }

    public void ResetPosition()
    {
        gameObject.SetActive(true);
    }
}
