using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    public static Character_Movement instance;


    public bool disableInputs = false;
    public bool isUlting = false;
    public bool isCharging = false;
    [HideInInspector] public Health myHealth;
    [HideInInspector] public Energy myEnergy;
    [HideInInspector] public Action_Shoot myShooter;

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
    [SerializeField] private float energyDoubleJump = 5;
    private bool canJump = true;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private ParticleSystem doubleJumpEffect;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioClip voiceJumpSfx;
    [SerializeField] private AudioClip doubleJumpSfx;
    [SerializeField] private AudioClip landingSfx;
    public bool isGrounded = true;
    public bool isFalling = false;
    public bool isJumping = false;

    [Space, Header("Wall")]
    [SerializeField] private Transform wallCheck;
    public bool isTouchingWall;
    private RaycastHit2D noSlideRaycast;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallCheckDistance;
    public bool isWallSliding;
    private Coroutine jumpTimer;

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
    [SerializeField] private float energyDash = 15;
    [SerializeField] private AudioClip dashSfx;
    public bool canDash = false;

    [Header("Dagger")]
    public GameObject impactFleshEffect;
    public AudioClip impactFleshSfx;
    public GameObject impactMetalEffect;
    public AudioClip impactMetalSfx;
    public GameObject impactShieldEffect;
    public AudioClip impactShieldSfx;
    public GameObject blockedWallImpactEffect;
    public AudioClip blockedWallImpactSfx;


    [Header("Ultimate")]
    public float ulti1Stacks;
    public float ulti1Required;
    public Ultimate ulti1;
    public float ulti2Stacks;
    public float ulti2Required;

    [Header("Power Ups Activate")]
    public List<PowerUp> myUpgrades = new List<PowerUp>();
    [SerializeField] private GameObject uiDash;
    [SerializeField] private GameObject uiUltimate1;
    [SerializeField] private GameObject uiFire;

    [Header("Save")]
    private float saveUlti1Stacks;
    private float saveCurrentHp;
    private float saveCurrentEnergy;
    public List<PowerUp> saveMyUpgrades = new List<PowerUp>();
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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        myShooter = GetComponent<Action_Shoot>();
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myHealth = GetComponent<Health>();
        myEnergy = GetComponent<Energy>();
        ulti1 = GetComponent<Ultimate>();
        currentJumps = maxJumps;
        gravityScale = rb.gravityScale;
        PowerUpGrab();
    }

    private void Start()
    {
        GameManager.instance.PlayerDisableEvent += DisablePlayer;
        GameManager.instance.PlayerRespawnEvent += RespawnPlayer;
        GameManager.instance.PlayerDisableEvent += StopMovement;
        GameManager.instance.StopPlayerMovementEvent += StopMovement;
        GameManager.instance.ResumePlayerMovementEvent += ResumeMovement;
        GameManager.instance.ResumePlayerMovementEvent += EnableFlip;
        GameManager.instance.SaveDataEvent += SaveData;
        GameManager.instance.LoadDataEvent += LoadData;
        GameManager.instance.DestroyEvent += Destroy;
        GameManager.instance.TriggerAction(GameManager.ExecuteAction.SaveData);
    }


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
                    CheckIfCanJump();
                    CheckIfWallSliding();
                    CheckDash();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.onPause)
        {
            if (myHealth.currentHP > 0)
            {
                CheckSurroundings();
                ControlAnimations();
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
                    if (Time.time >= (lastDash + dashCooldown) && dashCharges > 0 && myEnergy.CanUse(energyDash))
                    {
                        AttemptToDash();
                    }
                }
            }
        }

        if (Input.GetButtonDown("Ultimate1"))
        {
            if (myUpgrades.Contains(PowerUp.Ulti1) && ulti1Stacks == ulti1Required)
                ulti1.ActivateUltimate();
        }
    }
    
    #region Dash

    private void AttemptToDash()
    {
        myEnergy.currentEnergy -= energyDash;
        myEnergy.ReloadEnergy();
        SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, dashSfx);
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
        myAnim.SetFloat("velocity.y", rb.velocity.y);
        myAnim.SetBool("isJumping", isJumping);
        myAnim.SetBool("isWallSliding", isWallSliding);
        myAnim.SetBool("isUlting", isUlting);
        myAnim.SetBool("isCharging", isCharging);
        myAnim.SetBool("isDashing", isDashing);
        myAnim.SetBool("isFalling", isFalling);
        myAnim.SetFloat("life", myHealth.currentHP / myHealth.maxHP);
    }

    private void Flip()
    {
        if(canFlip)
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

    public void StopMovement()
    {
        StopDash();
        isJumping = false;
        isFalling = false;
        disableInputs = true;
        isMoving = false;
        rb.velocity = new Vector2(0, 0);
        ControlAnimations();
        myAnim.Play("Idle", 0);
    }
    public void ResumeMovement()
    {
        disableInputs = false;
    }

    #endregion

    #region Jump
    private void Jump()
    {
        if (canJump && currentJumps > 0 && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if(myAnim.GetBool("isJumping") || myAnim.GetBool("isFalling")) myAnim.Play("Idle", 0, 0f);
            isFalling = false;
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, voiceJumpSfx);
            if (currentJumps != maxJumps && myUpgrades.Contains(PowerUp.DoubleJump) && myEnergy.CanUse(energyDoubleJump))
            {
                myEnergy.currentEnergy -= energyDoubleJump;
                myEnergy.ReloadEnergy();
                if (myAnim.GetBool("isJumping") || myAnim.GetBool("isFalling")) myAnim.Play("Idle", 0, 0f);
                SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, doubleJumpSfx);
                doubleJumpEffect.Play();
            }

            currentJumps--;

            StartCoroutine(JumpTimer());
        }
    }

    private IEnumerator WallJumpTimer()
    {
        yield return new WaitForSeconds(0.1f);
        currentJumps--;
        jumpTimer = null;
    }
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.1f || (isWallSliding && rb.velocity.y <= 0))
        {
            currentJumps = maxJumps;
            jumpTimer = null;
        }
        else if(!isGrounded && rb.velocity.y <= 0.1f && !isWallSliding && currentJumps == maxJumps)
        {
            if(jumpTimer == null)
            {
                jumpTimer = StartCoroutine(WallJumpTimer());
            }
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

    #endregion

    #region Checks
    private void CheckSurroundings()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if(isGrounded == true && wasGrounded == false)
        {
            SoundManager.instance.PlaySound(SoundManager.SoundChannel.SFX, landingSfx);
        }
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundMask);
        noSlideRaycast = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundMask);
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0 && Input.GetAxisRaw("Horizontal") != 0 && noSlideRaycast.collider.tag != "NoSlide")
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
        Gizmos.DrawLine(wallCheck.position, new Vector3(isFacingRight ? wallCheck.position.x + wallCheckDistance : wallCheck.position.x - wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    #endregion

    #region  Power UP
    public void PowerUpGrab()
    {
        foreach (PowerUp item in myUpgrades)
        {
            switch (item)
            {
                case PowerUp.DoubleJump:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(true);
                    }
                    if(maxJumps < 2)
                    {
                        maxJumps = 2;
                    }
                    break;
                case PowerUp.Dash:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(true);
                    }
                    uiDash.SetActive(true);
                    break;
                case PowerUp.Fire:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(true);
                    }
                    uiFire.SetActive(true);
                    break;
                case PowerUp.Ice:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(true);
                    }
                    break;
                case PowerUp.Water:
                    break;
                case PowerUp.Ulti1:
                    uiUltimate1.SetActive(true);
                    break;
                case PowerUp.Ulti2:
                    break;
            }
        }
    }

    public void PowerUpErase()
    {
        foreach (PowerUp item in myUpgrades)
        {
            switch (item)
            {
                case PowerUp.DoubleJump:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(false);
                    }
                    maxJumps = 1;
                    break;
                case PowerUp.Dash:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(false);
                    }
                    uiDash.SetActive(false);
                    break;
                case PowerUp.Fire:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(false);
                    }
                    uiFire.SetActive(false);
                    break;
                case PowerUp.Ice:
                    for (int i = 0; i < myEnergy.uiGameObject.Length; i++)
                    {
                        myEnergy.uiGameObject[i].SetActive(false);
                    }
                    break;
                case PowerUp.Water:
                    break;
                case PowerUp.Ulti1:
                    uiUltimate1.SetActive(false);
                    break;
                case PowerUp.Ulti2:
                    break;
            }
        }
    }
    #endregion

    #region Respawn
    private void DisablePlayer()
    {
        myHealth.currentHP = 0;
        myHealth.myRenderer.enabled = false;
        disableInputs = true;
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void RespawnPlayer()
    {
        LoadData();
        myHealth.respawnEffect.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        myHealth.myRenderer.enabled = true;
        rb.velocity = Vector2.zero;
        isMoving = false;
        isGrounded = true;
        isJumping = false;
        isWallSliding = false;
        isUlting = false;
        isCharging = false;
        isDashing = false;
        isFalling = false;
        ControlAnimations();
        myAnim.Play("Idle", 0);
    }
    public void SaveData()
    {
        saveCurrentEnergy = myEnergy.currentEnergy;
        saveCurrentHp = myHealth.maxHP;
        saveUlti1Stacks = ulti1Stacks;
        saveMyUpgrades = new List<PowerUp>(myUpgrades);
        myHealth.initialPosition = transform.position;
    }

    public void LoadData()
    {
        myEnergy.currentEnergy = saveCurrentEnergy;
        myHealth.currentHP = saveCurrentHp;
        ulti1Stacks = saveUlti1Stacks;
        PowerUpErase();
        myUpgrades = new List<PowerUp>(saveMyUpgrades);
        PowerUpGrab();
        transform.position = myHealth.initialPosition;
        myHealth.RefreshLifeBar();
        myEnergy.ReloadEnergy();
        ulti1.RefreshStacks();
    }
    #endregion

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CheckPoint")
        {
            GameManager.instance.TriggerAction(GameManager.ExecuteAction.SaveData);
            collision.gameObject.SetActive(false);
        }
    }
}
