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
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        transform.position = new Vector3(transform.position.x, transform.position.y + climbAmount, transform.position.z);
        ToggleArm();
        climbingManager.UpdatePlayerHeightRpc(clientId, transform.position.y);
        if (transform.position.y >= climbingManager.finishLine)
        {
            climbingManager.PlayerFinished(clientId);
        }
    }
    
    private void ToggleArm()
    {
        isRightArm = !isRightArm;
    }
}
