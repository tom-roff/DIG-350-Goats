using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 9.8f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;
    // Removed groundLayer since we're checking against all objects
    
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
            verticalVelocity = jumpForce;
            JumpRpc();
        }
        
        // Apply gravity
        ApplyGravity();
        
        // Move horizontally
        Move(horizontalInput);
        
        // Send movement to server
        MoveRpc(horizontalInput, verticalVelocity);
    }
    
    private void CheckGrounded()
    {
        // Cast a ray downward from slightly above the player's feet
        // No layer mask means it will check against ANY collider
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance + 0.1f);
        
        // Visual debugging to see the ray (visible in Scene view when game is running)
        Debug.DrawRay(rayStart, Vector3.down * (groundCheckDistance + 0.1f), 
            isGrounded ? Color.green : Color.red, 0.1f);
    }
    
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Apply gravity to vertical velocity
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            // Reset vertical velocity when grounded
            verticalVelocity = -0.1f; // Small negative value to keep player grounded
        }
        
        // Apply vertical movement
        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
    }
    
    private void Move(float horizontalInput)
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
    
    [Rpc(SendTo.Server)]
    private void MoveRpc(float horizontalInput, float verticalVel)
    {
        // On server, set the vertical velocity and apply movement
        verticalVelocity = verticalVel;
        Move(horizontalInput);
        ApplyGravity();
    }
    
    [Rpc(SendTo.Server)]
    private void JumpRpc()
    {
        verticalVelocity = jumpForce;
    }
}
