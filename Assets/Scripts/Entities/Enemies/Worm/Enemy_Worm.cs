using System.Collections;
using UnityEngine;

public class Enemy_Worm : MonoBehaviour
{
    public float damage;
    public float speed;
    public float timeAfterHit = 1;
    private bool recoveringFromHit = false;

    public float wallCheckDistance = 1;
    public bool isFacingRight = true;
    public bool isGrounded = true;
    public bool isTouchingWall = false;
    private bool canMove = true;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    private Animator myAnim;
    private Health myHealth;
    private Rigidbody2D myRb;

    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();
        GameManager.instance.ResumeMovementEvent += ResumeMovement;
        GameManager.instance.StopMovementEvent += StopMovement;
    }

    void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if (myHealth.currentHP > 0 && canMove)
            {
                CheckSurroundings();
            }
        }
    }

    private void FixedUpdate()
    {
        if(myHealth.currentHP > 0 && canMove)
        {
            if (!recoveringFromHit)
            {
                if (isGrounded && !isTouchingWall)
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
                myRb.velocity = new Vector2(0, myRb.velocity.y);
            }
        }
    }

    private void Flip()
    {
        if(!isFacingRight)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            isFacingRight = true;
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            isFacingRight = false;
        }
    }

    public void StopMovement()
    {
        myRb.velocity = Vector2.zero;
        myAnim.SetFloat("speed", 0);
        canMove = false;
    }

    public void ResumeMovement()
    {
        myAnim.SetFloat("speed", 1);
        canMove = true;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
        isTouchingWall = Physics2D.Raycast(transform.position, !isFacingRight? transform.right : transform.right, wallCheckDistance, groundMask);
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
