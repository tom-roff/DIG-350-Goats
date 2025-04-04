using UnityEngine;
using Unity.Netcode;

public class LaserPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 10f;
    public float jumpForce = 15f;
    public float bounceForce = 2f;
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

        CheckInBounds();
        
        // Check ground state
        CheckGrounded();
        
        // Apply gravity
        ApplyGravity();
        
        MoveRpc(horizontalInput, verticalVelocity);
    }
    
    private void CheckInBounds()
    {
        if (transform.position.y < 0.5)
        {
            transform.position = new Vector3 (transform.position.x, 0.5f, transform.position.z);
        }
        if (transform.position.x < -4.75)
        {
            transform.position = new Vector3 (-4.75f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 4.75)
        {
            transform.position = new Vector3 (4.75f, transform.position.y, transform.position.z);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        
        LaserPlayerMovement otherPlayer = collision.gameObject.GetComponent<LaserPlayerMovement>();
        
        if (collision.gameObject.CompareTag("Player"))
        {
            // Calculate bounce direction based on positions
            Vector3 bounceDir = (transform.position - otherPlayer.transform.position).normalized;
            
            // Apply immediate bounce to both players
            // This player bounces in the direction away from the other player
            transform.position += bounceDir * bounceForce * 0.1f;
            // Add a small upward bounce
            verticalVelocity = Mathf.Max(verticalVelocity, jumpForce * 0.5f);
            
            // Other player bounces in the opposite direction
            otherPlayer.transform.position -= bounceDir * bounceForce * 0.1f;
            // Add a small upward bounce to other player
            otherPlayer.verticalVelocity = Mathf.Max(otherPlayer.verticalVelocity, jumpForce * 0.5f);
        }
    }
}
