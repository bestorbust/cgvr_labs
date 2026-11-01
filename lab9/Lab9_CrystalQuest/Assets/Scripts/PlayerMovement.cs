
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private float horizontal;
    private float vertical;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("PlayerMovement initialized!");
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        Vector3 velocity = rb.linearVelocity;

        rb.linearVelocity = new Vector3(
            movement.x * moveSpeed,
            velocity.y,
            movement.z * moveSpeed
        );

        if (movement.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
