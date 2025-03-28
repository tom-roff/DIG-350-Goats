using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private int spawnsUsed = 0;
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        ulong hostId = NetworkManager.Singleton.LocalClientId;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != hostId)
            {
                SpawnPlayer(clientId, spawnsUsed);
                spawnsUsed++;
            }
        }
    }
    
    private void SpawnPlayer(ulong clientId, int spawnsUsed)
    {
        
        Vector3 spawnPosition = spawnPoints[spawnsUsed].position;
        Quaternion spawnRotation = spawnPoints[spawnsUsed].rotation;
        
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        
        networkObject.SpawnWithOwnership(clientId);
    }
}
