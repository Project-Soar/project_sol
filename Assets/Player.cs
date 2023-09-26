using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private Animator anim;

    [Header("Move Info")]
    public float moveSpeed;
    private bool canDoubleJump;
    private float movingInput;
    public float jumpForce;
    private bool canMove;
    public Vector2 wallJumpDirection;

    [Header("Collision Info")]
    public LayerMask whatIsGround;
    public float groundCheckDistance;
    public float wallCheckDistance;
    private bool isGrounded;
    private bool isWallDetected;
    private bool canWallSlide;
    private bool isWallSliding;


    private bool facingRight = true;
    private int facingDirection = 1;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationController();

        CollisionCheck();

        InputChecks();
        
        FlipController();

        if(isGrounded)
        {
            canDoubleJump = true;
            canMove = true;
        }
        if(canWallSlide)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        
        Move();
    }

    private void Move()
    {
        if(canMove)
            rb.velocity = new Vector2(moveSpeed * movingInput , rb.velocity.y);
    }

    private void AnimationController()
    {
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isWallDetected",isWallDetected);
    }

    private void InputChecks()
    {
        movingInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
                
        }
        if(Input.GetAxis("Vertical") < 0)
            canWallSlide = false;
    }
    private void JumpButton()
    {
        if(isWallSliding)
        {
            WallJump();
        }
        else if(isGrounded)
        {
            Jump();
        }else if(canDoubleJump)
        {
            canDoubleJump =false;
            Jump();
        }

        canWallSlide = false;
    }

    private void WallJump()
    {
        canMove = false;
        rb.velocity = new Vector2(wallJumpDirection.x* - facingDirection, wallJumpDirection.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void FlipController()
    {
        if(facingRight && rb.velocity.x<0)
            Flip();
        else if(!facingRight && rb.velocity.x>0)
            Flip();
    }

    private void Flip()
    {
        facingDirection = facingDirection*-1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    private void CollisionCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance , whatIsGround);
        
        if(isWallDetected && rb.velocity.y <0)
            canWallSlide = true;
        if(!isWallDetected)
        {
            isWallSliding = false;
            canWallSlide = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckDistance * facingDirection, transform.position.y));
    }
}
