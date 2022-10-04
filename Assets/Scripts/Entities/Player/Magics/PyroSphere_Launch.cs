using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyroSphere_Launch : MonoBehaviour
{
    [HideInInspector] public Action_Shoot myShooter;
    private Rigidbody2D myRb;
    private Character_Movement myChar;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject myExplosion;
    public float speed = 1;

    private void Awake()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myRb = GetComponent<Rigidbody2D>();
        myExplosion = Instantiate(explosion);
        myExplosion.SetActive(false);
    }
    void OnEnable()
    {
        myRb.velocity = new Vector2(myChar.isFacingRight ? 1 * speed : -1 * speed, 0);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.GetComponent<IDamageable>() != null && collision.tag != "Player") || collision.gameObject.layer == 3 || collision.gameObject.layer == 10)
        {
            myExplosion.SetActive(true);
            myExplosion.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }
}
