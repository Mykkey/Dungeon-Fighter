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

    private void Awake()
    {
        isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
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
    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, playerMovementStats.groundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0, Vector2.down, playerMovementStats.groundDetectionRayLength, playerMovementStats.groundLayer);
        if (groundHit.collider != null) isGrounded = true;
        else isGrounded = false;
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion
}
