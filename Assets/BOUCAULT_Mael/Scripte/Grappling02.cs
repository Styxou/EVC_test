using UnityEngine;

namespace Athena.Prototype
{
    using UnityEngine;

    public class Grappling02 : MonoBehaviour
    {
        [Header("Settings")]
        public LayerMask whatIsGrappleable;
        public float maxDistance = 50f;
        public float pullSpeed = 50f;
        public KeyCode grappleKey = KeyCode.Mouse1;

        [Header("References")]
        public Transform cam;
        public Transform gunTip;
        public LineRenderer lr;

        private PlayerMove02 pm;
        private Vector3 grapplePoint;
        private bool isGrappling;

        private void Awake()
        {
            pm = GetComponent<PlayerMove02>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(grappleKey)) StartGrapple();
            if (Input.GetKeyUp(grappleKey)) StopGrapple();

            if (isGrappling)
            {
                Vector3 direction = (grapplePoint - transform.position).normalized;
                pm.GetComponent<Rigidbody>().AddForce(direction * pullSpeed, ForceMode.Acceleration);
            }
        }

        private void LateUpdate()
        {
            if (isGrappling)
            {
                lr.SetPosition(0, gunTip.position);
                lr.SetPosition(1, grapplePoint);
            }
        }

        private void StartGrapple()
        {
            if (!pm.CanGrab) return;

            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;
                isGrappling = true;
                lr.enabled = true;
                lr.positionCount = 2;
                pm.activeGrapple = true;
            }
        }

        public void StopGrapple()
        {
            isGrappling = false;
            lr.enabled = false;
            pm.activeGrapple = false;
        }
    }
}
