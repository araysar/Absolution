using System.Collections;
using UnityEngine;

public class Dagger : Projectile
{
    [SerializeField] private Animator myAnim;
    private Rigidbody2D myRb;
    private Character_Movement myChar;
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
        myAnim.SetBool("isFacingRight", myChar.isFacingRight);
        myRb.velocity = new Vector2(myChar.isFacingRight? 1 * speed : -1 * speed, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public override void Impact(Health.ArmorType armorType)
    {

    }
}