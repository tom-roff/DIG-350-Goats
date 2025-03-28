using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private float horizontalInput;

    void Update()
    {
        // Only process input for the player we own
        if (!IsOwner) return;
        
        horizontalInput = Input.GetAxis("Horizontal");
        
        // Move locally for responsive feel
        Move(horizontalInput);
        
        // Send movement to server to sync with other clients
        MoveServerRpc(horizontalInput);
    }

    private void Move(float horizontalInput)
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRpc(float horizontalInput)
    {
        Move(horizontalInput);
    }
}
