using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

    [Header("Mask")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

    [Header("WallRuning")]
    public float wallRunForce;
    public float maxWallRuntime;

    [Header("Climbing")]
    private float wallRunTimer;
    public float wallClimbing;

    [Header("WallJump")]
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.V;
    public KeyCode downwardsRunKey = KeyCode.B;
    private float horizontalInput;
    private float verticalInput;
    private bool upwardsRun;
    private bool downwardsRun;

    [Header("Detection")]
    public float wallCHeckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exit")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;
    public PlayerCam cam;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Camera")]
    public float camTilt;
    public float camFov;

    [Header("VFX")]
    [SerializeField] GameObject VfxDash;
    [SerializeField] GameObject VfxDash2;
    Animator AnimatorVfxDash1;
    Animator AnimatorVfxDash2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        AnimatorVfxDash1 = VfxDash.GetComponent<Animator>();
        AnimatorVfxDash2 = VfxDash2.GetComponent<Animator>();

        ///Active VFX gameObject
        VfxDash.SetActive(true);
        VfxDash2.SetActive(true);

        ///Play Animation 
        AnimatorVfxDash1.Play("A_Dash");
        AnimatorVfxDash2.Play("A_Dash");

        ///Stop Animation 
        AnimatorVfxDash1.speed = 0;
        AnimatorVfxDash2.speed = 0;

        ///Unactive VFX gameObject
        VfxDash.SetActive(false);
        VfxDash2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallRunning)
        {
            WallRunningMovement();
        }
    }

    private void checkForWall()
    {

        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCHeckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCHeckDistance, whatIsWall);
    }

    private bool AboveGround()
    {

        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);

    }

    private void StateMachine()
    {
        // get inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        upwardsRun = Input.GetKey(upwardsRunKey);
        downwardsRun = Input.GetKey(downwardsRunKey);
        // Sate 1 - Wallrun
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            //start running
            if (!pm.wallRunning)
            {
                StartWallRun();
            }

            // wallrun timer
            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;

            }

            if (wallRunTimer <= 0 && pm.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
                WallJump();

            }

            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }
        //State 2 - Exit
        else if (exitingWall)
        {
            if (pm.wallRunning)
            {
                stopWallRun();
             
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTime < 0)
            {
                exitingWall = false;
            }
        }

        // Sate 3 - None
        else
        {
            if (pm.wallRunning)
            {
                stopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        if(pm.CanWallRun == true)
        {
            pm.wallRunning = true;

            wallRunTimer = maxWallRuntime;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //cam fov
            cam.DoFov(camFov);
            if (wallLeft) cam.DoTile(-camTilt);
            if (wallRight) cam.DoTile(camTilt);

            VfxDash.SetActive(true);
            VfxDash2.SetActive(true);

            AnimatorVfxDash1.speed = 1;
            AnimatorVfxDash2.speed = 1;
        }

      

    }

    private void stopWallRun()
    {
        pm.wallRunning = false;
        exitingWall = false;

        ///Reset camera effectss
        ///
        cam.DoFov(80f);
        cam.DoTile(0f);

        VfxDash.SetActive(false);
        VfxDash2.SetActive(false);

        AnimatorVfxDash1.speed = 0;
        AnimatorVfxDash2.speed = 0;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //up/down
        if (upwardsRun)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbing, rb.linearVelocity.z);
        }
        if (downwardsRun)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbing, rb.linearVelocity.z);

        }

        //push to wall
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
        // weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);

        }
    }
    private void WallJump()
    {
        //enter exiting wall statre
        stopWallRun();

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //add force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    } 
}
