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
        Input.gyro.enabled = true;
    }
    
    private void Update()
    {
        
        Vector3 acceleration = Input.acceleration;
        

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
        transform.position = new Vector3(transform.position.x, transform.position.y + climbAmount, transform.position.z);
        ToggleArm();
        climbingManager.UpdatePlayerHeightRpc(NetworkManager.Singleton.LocalClientId, transform.position.y);
    }
    
    private void ToggleArm()
    {
        isRightArm = !isRightArm;
    }
}
