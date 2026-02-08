using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    private Rigidbody rb;
    private Spring spring;
    private ConfigurableJoint joint;

    [Header("Grappling Settings")]
    public float maxGrappleDistance = 50f;
    public float grappleDelayTime = 0.15f;
    public float overShootYAxis = 2f;

    [Header("Physics Settings")]
    public float attractionForce = 30f;
    public float timeBeforeAttraction = 2f;
    public float jointSpring = 4.5f;
    public float jointDamper = 7f;
    public float jointMassScale = 4.5f;

    [Header("Cooldown")]
    public float grapplingCd = 0.5f;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    [Header("Rope Appearance")]
    public int quality = 30;
    public float damper = 7f;
    public float strength = 800f;
    public float velocity = 15f;
    public float waveCount = 3f;
    public float waveHeight = 1f;
    public AnimationCurve affectCurve;

    [Header("Vfx")]
    [SerializeField] ParticleSystem VfxGrab1;
    [SerializeField] GameObject VfxGrab2;

    [HideInInspector] public bool bIsGrappling;
    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    private float grappleTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spring = new Spring();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grapplingKey) && !bIsGrappling && grapplingCdTimer <= 0)
            StartGrapple();

        if (Input.GetKeyUp(grapplingKey) && bIsGrappling)
            StopGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        if (bIsGrappling)
            grappleTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (bIsGrappling && grappleTimer >= timeBeforeAttraction)
        {
            if (joint != null) Destroy(joint);

            Vector3 directionToPoint = (grapplePoint - transform.position).normalized;
            rb.AddForce(directionToPoint * attractionForce, ForceMode.Force);
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        if (!pm.CanGrab) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            bIsGrappling = true;
            grappleTimer = 0f;
            grapplePoint = hit.point;
            pm.bIsFreeze = true;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

            if (VfxGrab2 != null)
                Instantiate(VfxGrab2, grapplePoint, Quaternion.identity);

            lr.enabled = true;
        }
    }

    private void ExecuteGrapple()
    {
        if (!bIsGrappling) return;

        pm.bIsFreeze = false;

        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distance = Vector3.Distance(transform.position, grapplePoint);
        joint.linearLimit = new SoftJointLimit { limit = distance };

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimitSpring sp = new SoftJointLimitSpring { spring = jointSpring, damper = jointDamper };
        joint.linearLimitSpring = sp;
        joint.massScale = jointMassScale;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
    }

    public void StopGrapple()
    {
        bIsGrappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;

        if (joint != null) Destroy(joint);
        if (VfxGrab1 != null) VfxGrab1.Stop();

        pm.bIsFreeze = false;
        CancelInvoke(nameof(ExecuteGrapple));
    }

    private void DrawRope()
    {
        if (!bIsGrappling)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0) lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
            if (VfxGrab1 != null) VfxGrab1.Play();
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 gunTipPos = gunTip.position;
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPos).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
            lr.SetPosition(i, Vector3.Lerp(gunTipPos, currentGrapplePosition, delta) + offset);
        }
    }
}