using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class ClimbingPlayerMovement : NetworkBehaviour
{
    private enum ClimbState
    {
        Ready,
        Reaching,
        Grabbing
    }
        [Header("Acceleration Thresholds")]
    [SerializeField] private float reachThreshold = 0.8f; // Upward acceleration threshold (0-1 scale)
    [SerializeField] private float grabThreshold = -0.4f; // Forward tilt acceleration threshold (negative value)

    
    [Header("Climbing Settings")]
    [SerializeField] private float climbAmount = 0.5f;
    [SerializeField] private float rotationThreshold = 30.0f;
    // [SerializeField] private float grabThreshold = -30.0f;
    [SerializeField] private float cooldownTime = 0.5f;
    
    [Header("References")]
    [SerializeField] private ClimbingManager climbingManager;
    
    private ClimbState currentState = ClimbState.Ready;
    private bool isRightArm = true;
    private bool canStateChange = true;
    private Vector3 baseRotation;
    
    private void Start()
    {
        // Enable gyroscope and accelerometer
        Input.gyro.enabled = true;
        
        // Store initial rotation as a reference point
        baseRotation = Input.gyro.gravity;
        
        Debug.Log("Climbing system initialized. Base rotation: " + baseRotation);
    }
    
    private void Update()
    {
        // Fallback for testing in editor
        // if (!Application.isMobilePlatform && Application.isEditor)
        // {
        //     HandleKeyboardInput();
        //     return;
        // }
        
        // Get device acceleration
        Vector3 acceleration = Input.acceleration;
        
        // Debug output
        Debug.Log($"State: {currentState}, Accel X: {acceleration.x:F2}, Y: {acceleration.y:F2}, Z: {acceleration.z:F2}");
        
        // State machine for climbing
        switch (currentState)
        {
            case ClimbState.Ready:
                // Detect reaching motion (upward acceleration)
                // When device is held normally, Y is up/down
                bool isReaching = acceleration.y > reachThreshold;
                
                if (isReaching && canStateChange)
                {
                    currentState = ClimbState.Reaching;
                    Debug.Log($"{(isRightArm ? "Right" : "Left")} arm reaching up. Y-accel: {acceleration.y:F2}");
                }
                break;
                
            case ClimbState.Reaching:
                // Detect grabbing motion (forward tilt - negative Z acceleration)
                // When tilted forward, Z acceleration becomes more negative
                bool isGrabbing = acceleration.z < grabThreshold;
                
                if (isGrabbing && canStateChange)
                {
                    GrabAndClimb();
                    currentState = ClimbState.Grabbing;
                    StartCoroutine(ResetStateAfterDelay(cooldownTime));
                    Debug.Log($"Grabbed! Z-accel: {acceleration.z:F2}");
                }
                break;
                
            case ClimbState.Grabbing:
                // Wait for cooldown in coroutine before returning to Ready
                break;
        }
    }

    
    private void GrabAndClimb()
    {
        // Move player up
        transform.position += Vector3.up * climbAmount;
        
        // Switch arms
        ToggleArm();
        
        // Update network position
        if (IsOwner && NetworkManager.Singleton != null)
        {
            climbingManager.UpdatePlayerHeightRpc(NetworkManager.Singleton.LocalClientId, transform.position.y);
            
            // Check for finish
            if (transform.position.y >= climbingManager.GetFinishHeight())
            {
                climbingManager.PlayerFinished(NetworkManager.Singleton.LocalClientId);
            }
        }
        
        Debug.Log($"Climbed! New height: {transform.position.y:F2}");
    }
    
    private IEnumerator ResetStateAfterDelay(float delay)
    {
        canStateChange = false;
        yield return new WaitForSeconds(delay);
        currentState = ClimbState.Ready;
        canStateChange = true;
        Debug.Log("Reset to Ready state");
    }
    
    // Convert gyroscope quaternion to Unity coordinate system
    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
    
    private void HandleKeyboardInput()
    {
        if (currentState != ClimbState.Ready) return;
        
        bool reachInput = isRightArm ? Input.GetKey(KeyCode.D) : Input.GetKey(KeyCode.A);
        bool grabInput = Input.GetKeyDown(KeyCode.W);
        
        if (reachInput)
        {
            currentState = ClimbState.Reaching;
            
            if (grabInput)
            {
                GrabAndClimb();
                currentState = ClimbState.Grabbing;
                StartCoroutine(ResetStateAfterDelay(cooldownTime));
            }
        }
    }
    
    public void ToggleArm()
    {
        isRightArm = !isRightArm;
        Debug.Log($"Switched to {(isRightArm ? "right" : "left")} arm");
    }
}
