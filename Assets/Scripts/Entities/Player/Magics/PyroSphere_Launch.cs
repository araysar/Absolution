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
    [SerializeField] private AudioClip explosionSound;
    public float speed = 1;

    private void Awake()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myRb = GetComponent<Rigidbody2D>();
        myExplosion = Instantiate(explosion);
        myExplosion.SetActive(false);
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        GameManager.instance.DestroyEvent += Destroy;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    void OnEnable()
    {
        myRb.velocity = new Vector2(myChar.isFacingRight ? 1 * speed : -1 * speed, 0);

        if (myChar.isFacingRight)
        {
            transform.Rotate(0, 0, 0, Space.Self);
        }
        else transform.Rotate(0, 180f, 0, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.GetComponent<IDamageable>() != null && collision.tag != "Player") || collision.gameObject.layer == 3 || collision.gameObject.layer == 11)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, explosionSound);
            if(myExplosion == null)
            {
                myExplosion = Instantiate(explosion);
            }
            myExplosion.SetActive(true);
            myExplosion.transform.position = transform.position;
            gameObject.SetActive(false);
        }

        else if(collision.gameObject.layer == 10) //camera
        {
            myShooter.pyroReady = true;
            myShooter.pyroAnimator.SetTrigger("ready");
            gameObject.SetActive(false);
        }
    }
}
