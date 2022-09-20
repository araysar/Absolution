using System.Collections;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [HideInInspector] public Action_Shoot myPool;
    [SerializeField] private Animator myAnim;
    private Rigidbody2D myRb;
    private Character_Movement myChar;
    public GameObject impactEffect;
    public bool isRolling = true;
    public float damage = 5;
    public float speed = 3;

    private void Awake()
    {
        myChar = FindObjectOfType<Character_Movement>();
        myAnim = GetComponent<Animator>();
        myRb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        myAnim.SetBool("isRolling", isRolling);
        myAnim.SetBool("isFacingRight", myChar.isFacingRight);
        myRb.velocity = new Vector2(myChar.isFacingRight? 1 * speed : -1 * speed, 0);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 10)
        {
            myPool.AddToPool(gameObject);
        }
        else if (collision.GetComponent<IDamageable>() != null && collision.tag != "Player")
        {
            Impact();
            collision.GetComponent<IDamageable>().TakeDamage(damage);
            myPool.AddToPool(gameObject);
        }
    }


    private void Impact()
    {
        if(impactEffect != null) Instantiate(impactEffect, transform.position, Quaternion.identity);
    }
}
