using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Skill")]
    public bool CanWallRun = true;
    public bool CanClimb = true;
    public bool CanGrab = true;

    [Header("Mouvement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    public float wallrunSpeed;
    
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Wallrun")]
    public bool wallRunning;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Slope")]
    public float maxSlopAngle;
    private RaycastHit slopeHit;
    bool exitingSlope;

    [Header("KeyBinds")]
    public KeyCode jumpKey     = KeyCode.Space;
    public KeyCode sprintKey   = KeyCode.LeftShift;
    public KeyCode crouchKey   = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Respawn")]
    public Vector3 Checkpoint;
    
    [Header("References")]
    public Transform orientation;
    public Climbing climbingScript;
    [SerializeField] WallRunning WallRunningScript;
    [SerializeField] Grappling GrapplingScript;


    [Header("Vfx")]
    [SerializeField] ParticleSystem VfxTap;
    [SerializeField] ParticleSystem VfxSpeed;
    [SerializeField] ParticleSystem VfxJump;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private Vector3 velocityToSet;

    private bool enableMovementOnNextTouch;

    [Header("State")]
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        wallrunning,
        freeze,
        air
    }

    public bool bIsFreeze;

    public bool activeGrapple;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        Checkpoint = gameObject.transform.position;
    }

  
    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        Vfx();

        if (grounded && !activeGrapple)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

    }
    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when jump
        if(Input.GetKey(jumpKey)&& readyToJump && grounded)
        {
            readyToJump = false;

            VfxJump.Play();

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);

        }

        //Crouch
        if (Input.GetKeyDown(crouchKey))
        {

            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {

            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);


        }

    }

    private void StateHandler()
    {

        //Freeze
        if (bIsFreeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.linearVelocity = Vector3.zero;
        }
        //  Sprint
        else if (Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
   
        }
        // crouch
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (wallRunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }
        // walk
        else if (grounded && !Input.GetKey(sprintKey))
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        if (climbingScript.bExtigingWall) return;

             // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (Onslope()&&!exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f* airMultiplier, ForceMode.Force);
        }

        if (!wallRunning)
        {
            rb.useGravity = !Onslope();
        }

    }

    private void SpeedControl()
    {
        if (activeGrapple) return;
        // limiting speed on slope
        if (Onslope()&& !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }

        }  

    }

    private void Jump()
    {

        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;


    }

    public void JumpToPosition (Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestriction), 3f);
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = velocityToSet;

    }

    private bool Onslope()
    {
        if(Physics.Raycast(transform.position,Vector3.down, out slopeHit, playerHeight*0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopAngle && angle != 0;

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        return false;
    }

    private void Vfx()
    {

        if(state == MovementState.sprinting && grounded)
        {
            if (!VfxSpeed.isPlaying)
            {
                VfxSpeed.Play();
            }
            if (!VfxTap.isPlaying)
            {
                VfxTap.Play();
            }
      
        }
        else if(state == MovementState.sprinting && !grounded)
        {
            if (!VfxSpeed.isPlaying)
            {
                VfxSpeed.Play();
            }
            if (VfxTap.isPlaying)
            {
                VfxTap.Stop();
            }
        }
        else
        {
            if (VfxSpeed.isPlaying)
            {
                VfxSpeed.Stop();

            }
            if (VfxTap.isPlaying)
            {
                VfxTap.Stop();
            }

        }
    }

    public void ResetRestriction()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestriction();

            GetComponent<Grappling>().StopGrapple();
        }

        if (collision.gameObject.layer == 10)
        {
            gameObject.transform.position = Checkpoint;

        }

        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            Debug.Log("Checkpoint");
            Checkpoint = other.gameObject.transform.GetChild(0).transform.position;
        }

        if (other.gameObject.layer == 12)
        {
           SkillCheckpoint skillCheckpoint = other.gameObject.GetComponent<SkillCheckpoint>();
           CanWallRun = skillCheckpoint.CanWallRun;
           CanClimb = skillCheckpoint.CanClimb;
           CanGrab = skillCheckpoint.CanGrab;

        }

    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

}
