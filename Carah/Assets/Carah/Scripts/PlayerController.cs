using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    public Collider2D collider;

    public float speed;
    public float runSpeed;
    public float jumpForce;
    public float vSpeed;
    public float moveInput;
    public bool isGrounded = false;
    public bool isRunning = false;
    public bool jumping = false;
    


    Vector2 lookDirection = new Vector2(1, 0);
    public bool facingRight = true;

    private Rigidbody2D RB;
    private PlayerInput playerInput;

    Animator animator;

    private float horizontal;
    private float vertical;
    private Vector2 collisionHit;


    private void Start()
    {
        
        RB = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        


    }

    private void Update()
    {
        collisionHit = collider.transform.position;
        // Raycast ground check
        RaycastHit2D hit = Physics2D.Raycast(collisionHit , -Vector2.up, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - collisionHit.y);
            
            Debug.Log(distance);
            Debug.DrawRay(collisionHit, hit.point, Color.green);
            if (distance > 0.05f)
            {
                isGrounded = false; 
            }
            else 
            { 
                isGrounded = true; 
            }
        }
        

        // Animator direction controller
        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f))
        {
            lookDirection.x = move.x;
            lookDirection.Normalize();
        }
        
        if (vertical == 0)
        {
            lookDirection.y = 0f;
        }
        

        animator.SetFloat("Look X", lookDirection.x);
        //animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        animator.SetFloat("vSpeed", vSpeed);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        if (isRunning == false)
        {
            RB.velocity = new Vector2(horizontal * speed, RB.velocity.y);
        }
        if (isRunning == true)
        {
            RB.velocity = new Vector2(horizontal * (speed * runSpeed), RB.velocity.y);
        }

        //controles animação de pulo

        vSpeed = RB.velocity.y;
        
        if (jumping && vSpeed == 0f)
        {
            jumping = false;
        }
        if (vSpeed < 0f && !isGrounded)
        {
            animator.SetBool("falling", true);
            animator.SetBool("jumping", false);
        }
        
        if (vSpeed < 0f)
        {
            
            animator.SetBool("jumping", false);
        }

        if (vSpeed >= 0f)
        {
            animator.SetBool("falling", false);
            
        }

        //flip char

        if (lookDirection.x > 0 && !facingRight)
            Flip();
        else if (lookDirection.x < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    // Input System controls
    public void Movement(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;

        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.action.triggered && isGrounded)
        {
            jumping = true;
            animator.SetBool("jumping", true);
        }
       
        if (context.performed && isGrounded)
        {  
            RB.velocity = new Vector2(RB.velocity.x, jumpForce); 
        }

       
        if (context.canceled && RB.velocity.y > 0f)
        { 
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.5f); 
        }

        

    }

    public void Run(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            isRunning = true;
            animator.SetBool("run", true);
        }

        
        if (context.canceled)
        {
            animator.SetBool("run", false);
            isRunning = false;

            //RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.5f);
        }
        //animator.SetTrigger("Launch");
        //PlaySound(launchClip);
    }

    public void Block(InputAction.CallbackContext context)
    {

    }

}
