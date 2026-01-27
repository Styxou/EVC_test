using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public PlayerCam cam;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float jumpForce;
    private float climbTimer;
    
    private bool climbing;

    [Header("ClimbJump")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;
    public int intClimbJump;
    private int intClimbJumpsLeft;
    private int intClimbJumpsRight;

    [Header("Exiting")]
    public bool bExtigingWall;
    public float fExitiWallTime;
    private float exitWallTimer;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    public float minWallNormalAngleChange;
    private float wallLookAngle;

    [Header("Camera")]
    public float camFov;
    public float camPan;

    [Header("Input")]
    public KeyCode climbKey = KeyCode.W;
    public KeyCode jumpKey = KeyCode.Space;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform tLastWall;
    private Vector3 lastWallNormal;

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();
        if (climbing && !bExtigingWall)
        {
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        //State 1 - Climbing
        if (wallFront && Input.GetKey(climbKey) && wallLookAngle < maxWallLookAngle && !bExtigingWall)
        {

            if (!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            //Timer
            if(climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }

            if(climbTimer < 0)
            {
                StopClimbing();
                ClimbJumpOutOfTime();
            }

          //  cam.DoFov(camFov);
            //cam.DoPan(camPan);

        }

        //State 2 - Exiting
        else if (bExtigingWall)
        {
            if (climbing)
            {
                StopClimbing();
            }
            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer < 0)
            {
                bExtigingWall = false;
            }
        }

        //State 3 - None
        else
        {
            if (climbing)
            {
                StopClimbing();
            }
        }

        if(wallFront && Input.GetKeyDown(jumpKey) && intClimbJumpsLeft > 0)
        {
            ClimbJum();

        }
    }

    private void WallCheck()
    {
        //Sphere cast to detect wall
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != tLastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime;
            intClimbJumpsLeft = intClimbJump;
        }
    }

    private void StartClimbing()
    {

        if(pm.CanClimb == true)
        {
            climbing = true;

            tLastWall = frontWallHit.transform;
            lastWallNormal = frontWallHit.normal;
        }
    

    }
    private void ClimbingMovement()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);

    }
    private void StopClimbing()
    {
        climbing = false;
        //cam.DoFov(80f);
        //cam.DoPan(0f);
    }

    private void ClimbJumpOutOfTime()
    {
        //add force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(jumpForce * -orientation.forward, ForceMode.Impulse);
    }

    private void ClimbJum()
    {
        bExtigingWall = true;
        exitWallTimer = fExitiWallTime;

        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        intClimbJumpsLeft--;
    }

}
