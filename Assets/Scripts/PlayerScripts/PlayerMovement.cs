using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats playerMovementStats;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;

    private Rigidbody2D rb;

    // Movewment variables
    private Vector2 moveVelocity;
    private bool isFacingRight;

    // Collision Check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;

    // Jump variables
    public float verticalVelocity {  get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    // Apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    // Jump buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    // Coyote time variables
    private float coyoteTimer;

    private void Awake()
    {
        isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        if (isGrounded) Move(playerMovementStats.groundAcceleration, playerMovementStats.groundDeceleration, InputManager.movement);
        else Move(playerMovementStats.airAcceleration, playerMovementStats.airDeceleration, InputManager.movement);
    }

    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero) {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.runIsHeld) targetVelocity = new Vector2(moveInput.x, 0) * playerMovementStats.maxRunSpeed;
            else targetVelocity = new Vector2(moveInput.x, 0) * playerMovementStats.maxWalkSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }else if (moveInput == Vector2.zero) {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0) Turn(false);
        else if (!isFacingRight && moveInput.x > 0) Turn(true);
    }

    private void Turn(bool turnRight)
    {
        if (turnRight) {
            isFacingRight = true;
            transform.Rotate(0, 180, 0);
        } else {
            isFacingRight = false;
            transform.Rotate(0, -180, 0);
        }
    }

    #endregion

    #region Jump

    private void JumpChecks()
    {
        if (InputManager.jumpWasPressed) {
            Debug.Log("jumping");
            jumpBufferTimer = playerMovementStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        if (InputManager.jumpWasReleased) {
            if (jumpBufferTimer > 0) jumpReleasedDuringBuffer = true;
            if (isJumping && verticalVelocity > 0) {
                if (isPastApexThreshold) {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = playerMovementStats.timeForUpwardsCancel;
                    verticalVelocity = 0;
                } else {
                    isFastFalling = true;
                    fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }

        if (jumpBufferTimer > 0 && isJumping && (isGrounded || coyoteTimer > 0)) {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer) {
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }
        }

        else if (jumpBufferTimer > 0 && isJumping && numberOfJumpsUsed < playerMovementStats.numberOfJumpsAllowed) {
            isFastFalling = false;
            InitiateJump(1);
        }else if (jumpBufferTimer > 0 && isFalling && numberOfJumpsUsed < playerMovementStats.numberOfJumpsAllowed - 1) {
            InitiateJump(2);
            isFastFalling = false;
        }

        if ((isJumping || isFalling) && isGrounded && verticalVelocity <= 0) {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;
            verticalVelocity = Physics.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsedLocal)
    {
        if (!isJumping) {
            isJumping = true;
        }
        jumpBufferTimer = 0;
        numberOfJumpsUsed += numberOfJumpsUsedLocal;
        verticalVelocity = playerMovementStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        if (isJumping) {
            if (bumpedHead) isFastFalling = true;
            if (verticalVelocity >= 0) {
                apexPoint = Mathf.InverseLerp(playerMovementStats.InitialJumpVelocity, 0, verticalVelocity);
                if (apexPoint > playerMovementStats.apexThreshold) {
                    if (!isPastApexThreshold) {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0;
                    }
                    if (isPastApexThreshold) {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < playerMovementStats.apexHangTime) {
                            verticalVelocity = 0;
                        } else {
                            verticalVelocity = -0.01f;
                        }
                    }
                } else {
                    verticalVelocity += playerMovementStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold) isPastApexThreshold = false;
                }
            } else if (!isFastFalling) verticalVelocity += playerMovementStats.Gravity * playerMovementStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            else if (verticalVelocity < 0) {
                if (!isFalling) isFalling = true;
            }
        }

        if (isFastFalling) {
            if (fastFallTime >= playerMovementStats.timeForUpwardsCancel) verticalVelocity += playerMovementStats.Gravity * playerMovementStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            else if (fastFallTime < playerMovementStats.timeForUpwardsCancel) verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0, (fastFallTime / playerMovementStats.timeForUpwardsCancel));
            fastFallTime += Time.fixedDeltaTime;
        }

        if (!isGrounded && !isJumping) {
            if (!isFalling) isFalling = true;
            verticalVelocity += playerMovementStats.Gravity * Time.fixedDeltaTime;
        }

        verticalVelocity = Mathf.Clamp(verticalVelocity, -playerMovementStats.maxFallSpeed, 50);

        rb.velocity = new Vector2(rb.velocity.x , verticalVelocity);
    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, playerMovementStats.groundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0, Vector2.down, playerMovementStats.groundDetectionRayLength, playerMovementStats.groundLayer);
        if (groundHit.collider != null) isGrounded = true;
        else isGrounded = false;
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * playerMovementStats.headWidth, playerMovementStats.headDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0, Vector2.up, playerMovementStats.headDetectionRayLength, playerMovementStats.groundLayer);
        if (headHit.collider != null) bumpedHead = true;
        else bumpedHead = false;


    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (!isGrounded) coyoteTimer -= Time.deltaTime;
        else coyoteTimer = playerMovementStats.jumpCoyoteTime;
    }

    #endregion
}
