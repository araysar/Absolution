using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    public bool disableInputs = false;
    public bool isUlting = false;
    public bool isCharging = false;
    private Health myHealth;
    public List<PowerUp> myUpgrades = new List<PowerUp>();

    [Header("Move")]
    public float speed = 3;
    [HideInInspector] public Rigidbody2D rb;
    public bool isFacingRight = true;
    private bool isMoving;
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
    public bool isJumping;

    [Space, Header("Wall")]
    [SerializeField] private Transform wallCheck;
    public bool isTouchingWall;
    private RaycastHit2D spikesRaycast;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallCheckDistance;
    public bool isWallSliding;

    [Space, Header("Dash")]
    [SerializeField] private PlayerAfterImagePool imagePool;
    [HideInInspector] public bool isDashing;
    private float dashTimeLeft;
    private float lastImageXPos;
    private float lastDash = -100f;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCooldown;
    public int dashCharges = 1;
    public bool canDash = false;

    [Header("Ultimate")]
    public float ulti1Stacks;
    public float ulti2Stacks;
    public float ulti1Required;
    public float ulti2Required;
    public Ultimate ulti1;

    public enum PowerUp
    {
        DoubleJump,
        Dash,
        Fire,
        Ice,
        Water,
        Ulti1,
        Ulti2,
    };

   

    //Animation
    private float gravityScale;
    private Animator myAnim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();
        ulti1 = GetComponentInChildren<Ultimate>();
        currentJumps = maxJumps;
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.onPause)
        {
            if(!disableInputs)
            {
                if(myHealth.currentHP > 0)
                {
                    Inputs();
                    CheckMovementDirection();
                    ControlAnimations();
                    CheckIfCanJump();
                    CheckIfWallSliding();
                    CheckDash();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(!GameManager.instance.onPause)
        {
            if (myHealth.currentHP > 0)
            {
                Movement();
                CheckSurroundings();
            }
        }
    }

    private void Inputs()
    {

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Dash"))
        {
            if(myUpgrades.Contains(PowerUp.Dash))
            {
                if (canDash)
                {
                    if (Time.time >= (lastDash + dashCooldown) && dashCharges > 0)
                    {
                        AttemptToDash();
                    }
                }
            }
        }
    }
    
    #region Dash

    private void AttemptToDash()
    {
        dashCharges--;
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
                StopDash();
            }
        }
        else
        {
            if(dashCharges == 0 && (isGrounded || isWallSliding))
            {
                StopDash();
                dashCharges = 1;
            }
        }
    }

    public void StopDash()
    {
        isDashing = false;
        canMove = true;
        lastDash = Time.time - dashCooldown;
        canFlip = true;
        rb.gravityScale = gravityScale;
    }
    #endregion

    #region Flip / Animations
    public void ControlAnimations()
    {
        myAnim.SetBool("isMoving", isMoving);
        myAnim.SetBool("isGrounded", isGrounded);
        myAnim.SetBool("isFalling", isFalling);
        myAnim.SetBool("isJumping", isJumping);
        myAnim.SetBool("isWallSliding", isWallSliding);
        myAnim.SetBool("isUlting", isUlting);
        myAnim.SetBool("isCharging", isCharging);
        myAnim.SetBool("isDashing", isDashing);
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
        if(!isWallSliding)
        {
            if (Input.GetAxisRaw("Horizontal") < 0 && isFacingRight) Flip();
            else if (Input.GetAxisRaw("Horizontal") > 0 && !isFacingRight) Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f) isMoving = true;
        else isMoving = false;

    }

    private void Movement()
    {
        if(!disableInputs)
        {
            if (canMove)
            {
                rb.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * speed) * Time.deltaTime, rb.velocity.y);
                if (!isGrounded && rb.velocity.y >= 0.1f)
                {
                    isJumping = false;
                    myAnim.SetBool("isJumping", isJumping);
                    isJumping = true;
                    myAnim.SetBool("isJumping", isJumping);
                    if (currentJumps == maxJumps)
                    {
                        currentJumps = maxJumps - 1;
                    }
                }
                else if(rb.velocity.y > -0.1f && rb.velocity.y < 0.1f)
                {
                    isJumping = false;
                    isFalling = false;
                }
                else if(rb.velocity.y < -0.1f)
                {
                    isFalling = true;
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
    }

    #endregion

    #region Jump
    private void Jump()
    {
        if (canJump && currentJumps > 0 && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isFalling = false;
            if (currentJumps != maxJumps)
            {
                myAnim.Play("Jump", 0, 0f);
                doubleJumpEffect.Play();
            }

            currentJumps--;

            StartCoroutine(JumpTimer());
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.1 || (isWallSliding && rb.velocity.y <= 0))
        {
            currentJumps = maxJumps;
        }

        if (currentJumps <= 0)
        {
            canJump = false;
        }
        else if(currentJumps > 0)
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
        spikesRaycast = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundMask);
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y < 0 && Input.GetAxisRaw("Horizontal") != 0 && spikesRaycast.collider.tag != "Spikes")
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
