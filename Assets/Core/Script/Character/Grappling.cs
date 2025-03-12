using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overShootYAxis;

    private Vector3 grapplePoint;

    [Header("CoolDown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    private Spring spring;

    [Header("Rope")]
    public LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;

    [Header("Vfx")]
    [SerializeField] ParticleSystem VfxGrab1;
    [SerializeField] GameObject VfxGrab2;

    [HideInInspector]public bool bIsGrappling;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        spring = new Spring();
    }
    private void Update()
    {
        if (Input.GetKeyDown(grapplingKey) && !bIsGrappling)
        {
            StartGrapple();
            
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        DrawRope();   
    }

    private void StartGrapple()
    {
        if(pm.CanGrab == true)
        {
            if (grapplingCdTimer > 0)
            {
                return;
            }

            bIsGrappling = true;

            pm.bIsFreeze = true;

            //Raycast to find if we hit a target
            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;

                Invoke(nameof(ExecuteGrapple), grappleDelayTime);

                Instantiate(VfxGrab2, grapplePoint, Quaternion.identity);

            }
            else
            {
                grapplePoint = cam.position + cam.forward * maxGrappleDistance;

                Invoke(nameof(StopGrapple), grappleDelayTime);
            }

            lr.enabled = true;

        }

       
    }
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!bIsGrappling)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }


        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
            VfxGrab1.Play();
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var gunTipPosition = gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
    private void ExecuteGrapple()
    {
        pm.bIsFreeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }
    public void StopGrapple()
    {
        bIsGrappling = false;

        pm.bIsFreeze = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
}
