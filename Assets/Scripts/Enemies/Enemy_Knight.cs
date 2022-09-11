using System.Collections;
using UnityEngine;

public class Enemy_Knight : MonoBehaviour
{
    
    [Header("Move")]
    public float normalSpeed;
    public float chasingSpeed;
    public float maxDistance = 2;
    private float currentDistance = 0;
    private bool recoveringFromHit = false;

    [Space, Header("Attack")]
    public float damage;
    public float timeAfterHit = 1;

    [Space, Header("Check")]
    public float wallCheckDistance = 1;
    public float playerCheckDistance = 5;
    public bool isFacingRight = true;
    public bool isGrounded = true;
    public bool isTouchingWall = false;
    public bool isChasing = false;
    private Vector2 lastPlayerPosition = Vector2.zero;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;

    [Space, Header("Components")]
    private Animator myAnim;
    private Health myHealth;
    private Rigidbody2D myRb;
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();

        int alf = Random.Range(0, 2);
        if (alf > 0)
        {
            Flip();
        }

    }

    void Update()
    {
        if (myHealth.currentHP > 0)
        {
            CheckSurroundings();
            AnimationController();
            OnSight();
            if(lastPlayerPosition == Vector2.zero)
            {
                isChasing = false;
            }
            else
            {
                if((lastPlayerPosition - (Vector2)transform.position).magnitude < 0.1f )
                {
                    isChasing = false;
                    lastPlayerPosition = Vector2.zero;
                }
            }
            currentDistance += Time.deltaTime;

            Debug.Log(lastPlayerPosition);
        }
    }

    private void FixedUpdate()
    {
        if (myHealth.currentHP > 0)
        {
            if (!recoveringFromHit)
            {
                if (!isChasing)
                {
                    if (currentDistance < maxDistance)
                    {
                        if (isGrounded && !isTouchingWall)
                        {
                            myRb.velocity = new Vector2((isFacingRight ? (1 * normalSpeed) : (-1 * normalSpeed)) * Time.fixedDeltaTime, myRb.velocity.y);
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
                    if (isGrounded && !isTouchingWall)
                    {
                        myRb.velocity = new Vector2((isFacingRight ? (1 * chasingSpeed) : (-1 * chasingSpeed)) * Time.fixedDeltaTime, myRb.velocity.y);
                    }
                    else if (!isGrounded || isTouchingWall)
                    {
                        Flip();
                    }
                    if(isFacingRight?(lastPlayerPosition.x - transform.position.x) < 0.1f : (lastPlayerPosition.x - transform.position.x) > 0.1f)
                    {
                        isChasing = false;
                        lastPlayerPosition = Vector2.zero;
                        GetComponent<Renderer>().material.color = Color.white;
                    }
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
        isChasing = false;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);

    }

    private void AnimationController()
    {
        myAnim.SetBool("isChasing", isChasing);
    }


    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
        isTouchingWall = Physics2D.Raycast(transform.position, transform.right, wallCheckDistance, groundMask);

    }

    private bool OnSight()
    {
        RaycastHit2D target = Physics2D.Raycast(transform.position, transform.right, playerCheckDistance, playerMask);
        if(target)
        {
            bool obstacle = Physics2D.Raycast(transform.position, transform.right, (target.transform.position - transform.position).magnitude, groundMask);
            if(obstacle)
            {
                return false;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                lastPlayerPosition = target.transform.position;
                isChasing = true; 
                return true;
            }
        }
        else
        {
            return false;
        }
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

        Gizmos.color = Color.green;
        Vector2 transform2 = new Vector2(transform.position.x, transform.position.y - 0.1f);
        Gizmos.DrawLine(transform2, new Vector2(isFacingRight ? transform.position.x + playerCheckDistance : transform.position.x - playerCheckDistance, transform.position.y - 0.1f));
    }
}

