using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    [Header("Move")]
    public float speed = 3;
    [HideInInspector] public Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isMoving;
    private float inputMovement;
    private bool canMove = true;
    private bool canFlip = true;

    [Space, Header("Jump")]
    public float jumpForce = 5;
    [SerializeField] private Transform groundCheck;
    public int maxJumps = 1;
    public int currentJumps = 1;
    private bool canJump = true;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private ParticleSystem doubleJumpEffect;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;
    public bool isGrounded;
    public bool isFalling;

    [Space, Header("Wall")]
    [SerializeField] private Transform wallCheck;
    public bool isTouchingWall;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallCheckDistance;
    public bool isWallSliding;

    [Space, Header("Dash")]
    [SerializeField] private PlayerAfterImagePool imagePool;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastDash = -100f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCooldown;
    public int dashCharges = 1;
    public bool canDash = false;


    //Animation
    private float gravityScale;
    private Animator myAnim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        currentJumps = maxJumps;
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        CheckMovementDirection();
        ControlAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckDash();
    }

    private void FixedUpdate()
    {
        Movement();
        CheckSurroundings();
    }

    private void Inputs()
    {
        inputMovement = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Dash"))
        {
            if(canDash)
            {
                if (Time.time >= (lastDash + dashCooldown))
                {
                    AttemptToDash();
                }
            }
        }
    }

    
    #region Dash

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        imagePool.GetFromPool();
        lastImageXPos = transform.position.x;
    }
    private void CheckDash()
    {
        if(isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.gravityScale = 0;
                rb.velocity = new Vector2(dashSpeed * (isFacingRight ? 1 : -1), 0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    imagePool.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
                rb.gravityScale = gravityScale;
            }
        }
    }
    #endregion

    #region Flip / Animations
    private void ControlAnimations()
    {
        myAnim.SetBool("isMoving", isMoving);
        myAnim.SetBool("isGrounded", isGrounded);
        myAnim.SetBool("isFalling", isFalling);
        myAnim.SetBool("isWallSliding", isWallSliding);
    }

    private void Flip()
    {
        if (canFlip)
        {
            if (!isWallSliding)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    private void EnableFlip()
    {
        canFlip = true;
    }

    private void DisableFlip()
    {
        canFlip = false;
    }

    #endregion

    #region Movement
    private void CheckMovementDirection()
    {
        if (inputMovement < 0 && isFacingRight) Flip();
        else if (inputMovement > 0 && !isFacingRight) Flip();

        if (Mathf.Abs(rb.velocity.x) >= 0.01f) isMoving = true;
        else isMoving = false;

    }

    private void Movement()
    {
        if(canMove)
        {
            rb.velocity = new Vector2((inputMovement * speed) * Time.deltaTime, rb.velocity.y);
            if(!isGrounded && rb.velocity.y < 0)
            {
                isFalling = true;
                if(currentJumps == maxJumps)
                {
                    currentJumps = maxJumps - 1;
                }
            }
            else
            {
                isFalling = false;
            }

            if (isWallSliding)
            {
                if (rb.velocity.y < -wallSlideSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                }
            }
        }
        if (rb.velocity.y < -jumpForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce);
        }
    }

    #endregion

    #region Jump
    private void Jump()
    {
        if (canJump && currentJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            myAnim.SetTrigger("jump");
            if (currentJumps != maxJumps)
            {
                doubleJumpEffect.Play();
            }

            currentJumps--;

            StartCoroutine(JumpTimer());
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0 || (isWallSliding && rb.velocity.y <= 0))
        {
            currentJumps = maxJumps;
        }

        if (currentJumps <= 0 || rb.velocity.y > 0)
        {
            canJump = false;
        }
        else if(currentJumps > 0 && rb.velocity.y <= 0 )
        {
            canJump = true;
            
        }

    }

    private IEnumerator JumpTimer()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundMask);
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y < 0 && inputMovement != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(isFacingRight? wallCheck.position.x + wallCheckDistance: wallCheck.position.x - wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    #endregion
}
