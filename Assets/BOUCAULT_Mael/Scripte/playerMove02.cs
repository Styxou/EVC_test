using UnityEngine;

public class PlayerMove02 : MonoBehaviour
{
    [Header("Permissions")]
    public bool CanGrab = true;
    public bool CanWallRun = true;
    public bool CanClimb = true;

    [Header("Mouvement")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 12f;
    public float groundDrag = 5f;
    public float jumpForce = 5f;
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;

    [HideInInspector] public bool activeGrapple;
    [HideInInspector] public bool wallRunning;
    private bool grounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (!activeGrapple)
        {
            rb.linearDamping = grounded ? groundDrag : 0;
            if (Input.GetKeyDown(KeyCode.Space) && grounded) Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!activeGrapple && !wallRunning) MovePlayer();
    }

    private void MovePlayer()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = orientation.forward * z + orientation.right * x;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}