using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Movement Variables")]
    public float movementSpeed = 10f;
    private float movementInputDirection;
    public float turnTimerSet = .1f;
    private float turnTimer;
    private int facingDirection = 1;
    private bool isFacingRight = true;
    private bool canMove;
    private bool canFlip;

    [Header("Dash Variables")]
    public float dashTime = .2f;
    public float dashSpeed = 30f;
    public float dashCoolDown = 1.5f;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDash = -100f;

    [Header("Jump Variables")]
    public float jumpForce = 16f;
    public float airDragMultiplier = .95f;
    public float jumpTimerSet = .15f;
    public float variableJumpHeightMultiplier = .5f;
    public int amountOfJumps = 1;
    private int amountOfJumpsLeft;
    private float jumpTimer;
    private bool canJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;

    

    [Header("Check Variables")]
    public float groundCheckRadius;
    public float wallCheckDistance;
    private bool isGrounded;
    private bool isTouchingWall;

    [Header("Respawn Point")]
    public Vector3 respawnPoint;

    [Header("Other Variables")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask whatIsGround;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (instance == null) return;

        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingDirection = 1;
        amountOfJumpsLeft = amountOfJumps;
        respawnPoint = transform.position;
    }


    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckJump();
        CheckDash();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    #region Respawn Function
    public void Respawn()
    {

        gameObject.SetActive(false);
        transform.position = respawnPoint;
        gameObject.SetActive(true);

    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            this.transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            this.transform.parent = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RotateStick"))
        {
            Respawn();
        }
        if (other.CompareTag("Checkpoint"))
        {
            var ch = other.GetComponent<CheckpointController>();
            ch.checkpointReached = true;
            respawnPoint = other.transform.position;
        }
        
    }

    public void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
                Jump();
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;
                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time >= (lastDash + dashCoolDown))
                AttemptToDash();
        }

    }

    #region Dash
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

        }
    }

    #endregion

    #region Jump
    void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (isGrounded)
                Jump();
        }

        if (isAttemptingToJump)
            jumpTimer -= Time.deltaTime;
    }

    public void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0)
            canJump = false;
        else
            canJump = true;
    }

    void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    #endregion

    #region Movement
    public void ApplyMovement()
    {
        if (!isGrounded && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);

        }

        else if (canMove)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }

    }

    void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

    }

    void Flip()
    {
        if (canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);

        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

    }

    #endregion

    

}
