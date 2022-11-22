using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    [SerializeField] private GameObject myCollision;
    Animator myAnim;
    [SerializeField] private bool isReady = true;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    private void ExitAnimation()
    {
        myAnim.SetTrigger("exit");
    }

    private void RestartAnimation()
    {
        isReady = true;
        myAnim.SetTrigger("back");
    }

    public void Activate()
    {
        if (isReady)
        {
            isReady = false;
            myAnim.SetTrigger("shrink");
        }
    }
}
