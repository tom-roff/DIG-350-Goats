using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class LaserPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 10f;
    public float jumpForce = 15f;
    public float bounceForce = 5f;
    public float bounceDuration = 0.3f;
    
    private float horizontalInput;
    private float verticalVelocity = 0f;
    private bool isGrounded;
    private Vector2 bounceVector = Vector2.zero;
    private bool isBouncing = false;
    

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        
        float deadzone = 0.2f;
        
        if (acceleration.x < -deadzone)
        {
            horizontalInput = -1;
        }
        else if (acceleration.x > deadzone)
        {
            horizontalInput = 1;
        }
        else
        {
            horizontalInput = 0;
        }
        
        if (Input.touchCount > 0 && isGrounded)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                verticalVelocity = jumpForce;
            }
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
        
        // Pass current bounce vector along with movement input
        MoveRpc(horizontalInput, verticalVelocity, bounceVector, isBouncing);
    }
    
    private void CheckInBounds()
    {
        if (transform.position.y < 0.5)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
        if (transform.position.x < -4.75)
        {
            transform.position = new Vector3(-4.75f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 4.75)
        {
            transform.position = new Vector3(4.75f, transform.position.y, transform.position.z);
        }
        if (transform.position.y < -100)
        {
            transform.position = new Vector3(0, 1f, 0);
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
            verticalVelocity -= gravity * Time.fixedDeltaTime;
        }
    }

    
    private void Move(float horizontalInput, float verticalVel, Vector2 bounceVec, bool isBouncing)
    {
        // Calculate horizontal movement from input
        float horizontalMovement = horizontalInput * moveSpeed * Time.fixedDeltaTime;
        
        // If bouncing, add bounce force to movement
        if (isBouncing)
        {
            horizontalMovement += bounceVec.x * Time.fixedDeltaTime;
        }
        
        // Vertical movement
        float verticalMovement = verticalVel * Time.fixedDeltaTime;
        
        // Apply both movements
        transform.Translate(horizontalMovement, verticalMovement, 0f);
    }

    
    [Rpc(SendTo.Server)]
    private void MoveRpc(float horizontalInput, float verticalVel, Vector2 bounceVec, bool isBouncing)
    {
        // Update values on server
        verticalVelocity = verticalVel;
        
        // Move on server
        Move(horizontalInput, verticalVel, bounceVec, isBouncing);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            LaserPlayerMovement otherPlayer = collision.gameObject.GetComponent<LaserPlayerMovement>();
            
            // Calculate bounce direction based on positions
            Vector3 bounceDir = (transform.position - otherPlayer.transform.position).normalized;
            
            // Apply bounce to this player
            ApplyBounceClientRpc(bounceDir.x, bounceDir.z, NetworkManager.Singleton.LocalClientId);
            
            // Apply bounce to other player (in opposite direction)
            otherPlayer.ApplyBounceClientRpc(-bounceDir.x, -bounceDir.z, otherPlayer.OwnerClientId);
        }
    }
    
    [ClientRpc]
    private void ApplyBounceClientRpc(float dirX, float dirZ, ulong clientId)
    {
        // Only process this if it's for our client
        if (OwnerClientId != clientId) return;
        
        // Start bounce effect
        StartCoroutine(ApplyBounceEffect(new Vector2(dirX, dirZ)));
        
        // Add a small upward bounce immediately
        verticalVelocity = Mathf.Max(verticalVelocity, jumpForce * 0.5f);
    }
    
    private IEnumerator ApplyBounceEffect(Vector2 direction)
    {
        // Set bounce parameters
        bounceVector = direction * bounceForce;
        isBouncing = true;
        
        // Apply bounce for the specified duration
        float elapsedTime = 0f;
        
        while (elapsedTime < bounceDuration)
        {
            // Optional: Gradually reduce bounce force over time
            float timeRatio = 1 - (elapsedTime / bounceDuration);
            bounceVector = direction * bounceForce * timeRatio;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Reset bounce parameters
        bounceVector = Vector2.zero;
        isBouncing = false;
    }
}
