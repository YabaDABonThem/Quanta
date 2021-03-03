using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
    [SerializeField] private float HSlopeCheckDistance = 0.5f;

    //propeties
    public bool CanMove { get; set; }

    //References
    private Rigidbody2D controllerRigidbody;
    private CapsuleCollider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;

    //funnctional field
    //private Vector2 colliderSize;
    private Vector3 Velocity = Vector3.zero;
    //private Vector2 slopeNormalPerp;
    private LayerMask currentGroundType;
    private bool isOnGround = false;
    private bool isJumping;
    private float slopeAngle;
    private float slopeSideAngle;
    private float oldSlopeAngle;
    private bool isOnSlope;
    private bool canWalkOnSlope;
    private float jumpGravityScale = 2f;
    private float fallGravityScale = 2.5f;
    private float SteepSlopeGravityScale = 3f;
    private float playerInputX;
    private bool USVisValid;

    // Start is called before the first frame update
    void Start()
    {
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<CapsuleCollider2D>();
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
        UpdateSlope();
        UpdateGravity();
    }

    private void UpdateGrounding()
    {
        //Check if character is falling using the y velocity
        if (controllerRigidbody.velocity.y < 0.0f)
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
    }

    public void UpdateMovement(float moveIn, bool crouch, bool jumpIn)
    {
        //Stores user x input
        //playerInputX = moveIn;

        //most basic controls
        if (jumpIn && isOnGround && canWalkOnSlope)
        {
            isJumping = true;
            controllerRigidbody.velocity = new Vector2(controllerRigidbody.velocity.x, 1f * jumpForce);
        }

        if (isOnGround && canWalkOnSlope)
        {
            Vector2 targetVelocity = new Vector2(moveIn * characterSpeed, controllerRigidbody.velocity.y);
            controllerRigidbody.velocity = targetVelocity;
        }
        else if (isOnGround && !canWalkOnSlope)
        {
            Vector3 targetVelocity = new Vector2(moveIn * characterSpeed, controllerRigidbody.velocity.y);
            controllerRigidbody.velocity = Vector3.SmoothDamp(controllerRigidbody.velocity, targetVelocity, ref Velocity, movementSmoothing);
        }

        //Handles Jumps
        /*if (jumpIn && isOnGround && canWalkOnSlope)
        {
            isJumping = true;
            controllerRigidbody.velocity = new Vector2(controllerRigidbody.velocity.x, 1.0f * jumpForce);
        }

        //Handles movement w/ slope checks
        if (isOnGround && !isOnSlope && !isJumping)
        {
            Vector3 targetVelocity = new Vector2(moveIn * characterSpeed, 0.0f);
            controllerRigidbody.velocity = targetVelocity;
        }
        else if(isOnGround && isOnSlope && !isJumping && !canWalkOnSlope)
        {
            Vector3 targetVelocity = new Vector2(moveIn * characterSpeed, 0.0f);
            controllerRigidbody.velocity = Vector3.SmoothDamp(controllerRigidbody.velocity, targetVelocity, ref Velocity, movementSmoothing);
        }
        else if(isOnGround && isOnSlope && !isJumping && canWalkOnSlope)
        {
            Vector3 targetVelocity = new Vector2(characterSpeed * slopeNormalPerp.x * -moveIn, characterSpeed * slopeNormalPerp.y * -moveIn);
            controllerRigidbody.velocity = targetVelocity;
        }
        else if(!isOnGround)
        {
            Vector3 targetVelocity = new Vector2(moveIn * characterSpeed, controllerRigidbody.velocity.y);
            controllerRigidbody.velocity = targetVelocity;
        }
        //Debug.Log(characterSpeed * slopeNormalPerp.y * -moveIn);

        //toggles friction for slopes
        if (isOnSlope && moveIn == 0.0f && canWalkOnSlope)
        {
            controllerRigidbody.sharedMaterial = fullFriction;
        }
        else
        {
            controllerRigidbody.sharedMaterial = noFriction;
        }*/

        //implement running sound and animation.
    }

    private void UpdateGravity()
    {
        //changes gravity based on the motion of the character

        //the character is falling
        if (!isJumping && !isOnGround)
        {
            controllerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityScale - 1) * Time.deltaTime;
        }
        //the character is jumping up
        else if (isJumping && !Input.GetButton("Jump"))
        {
            controllerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (jumpGravityScale - 1) * Time.deltaTime;
        }
        //the character is on a un-climable slope
        else if (!canWalkOnSlope && isOnSlope && isOnGround)
        {
            controllerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (SteepSlopeGravityScale - 1) * Time.deltaTime;
        }
    }

    private void UpdateSlope()
    {
        Vector2 checkPosV = transform.position - new Vector3(0f, 1.45f);
        Vector2 checkPosH = transform.position - new Vector3(0f, 1.45f);

        USH(checkPosH);
        USV(checkPosV);
    }

    private void USH(Vector2 checkPosH)
    {
        RaycastHit2D hitFront = Physics2D.Raycast(checkPosH, transform.right, HSlopeCheckDistance, currentGroundType);
        RaycastHit2D hitBack = Physics2D.Raycast(checkPosH, -transform.right, HSlopeCheckDistance, currentGroundType);
        Debug.DrawLine(checkPosH, new Vector2(checkPosH.x + HSlopeCheckDistance, checkPosH.y), Color.green);
        Debug.DrawLine(checkPosH, new Vector2(checkPosH.x - HSlopeCheckDistance, checkPosH.y), Color.green);


        //nah don't let USV roam free only do check within the method, change this
        if (playerInputX >= 0)
        {
            if (hitFront)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
                Debug.DrawRay(hitFront.point, hitFront.normal, Color.red);
            }
            else if (hitBack)
            {
                if (!USVisValid)
                {
                    isOnSlope = true;
                    slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
                    Debug.DrawRay(hitBack.point, hitBack.normal, Color.red);
                }
            }
            else
            {
                slopeSideAngle = 0.0f;
                isOnSlope = false;
            }
        }
        else if (playerInputX < 0)
        {
            if (hitBack)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
                Debug.DrawRay(hitBack.point, hitBack.normal, Color.red);
            }
            else if (hitFront)
            {
                if (!USVisValid)
                {
                    isOnSlope = true;
                    slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
                    Debug.DrawRay(hitFront.point, hitFront.normal, Color.red);
                }
            }
            else
            {
                slopeSideAngle = 0.0f;
                isOnSlope = false;
            }
        }

        //Debug.Log(slopeSideAngle);

        if (slopeAngle > maximumSlopeAngle || slopeSideAngle > maximumSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
    }

    private void USV(Vector2 checkPosV)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosV, Vector2.down, VSlopeCheckDistance, currentGroundType);
        Debug.DrawLine(checkPosV, new Vector2(checkPosV.x, checkPosV.y - VSlopeCheckDistance), Color.green);

        if (hit)
        {
            USVisValid = true;
            //slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != oldSlopeAngle)
            {
                isOnSlope = true;
            }

            oldSlopeAngle = slopeAngle;

            //Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            //Debug.DrawRay(hit.point, hit.normal, Color.red);

            return;
        }

        USVisValid = false;
    }

}


