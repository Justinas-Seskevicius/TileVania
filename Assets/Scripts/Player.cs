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

    // Cache
    Rigidbody2D rigidbody2D;
    Animator animator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;
    float startingGravity;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        startingGravity = rigidbody2D.gravityScale;
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        ClimbLadder();
        FlipSprite();
    }

    private void Move()
    {
        float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(horizontalThrow * movementSpeed, rigidbody2D.velocity.y);
        rigidbody2D.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x) > Mathf.Epsilon;
        animator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if(CrossPlatformInputManager.GetButtonDown("Jump") && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            rigidbody2D.velocity += jumpVelocity;
        }
    }

    private void ClimbLadder()
    {
        if(!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rigidbody2D.gravityScale = startingGravity;
            animator.SetBool("IsClimbing", false);
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(rigidbody2D.velocity.x, controlThrow * climbSpeed);
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.velocity = climbVelocity;

        bool playerHasVerticalMovement = Mathf.Abs(rigidbody2D.velocity.y) > Mathf.Epsilon;
        animator.SetBool("IsClimbing", playerHasVerticalMovement);
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbody2D.velocity.x), transform.localScale.y);
        }
    }
}
