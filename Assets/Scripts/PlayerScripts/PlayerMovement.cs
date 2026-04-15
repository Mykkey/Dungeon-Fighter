using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D playerCollider;   // This and its assignment in start will need changing when type of collider changes

    public float horizontalMovementSpeed;
    public float maxHorizontalSpeed;
    public float verticalJumpSpeed;
    public float arialHorizontalMovementDampener;
    bool isGrounded;
    bool isTouchingWall;


    void Start()
    {
        transform.position = Vector3.zero;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        rb.linearDamping = 3;
        rb.gravityScale = 10;
        rb.freezeRotation = true;

        horizontalMovementSpeed = 100;
        maxHorizontalSpeed = 200;
        verticalJumpSpeed = 750;
        arialHorizontalMovementDampener = 0.25f;
    }

    void FixedUpdate()
    {
        handlePlayerInputs();
    }

    private void handlePlayerInputs()
    {
        // Check is grounded:
        isGrounded = playerCollider.IsTouching(GameObject.Find("Ground").GetComponent<BoxCollider2D>()) ? true : false;

        if (Input.GetKey(KeyCode.Space)) {                                  // Jumping
            if (isGrounded) rb.AddForce(new Vector2(0, verticalJumpSpeed));
        }
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && rb.linearVelocityX > -maxHorizontalSpeed) {   // Move Left
            rb.AddForce(new Vector2(-horizontalMovementSpeed * (isGrounded ? 1 : arialHorizontalMovementDampener), 0));
        }
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && rb.linearVelocityX < maxHorizontalSpeed) {  // Move Right
            rb.AddForce(new Vector2(horizontalMovementSpeed * (isGrounded ? 1 : arialHorizontalMovementDampener), 0));
        }
    }
}
