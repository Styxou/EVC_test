using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{

    [Header("PlayerGrind")]
    [HideInInspector] public bool onRail;
    [SerializeField] float grindSpeed;
    [SerializeField] float speedUp;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float timeOnSpline;
    bool bForward = true;
    bool bSpeedUp = false;

    [Header("References")]
    public RailScript currentRailScript;
    [SerializeField] LayerMask whatIsRail;
    [SerializeField] Camera cam;
    Rigidbody rb;
    Wapeon Weapon;

    Grappling Grappling;

    [Header("Force")]
    [SerializeField] float ForceToAddUp;
    [SerializeField] float ForceToAddForward;
    [SerializeField] float ForceJump;

    [Header("VFX")]
    [SerializeField] ParticleSystem Particle;
    [SerializeField] GameObject VfxBzzz1;
    [SerializeField] GameObject VfxBzzz2;
    Animator AnimatorVfxBzzz1;
    Animator AnimatorVfxBzzz2;

    private void Awake()
    {


        rb = GetComponent<Rigidbody>();
        Weapon = GetComponent<Wapeon>();
        Grappling = GetComponent<Grappling>();

        AnimatorVfxBzzz1 = VfxBzzz1.GetComponent<Animator>();
        AnimatorVfxBzzz2 = VfxBzzz2.GetComponent<Animator>();

        ///Active VFX gameObject
        VfxBzzz1.SetActive(true);
        VfxBzzz2.SetActive(true);

        ///Play Animation 
        AnimatorVfxBzzz1.Play("A_Bzzz");
        AnimatorVfxBzzz2.Play("A_Bzzz");

        ///Stop Animation 
        AnimatorVfxBzzz1.speed = 0;
        AnimatorVfxBzzz2.speed = 0;

        ///Unactive VFX gameObject
        VfxBzzz1.SetActive(false);
        VfxBzzz2.SetActive(false);
    }

   

    private void FixedUpdate()
    {
        //If on the rail, move the player along the rail
        if (onRail)
        {
            MovePlayerAlongRail();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (onRail)
        {
            InputGrind();
        }


    }
    void InputGrind()
    {
        ///Move
        if (Input.GetKeyDown(KeyCode.W))
        {
            bForward = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            bForward = false;

        }

        ///SpeedUp
        if (bForward == true && Input.GetKey(KeyCode.LeftShift))
        {
            bSpeedUp = true;
            if(Particle.isPlaying == false)
            {
                Particle.Play();
            }
        }
        else if (bForward == false && Input.GetKey(KeyCode.LeftShift))
        {
            bSpeedUp = true;
            if (Particle.isPlaying == false)
            {
                Particle.Play();
            }
        }

        else if (bSpeedUp == true && Input.GetKeyUp(KeyCode.LeftShift))
        {
            bSpeedUp = false;
            if(Particle.isPlaying == true)
            {
                Particle.Stop();
            }
        }
        ///Grappling
        if (Grappling.bIsGrappling)
        {
            ThrowOffRail();
            ResetPlayerGrind();
        }
        ///Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpOffRail();
            ResetPlayerGrind();
        }
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //progress
            float progress = timeOnSpline / timeForFullSpline;

            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                ResetPlayerGrind();
                return;
            }

            //Calculating the local positions of the player's current position and next position
            float3 pos, tangent, up;
            // float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);

            //Converting the local positions into world positions.
            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);


            transform.position = worldPos + (transform.up * heightOffset);

            //Finally incrementing or decrementing elapsed time for the next update based on direction.
            if (currentRailScript.normalDir)
            {
                if (bForward == true)
                {
                    if (bSpeedUp == false)
                    {
                        timeOnSpline += Time.deltaTime;
                    }
                    else if (bSpeedUp == true)
                    {
                        timeOnSpline += Time.deltaTime * 2;
                    }
                }
                if (bForward == false)
                {
                    if (bSpeedUp == false)
                    {
                        timeOnSpline -= Time.deltaTime;
                    }
                    else if (bSpeedUp == true)
                    {
                        timeOnSpline -= Time.deltaTime * 2;

                    }
                }
            }
            else
            {
                if (bForward == true)
                {
                    if (bSpeedUp == false)
                    {
                        timeOnSpline -= Time.deltaTime;
                    }
                    else if (bSpeedUp == true)
                    {
                        timeOnSpline -= Time.deltaTime*2;

                    }

                }
                if (bForward == false)
                {
                    if (bSpeedUp == false)
                    {
                        timeOnSpline += Time.deltaTime;
                    }
                    else if (bSpeedUp == true)
                    {
                        timeOnSpline += Time.deltaTime * 2;
                    }

                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Weapon.IsChained = true;
            Weapon.ActivateUI();
            onRail = true;
            currentRailScript = collision.gameObject.GetComponent<RailScript>();
            CalculateAndSetRailPosition();

            ///Active VFX gameObject
            VfxBzzz1.SetActive(true);
            VfxBzzz2.SetActive(true);

            ///Play Animation 
            AnimatorVfxBzzz1.speed = 1;
            AnimatorVfxBzzz2.speed = 1;
        }
    }

    void CalculateAndSetRailPosition()
    {
        //What time to spend on spline

        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;



        Vector3 splinePoint;

        //The value of the player's position on the spline.
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        timeOnSpline = timeForFullSpline * normalisedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        currentRailScript.CalculateDirection(forward, cam.transform.forward);
        transform.position = splinePoint + (transform.up * heightOffset);
    }
    void ThrowOffRail()
    {
        Vector3 forceToApplyUp = cam.transform.up * ForceToAddUp;
        Vector3 forceToApplyForward = cam.transform.forward * ForceToAddForward;
        
        //add force to throwAway
        if (bForward == true)
        {
            rb.AddForce(forceToApplyUp, ForceMode.Impulse);
            rb.AddForce(forceToApplyForward, ForceMode.Impulse);
        }
        else if (bForward == false)
        {
            rb.AddForce(forceToApplyUp, ForceMode.Impulse);
            rb.AddForce(-forceToApplyForward, ForceMode.Impulse);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }

    void JumpOffRail()
    {
        Vector3 forceToApplyUp = cam.transform.up * ForceJump;
        Vector3 forceToApplyForward = cam.transform.forward * ForceToAddForward;

        //add force
        rb.AddForce(forceToApplyUp, ForceMode.Impulse);
        rb.AddForce(forceToApplyForward, ForceMode.Impulse);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

       
    }

    private void ResetPlayerGrind()
    {
        onRail = false;
        currentRailScript = null;
        timeForFullSpline = 0f;
        timeOnSpline = 0f;
        bSpeedUp = false;
        bForward = true;

        ///Stop Animation 
        AnimatorVfxBzzz1.speed = 0;
        AnimatorVfxBzzz2.speed = 0;

        ///Unactive VFX gameObject
        VfxBzzz1.SetActive(false);
        VfxBzzz2.SetActive(false);

        if (Particle.isPlaying == true)
        {
            Particle.Stop();
        }
    }

}
