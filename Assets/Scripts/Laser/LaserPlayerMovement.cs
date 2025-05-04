using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.VisualScripting;

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
    [SerializeField] private Rigidbody rb;
    private float deadzone = 0.1f;
    private float maxTilt = 0.6f;
    [SerializeField] private Material[] colorMaterials = new Material[6];

    private void Start() {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        // if (!IsServer){
        //     gameObject.GetComponent<Renderer>().material = colorMaterials[NetworkManager.Singleton.LocalClientId - 1];
        // }
        ConfigureCollisions();
    }
    

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        
        if (Mathf.Abs(acceleration.x) > deadzone)
        {
            float tiltAmount = Mathf.Abs(acceleration.x) - deadzone;
            
            float speedFactor = tiltAmount / (maxTilt - deadzone);
            
            horizontalInput = Mathf.Sign(acceleration.x) * Mathf.Clamp(speedFactor, 0f, 1f);
        }
        else
        {
            horizontalInput = 0;
        }
        
        // Jump logic remains unchanged
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

        // CheckInBounds();
        
        // Check ground state
        CheckGrounded();
        
        // Apply gravity
        ApplyGravity();
        
        // Pass current bounce vector along with movement input
        MoveRpc(horizontalInput, verticalVelocity, bounceVector, isBouncing);
    }
    
    // private void CheckInBounds()
    // {
    //     if (transform.position.y < 0.5)
    //     {
    //         transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
    //     }
    //     if (transform.position.x < -4.75)
    //     {
    //         transform.position = new Vector3(-4.75f, transform.position.y, transform.position.z);
    //     }
    //     else if (transform.position.x > 4.75)
    //     {
    //         transform.position = new Vector3(4.75f, transform.position.y, transform.position.z);
    //     }
    //     if (transform.position.y < -100)
    //     {
    //         transform.position = new Vector3(0, 1f, 0);
    //     }
    // }

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

    
    private void Move(float horizontalInput, float verticalVel, Vector2 bounceVec, bool isBouncing) {
        // Calculate horizontal force
        Vector3 moveForce = new Vector3(horizontalInput * moveSpeed, 0, 0);
        
        // Apply controlled movement
        rb.linearVelocity = new Vector3(moveForce.x, rb.linearVelocity.y, 0);
        
        // For jumps, apply impulse force
        if (verticalVel > 0 && verticalVelocity != rb.linearVelocity.y) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, verticalVel, 0);
        }
        
        // Add bounce forces if needed
        if (isBouncing) {
            rb.AddForce(new Vector3(bounceVec.x, 0, bounceVec.y) * bounceForce, ForceMode.Impulse);
        }
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

    [Rpc(SendTo.NotServer)]
    private void SyncPositionRpc(Vector3 serverPosition) {
        if (!IsOwner) return;
        
        // If client is too far from server position, correct it
        if (Vector3.Distance(transform.position, serverPosition) > 1.0f) {
            transform.position = serverPosition;
        }
    }

    // Call this periodically from the server, perhaps every 0.5-1 seconds
    private void SyncPositions() {
        foreach (var player in FindObjectsOfType<LaserPlayerMovement>()) {
            player.SyncPositionRpc(player.transform.position);
        }
    }

    private void ConfigureCollisions() {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

}
