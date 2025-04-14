using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class ClimbingPlayerMovement : NetworkBehaviour
{
    private int climbAmount = 2;
    
    [Header("References")]
    [SerializeField] private ClimbingManager climbingManager;

    private bool isRightArm = true;
    
    private void Start()
    {
        // Enable gyroscope and accelerometer
        Input.gyro.enabled = true;
    }
    
    private void Update()
    {
        
        // Get device acceleration
        Vector3 acceleration = Input.acceleration;
        
        // Debug output
        // Debug.Log($"Accel X: {acceleration.x:F2}, Y: {acceleration.y:F2}, Z: {acceleration.z:F2}");
        

        if (acceleration.x < -0.7 & acceleration.y > -0.8 & isRightArm)
        {
            Climb();
        }
        else if (acceleration.x > 0.7 & acceleration.y > -0.8 & !isRightArm)
        {
            Climb();
        }
    }

    private void Climb()
    {
        // Debug.Log($"Climbed!! Right Arm: {isRightArm}, X: {Input.acceleration.x}, Y: {Input.acceleration.y}");
        transform.position = new Vector3(transform.position.x, transform.position.y + climbAmount, transform.position.z);
        ToggleArm();
    }
    
    private void ToggleArm()
    {
        isRightArm = !isRightArm;
    }
}
