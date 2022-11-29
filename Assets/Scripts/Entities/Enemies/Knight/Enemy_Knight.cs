using System.Collections;
using UnityEngine;

public class Enemy_Knight : MonoBehaviour
{
    
    [Header("Move")]
    public float normalSpeed;
    public float chasingSpeed;
    public float maxDistance = 2;
    private float currentDistance = 0;
    [SerializeField] private bool isResting = false;
    private float currentRestingTime = 0;
    private float maxRestingTime = 2;

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
    private bool canMove = true;
    private Vector2 lastPlayerPosition = Vector2.zero;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioClip chargeSfx;

    [Space, Header("Components")]
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
        currentDistance = 0;
        isChasing = false;
    }
    public void StopMovement()
    {
        myAnim.SetFloat("speed", 0);
        myRb.velocity = Vector2.zero;
        canMove = false;
    }

    public void ResumeMovement()
    {
        myAnim.SetFloat("speed", 1);
        canMove = true;
    }

    void Update()
    {
        if(!GameManager.instance.onPause)
        {
            if (myHealth.currentHP > 0 && canMove)
            {
                CheckSurroundings();
                AnimationController();

                OnSight();

                if (lastPlayerPosition == Vector2.zero && isChasing)
                {
                    isChasing = false;
                    currentDistance -= maxDistance / 2;
                }
                else
                {
                    if ((lastPlayerPosition - (Vector2)transform.position).magnitude < 0.1f)
                    {
                        currentDistance -= maxDistance / 2;
                        isChasing = false;
                        lastPlayerPosition = Vector2.zero;
                    }
                }

                if(isResting)
                {
                    currentRestingTime += Time.deltaTime;

                    if (currentRestingTime > maxRestingTime)
                    {
                        currentDistance = 0;
                        currentRestingTime = 0;
                        isResting = false;
                    }
                }
                else
                {
                    currentDistance += Time.deltaTime;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (myHealth.currentHP > 0 && canMove)
        {
            if (!isResting || isChasing)
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
                        myRb.velocity = new Vector2(0, myRb.velocity.y);
                        isResting = true;
                    }
                }
                else
                {
                    if (isGrounded && !isTouchingWall)
                    {
                        myRb.velocity = new Vector2((isFacingRight ? (1 * chasingSpeed) : (-1 * chasingSpeed)) * Time.fixedDeltaTime, myRb.velocity.y);
                    }

                    if(isFacingRight?(lastPlayerPosition.x - transform.position.x) < 0.2f : (lastPlayerPosition.x - transform.position.x) > 0.2f)
                    {
                        isChasing = false;
                        currentDistance -= maxDistance / 2;
                        lastPlayerPosition = Vector2.zero;
                    }
                }

            }
            else if (isResting)
            {
                myRb.velocity = new Vector2(0, myRb.velocity.y);
            }
        }
    }

    private void Flip()
    {
        if (!isFacingRight)
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

    private void AnimationController()
    {
        myAnim.SetBool("isChasing", isChasing);
        if(myRb.velocity.x != 0)
        {
            myAnim.SetBool("isMoving", true);
        }
        else
        {
            myAnim.SetBool("isMoving", false);
        }
    }


    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundMask);

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
                lastPlayerPosition = target.transform.position;
                if(!isChasing)
                {
                    isChasing = true;
                    SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, chargeSfx);
                }
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        Gizmos.DrawLine(wallCheck.position, new Vector2(isFacingRight ? wallCheck.position.x + wallCheckDistance : wallCheck.position.x - wallCheckDistance, wallCheck.position.y));

        Gizmos.color = Color.green;
        Vector2 transform2 = new Vector2(transform.position.x, transform.position.y - 0.1f);
        Gizmos.DrawLine(transform2, new Vector2(isFacingRight ? transform.position.x + playerCheckDistance : transform.position.x - playerCheckDistance, transform.position.y - 0.1f));
    }
}

