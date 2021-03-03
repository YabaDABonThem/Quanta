using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DavidsController : MonoBehaviour
{
    //manipulatable fields
    [Header("Movement")]
    [Range(0f, 3f)] [SerializeField] float movementSmoothing = 0.5f;
    [SerializeField] float jumpForce = 4.0f;
    [SerializeField] float characterSpeed = 5f;
    [SerializeField] PhysicsMaterial2D noFriction;
    [SerializeField] PhysicsMaterial2D fullFriction;
    [SerializeField] float maximumSlopeAngle = 60;

    [Header("Movement Control")]
    [SerializeField] private float VSlopeCheckDistance = 0.25f;
    [SerializeField] private float HSlopeCheckDistance = 0.8f;

    //propeties
    public bool CanMove { get; set; }

    //References
    private Rigidbody2D controllerRB;
    private CapsuleCollider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;
    private LayerMask wallMask;

    //funnctional field
    private Vector3 Velocity = Vector3.zero;
    private LayerMask currentGroundType;
    private bool isOnGround = false;
    private bool isJumping;
    private float jumpGravityScale = 2f;
    private float fallGravityScale = 2.5f;
    private float SteepSlopeGravityScale = 3f;
    private bool isOnWall;

    // Start is called before the first frame update
    void Start()
    {
        controllerRB = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<CapsuleCollider2D>();
        wallMask = LayerMask.GetMask("Wall");
        softGroundMask = LayerMask.GetMask("Soft Ground");
        hardGroundMask = LayerMask.GetMask("Hard Ground");

        //colliderSize = controllerCollider.size;

        CanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateGrounding();
        //UpdateSlope();
        UpdateGravity();
    }

    private void UpdateGrounding()
    {
        //Check if character is falling using the y velocity
        if(controllerRB.velocity.y < 0.0f)
        {
            isJumping = false; 
        }

        //Check for ground type or whether or not the character is on ground. 
        if (controllerCollider.IsTouchingLayers(softGroundMask))
        {
            isOnGround = true;
            currentGroundType = softGroundMask;
        }
        else if (controllerCollider.IsTouchingLayers(hardGroundMask))
        {
            isOnGround = true;
            currentGroundType = hardGroundMask;
        }
        else
        {
            isOnGround = false;
        }

        //Check whether or not the character is in contact with a wall
        if(controllerCollider.IsTouchingLayers(wallMask))
        {
            isOnWall = true;
        }
        else
        {
            isOnWall = false;
        }
    }

    public void UpdateMovement(float moveIn, bool crouch, bool jumpIn)
    {
        //playerInputX = moveIn;

        //most basic controls
        if(jumpIn && isOnGround && !isOnWall)
        {
            isJumping = true;
            controllerRB.velocity = new Vector2(controllerRB.velocity.x, 1f * jumpForce);
        }

        Debug.Log(isOnWall);

        if (isOnWall)
        {
            Vector3 targetVelocity = new Vector2(moveIn * characterSpeed, controllerRB.velocity.y);
            controllerRB.velocity = Vector3.SmoothDamp(controllerRB.velocity, targetVelocity, ref Velocity, movementSmoothing);
        }
        else
        {
            Vector2 targetVelocity = new Vector2(moveIn * characterSpeed, controllerRB.velocity.y);
            controllerRB.velocity = targetVelocity;
        }

        //toggles friction for slopes
        if (moveIn == 0.0f && !isOnWall)
        {
            controllerRB.sharedMaterial = fullFriction;
        }
        else
        {
            controllerRB.sharedMaterial = noFriction;
        }

        //implement running sound and animation.
    }

    private void UpdateGravity()
    { 
        //changes gravity based on the motion of the character

        //the character is falling
        if(!isJumping && !isOnGround)
        {
            controllerRB.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityScale - 1) * Time.deltaTime;
        }
        //the character is jumping up
        else if (isJumping && !Input.GetButton("Jump"))
        {
            controllerRB.velocity += Vector2.up * Physics2D.gravity.y * (jumpGravityScale - 1) * Time.deltaTime;
        }
        //the character is on a un-climable slope
        else if (isOnWall)
        {
            controllerRB.velocity += Vector2.up * Physics2D.gravity.y * (SteepSlopeGravityScale - 1) * Time.deltaTime;
        }
    }

}
