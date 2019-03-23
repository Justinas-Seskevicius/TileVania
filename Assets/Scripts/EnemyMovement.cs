using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;

    Rigidbody2D rigidBody;
    BoxCollider2D wallCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        wallCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if(IsFacingRight())
        {
            rigidBody.velocity = new Vector2(movementSpeed, 0f);
        }
        else
        {
            rigidBody.velocity = new Vector2(-movementSpeed, 0f);
        }
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(rigidBody.velocity.x)), 1f);
    }
}
