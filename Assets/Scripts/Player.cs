using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(5f, 5f);
    [SerializeField] LayerMask jumpableSurfaceMasks;
    [SerializeField] Transform groundChecker;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float maxCoyoteTime = 0.3f;
    [SerializeField] float fallMultiplier = 2.5f;

    // Cache
    Rigidbody2D myRigidBody;
    Animator animator;
    BoxCollider2D bodyCollider;
    SpriteRenderer bodySpriteRenderer;

    // State
    float startingGravity;
    bool isAlive;
    bool isGrounded;
    float coyoteTime;

    void Start()
    {
        isAlive = true;
        myRigidBody = GetComponent<Rigidbody2D>();
        startingGravity = myRigidBody.gravityScale;
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<BoxCollider2D>();
        bodySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive) { return; }

        Move();
        Jump();
        ClimbLadder();
        FlipSprite();
        Die();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, jumpableSurfaceMasks);
    }

    private void Move()
    {
        float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(horizontalThrow * movementSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        animator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if(isGrounded)
        {
            coyoteTime = maxCoyoteTime;
        }
        else
        {
            coyoteTime -= Time.deltaTime;
        }

        if(CrossPlatformInputManager.GetButtonDown("Jump") && (isGrounded || coyoteTime > 0))
        {
            myRigidBody.AddForce(Vector2.up * jumpSpeed);
        }

        if(myRigidBody.velocity.y < 0)
        {
            myRigidBody.AddForce(Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    private void ClimbLadder()
    {
        if(!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidBody.gravityScale = startingGravity;
            animator.SetBool("IsClimbing", false);
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.gravityScale = 0f;
        myRigidBody.velocity = climbVelocity;

        bool playerHasVerticalMovement = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        animator.SetBool("IsClimbing", playerHasVerticalMovement);
    }

    private void FlipSprite()
    {
        float moveInput = CrossPlatformInputManager.GetAxis("Horizontal");
        if(moveInput > 0)
        {
            bodySpriteRenderer.flipX = false;
        }
        else if(moveInput < 0)
        {
            bodySpriteRenderer.flipX = true;
        }
    }

    private void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
