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
    [SerializeField] float maximumSlopeAngle;

    [Header("Movement Control")]
    [SerializeField] private float slopeCheckDistanceV;
    [SerializeField] private float slopeCheckDistanceH;

    //propeties
    public bool CanMove { get; set; }

    //References
    private Rigidbody2D controllerRigidbody;
    private CapsuleCollider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;

    //funnctional field
    private Vector2 colliderSize;
    private Vector3 Velocity = Vector3.zero;
    private Vector2 slopeNormalPerp;
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

    // Start is called before the first frame update
    void Start()
    {
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<CapsuleCollider2D>();
        softGroundMask = LayerMask.GetMask("Soft Ground");
        hardGroundMask = LayerMask.GetMask("Hard Ground");

        colliderSize = controllerCollider.size;

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
        if(controllerRigidbody.velocity.y < 0.0f)
        {
            isJumping = false; 
        }

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
        //Handles Jumps
        if (jumpIn && isOnGround && canWalkOnSlope)
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

        //toggles friction for slopes
        if (isOnSlope && moveIn == 0.0f && canWalkOnSlope)
        {
            controllerRigidbody.sharedMaterial = fullFriction;
        }
        else
        {
            controllerRigidbody.sharedMaterial = noFriction;
        }

        //implement running sound and animation.
    }

    private void UpdateGravity()
    { 
        if(!isJumping && !isOnGround)
        {
            controllerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityScale - 1) * Time.deltaTime;
        }
        else if (isJumping && !Input.GetButton("Jump"))
        {
            controllerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (jumpGravityScale - 1) * Time.deltaTime;
        }
    }

    private void UpdateSlope()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, 0.87f);

        USV(checkPos);
        USH(checkPos);
    }

    private void USH(Vector2 checkPos)
    {
        RaycastHit2D hitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistanceH, currentGroundType);
        RaycastHit2D hitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistanceH, currentGroundType);
        Debug.DrawLine(checkPos, new Vector2(checkPos.x + slopeCheckDistanceH, checkPos.y), Color.green);
        Debug.DrawLine(checkPos, new Vector2(checkPos.x - slopeCheckDistanceH, checkPos.y), Color.green);

        if (hitFront)
        { 
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
        }
        else if(hitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void USV(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistanceV, currentGroundType);
        Debug.DrawLine(checkPos, new Vector2(checkPos.x, checkPos.y - slopeCheckDistanceV), Color.green);

        if(hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeAngle != oldSlopeAngle)
            {
                isOnSlope = true;
            }

            oldSlopeAngle = slopeAngle; 

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.red);
        }

        if(slopeAngle > maximumSlopeAngle || (slopeSideAngle > maximumSlopeAngle && slopeSideAngle != 90))
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
    }

}
