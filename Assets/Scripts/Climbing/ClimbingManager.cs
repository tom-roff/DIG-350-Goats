using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ClimbingManager : NetworkBehaviour
{
    [SerializeField] private int finishLine = 100;
    
    // Dictionary to track each player's height
    private Dictionary<ulong, float> playerHeights = new Dictionary<ulong, float>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId) continue;

                playerHeights[clientId] = 0f;
            }
        }
    }

    // Called by player controllers to update their position
    [Rpc(SendTo.Server)]
    public void UpdatePlayerHeightRpc(ulong clientId, float height)
    {
        playerHeights[clientId] = height;

        Debug.Log("This was called");
        
        // Check if player has reached the finish line
        if (height >= finishLine)
        {
            PlayerFinished(clientId);
        }
    }

    public float GetPlayerHeight(ulong clientId)
    {
        if (playerHeights.ContainsKey(clientId))
        {
            return playerHeights[clientId];
        }
        return 0f; // Default value if player not found
    }

    public float GetFinishHeight()
    {
        return finishLine;
    }
    
    private void PlayerFinished(ulong clientId)
    {
        Debug.Log($"Player {clientId} has finished the climb!");
        // Implement any game-specific logic for when a player finishes
        // For example: announce winner, give rewards, etc.
    }
}
