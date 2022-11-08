using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBossDoor : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.ResetBossBattleEvent += Enable;
        GameManager.instance.EnterBossDoorEvent += Disable;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void Enable()
    {
        gameObject.SetActive(true);
    }
}
