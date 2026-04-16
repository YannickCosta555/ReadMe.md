using System.Runtime.InteropServices;
using NUnit.Framework.Internal.Execution;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float WalkSpeed = 10;
    private float xAxis;


    [SerializeField] private float jumpForce = 45f;
    private float JumpBufferCounter = 0;  //stores the input
    [SerializeField] private float JumpBufferFrames; //set show long the max input buffer will be
    

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    PlayerStateList pState; //variable
    private float coyoteTimeCounter = 0; //stores the input
    [SerializeField] private float coyoteTime;  //sets how much time coyote time will last
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;




    Rigidbody2D rb;
    private Animator anim;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        anim = GetComponent<Animator>();


        pState = GetComponent<PlayerStateList>(); //allows me to refrence the player states
    }


    void Update()
    {
        GetInput();
        UpdateJumpVariables();
        Flip();
        Move();
        Jump();
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");

    }
    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void Move()
    {

        rb.linearVelocity = new Vector2(WalkSpeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {

     
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            pState.jumping = false; //because of groundcheck grounded() is false hence why pState has to be false u have to be on the ground to do it

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }


        if (!pState.jumping)
        {

            if (JumpBufferCounter > 0 && coyoteTimeCounter > 0)// Jump when grounded and jump button is pressed
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);

                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"));
            {
                pState.jumping = true;

                    airJumpCounter++;

                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
            }





        }

            
        anim.SetBool("Jumping", pState.jumping);
        
    }


    void UpdateJumpVariables()
    {
        if (Grounded())   //we want to reset the jumping bool when the player is grounded so game dont think we in permanent jumping
        {
            coyoteTimeCounter = coyoteTime;

            pState.jumping = false;
        }
       
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = JumpBufferFrames;
        }
        
        else     //sets the counter to our maximum number fo buffer frames when th eplayer presses the jump buttton and decreases it over time based on the frame duration
        {
            JumpBufferCounter = JumpBufferCounter - Time.deltaTime * 10;
        }







    }
}


    


