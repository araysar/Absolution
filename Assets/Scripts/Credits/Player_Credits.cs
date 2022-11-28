using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Credits : MonoBehaviour
{
    private Action action = delegate { };
    private Animator myAnim;
    [SerializeField] private float speed;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        PlayMovement();
    }

    // Update is called once per frame
    void Update()
    {
        action();
    }

    public void StopMovement()
    {
        action = delegate { };
        myAnim.SetBool("isMoving", false);
    }

    public void PlayMovement()
    {
        action = Move;
        myAnim.SetBool("isMoving", true);
    }

    private void Move()
    {
        transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
    }
}
