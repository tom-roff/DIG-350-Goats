using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 4f;
    public float jumpForce = 20f;
    public float groundCheckDistance = 0.1f;
    private float horizontalInput;
    private float verticalVelocity = 0f;
    private bool isGrounded;
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Check ground state
        CheckGrounded();
        
        // Get input
        horizontalInput = Input.GetAxis("Horizontal");
        
        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump");
            verticalVelocity += jumpForce;
            Move(horizontalInput, verticalVelocity);
        
            // Send movement to server
            MoveRpc(horizontalInput, verticalVelocity);
        }
        else
        {
            // Move horizontally
            Move(horizontalInput, verticalVelocity);
            
            // Send movement to server
            MoveRpc(horizontalInput, verticalVelocity);
        }
        
        // Apply gravity
        ApplyGravity();
    }
    
    private void CheckGrounded()
    {
        // Cast a ray downward from slightly above the player's feet
        Vector3 rayStart = transform.position - Vector3.up * 0.51f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance - 0.51f);
    }
    
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Apply gravity to vertical velocity
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            // Reset vertical velocity when grounded
            verticalVelocity = -0.001f; // Small negative value to keep player grounded
        }
        
        // Apply vertical movement
        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
    }
    
    private void Move(float horizontalInput, float verticalVel)
    {
        verticalVelocity = verticalVel;
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
        ApplyGravity();
    }
    
    [Rpc(SendTo.Server)]
    private void MoveRpc(float horizontalInput, float verticalVel)
    {
        // On server, set the vertical velocity and apply movement
        verticalVelocity = verticalVel;
        Move(horizontalInput, verticalVel);
        ApplyGravity();
    }
}
