using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    #region Variables
    [Header("Secret Path Tilemap")]
    public Tilemap PathTileMap;
    public GameObject secretEntrance;

    [Header("Movement Variables")]
    public float movementSpeed = 10f;
    private float movementInputDirection;
    public float turnTimerSet = .1f;
    private float turnTimer;
    private int facingDirection = 1;
    private bool isFacingRight =true;
    private bool canMove;
    private bool canFlip;

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

    [Header("Dash Variables")]
    private bool isDashing;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    public float dashTime = .2f;
    public float dashSpeed = 50f;
    public float distanceBetweenImages = .1f;
    public float dashCoolDown = 2.5f;

    [Header("Respawn Point")]
    public Vector3 respawnPoint;
    public float respawnDelay;

    [Header("Other Variables")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask whatIsGround;
    private Rigidbody2D rb;

    #endregion

    
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingDirection = 1;
        amountOfJumpsLeft = amountOfJumps;
        respawnPoint = transform.position;
        if (secretEntrance == null) return;
        secretEntrance.SetActive(false);
    }

    private void Update()
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

    #region Check Functions

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        
    }


    void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        if (movementInputDirection != 0) // Mathf.Abs(rb.velocity.x) >= .01f
        { 
            canMove = true;
            //anim.SetBool("move", canMove);
        }
        else
        {
            canMove = false;
            //anim.SetBool("move", canMove);
        }

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

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImgPool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

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

    private void CheckIfCanJump()
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

    #endregion

    #region Respawn Function
    public void Respawn()
    {
        
        StartCoroutine(RespawnDelay());
        gameObject.SetActive(true);
        transform.position = respawnPoint;
    }

    IEnumerator RespawnDelay()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        
    }

    
    #endregion

    #region Other Functions



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "FallDetector")
        {
            Respawn();
        }
        if (other.tag == "Checkpoint")
        {
            respawnPoint = other.transform.position;
        }
        if (other.CompareTag("Path"))
        {
            SecretPath();
            secretEntrance.SetActive(true);
        }
    }

    public void SecretPath()
    {
        PathTileMap.ClearAllTiles();
    }

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

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImgPool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
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


    void Flip()
    {
        if (canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Debug.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x+wallCheckDistance*facingDirection,wallCheck.position.y,wallCheck.position.z), Color.blue);
    }

    #endregion

}
