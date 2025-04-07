using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ClimbingManager : NetworkBehaviour
{
    private int finishLine = 30;

    [SerializeField] private GameObject playerUI;
    
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
    
    public void PlayerFinished(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != 0)
        {
            playerUI.gameObject.SetActive(true);
        }
    }
}
