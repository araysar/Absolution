using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] private int sceneNumber = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.ChangeScene(sceneNumber);
    }
}
