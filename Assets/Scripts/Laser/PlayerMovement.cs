using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 10f;
    public float jumpForce = 15f;
    private float horizontalInput;
    private float verticalVelocity = 0f;
    private bool isGrounded;
    

    void Update()
    {
        // Get input
        horizontalInput = Input.GetAxis("Horizontal");

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        
        // Check ground state
        CheckGrounded();
        
        // Apply gravity
        ApplyGravity();
        
        MoveRpc(horizontalInput, verticalVelocity);
    }
    
    private void CheckGrounded()
    {
        // Check if player is on the ground
        isGrounded = transform.position.y <= 0.51;
    }
    
    private void ApplyGravity()
    {
        if ((isGrounded) && (verticalVelocity < 0))
        {
            // When grounded, reset vertical velocity
            transform.position = new Vector3 (transform.position.x, 0.5f, transform.position.z);
            verticalVelocity = 0f;
        }
        else
        {
            // Apply gravity
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }
    
    private void Move(float horizontalInput, float verticalVel)
    {
        if (verticalVel != 0)
        {
            Debug.Log(verticalVel);
        }

        // Horizontal movement
        float horizontalMovement = horizontalInput * moveSpeed * Time.deltaTime;
        
        // Vertical movement
        float verticalMovement = verticalVel * Time.deltaTime;
        
        // Apply both movements
        transform.Translate(horizontalMovement, verticalMovement, 0f);
    }
    
    [Rpc(SendTo.Server)]
    private void MoveRpc(float horizontalInput, float verticalVel)
    {
        // Update values on server
        verticalVelocity = verticalVel;
        
        // Move on server
        Move(horizontalInput, verticalVel);
    }
}
