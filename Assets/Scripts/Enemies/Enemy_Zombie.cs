using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Zombie : MonoBehaviour
{
    public float damage;
    public float speed;
    public float timeDash;
    public float maxDistance = 2;
    private float currentDistance = 0;
    public bool isDashing = false;
    public float timeAfterHit = 1;
    private bool recoveringFromHit = false;

    public float wallCheckDistance = 1;
    public bool isFacingRight = true;
    public bool isGrounded = true;
    public bool isTouchingWall = false;

    private BoxCollider2D myCollider;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;

    private Animator myAnim;
    private Health myHealth;
    private Rigidbody2D myRb;
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        myRb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();

        int alf = Random.Range(0, 2);
        if(alf > 0)
        {
            Flip();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(myHealth.currentHP > 0)
        {
            CheckSurroundings();
            currentDistance += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(myHealth.currentHP > 0)
        {
            if (!recoveringFromHit)
            {
                if(!isDashing)
                {
                    if (currentDistance < maxDistance)
                    {
                        if (isGrounded)
                        {
                            myRb.velocity = new Vector2((isFacingRight ? (1 * speed) : (-1 * speed)) * Time.deltaTime, myRb.velocity.y);
                        }
                        else if (!isGrounded || isTouchingWall)
                        {
                            Flip();
                        }
                    }
                    else
                    {
                        Flip();
                    }
                }
                else
                {

                }
                
            }
            else
            {
                myRb.velocity = new Vector2(0, myRb.velocity.y);
            }
        }
    }

    private void Flip()
    {
        currentDistance = 0;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);

    }


    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
        isTouchingWall = Physics2D.Raycast(transform.position, transform.right, wallCheckDistance, groundMask);
    }

    IEnumerator AfterHit()
    {
        recoveringFromHit = true;
        yield return new WaitForSeconds(timeAfterHit);
        recoveringFromHit = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        Gizmos.DrawLine(transform.position, new Vector2(isFacingRight ? transform.position.x + wallCheckDistance : transform.position.x - wallCheckDistance, transform.position.y));
    }
}
