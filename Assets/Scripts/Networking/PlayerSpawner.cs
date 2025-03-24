using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    // Dictionary to track which spawn points are occupied
    private Dictionary<int, bool> occupiedSpawnPoints = new Dictionary<int, bool>();

    void Start()
    {
        Debug.Log("Start Everyone");
        if (!IsServer) return;

        Debug.Log("Start Server");

        // Initialize spawn point tracking
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            occupiedSpawnPoints[i] = false;
        }

        // Spawn all currently connected players
        SpawnAllPlayers();
        
        // Subscribe to client connection event for future connections
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerOnJoin;
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("On Network Spawn not is server");
        if (!IsServer) return;

        Debug.Log("On Network Spawn");

        // Initialize spawn point tracking
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            occupiedSpawnPoints[i] = false;
        }

        // Spawn all currently connected players
        SpawnAllPlayers();
        
        // Subscribe to client connection event for future connections
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerOnJoin;
    }

    private void SpawnPlayerOnJoin(ulong clientId)
    {
        if (IsServer)
        {
            SpawnPlayer(clientId);
        }
    }
    
    public void SpawnAllPlayers()
    {
        if (!IsServer) return;

        int count = 0;
        Debug.Log(count);
        
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
            count += 1;
            Debug.Log(count);
        }
    }
    
    private void SpawnPlayer(ulong clientId)
    {
        // Find an available spawn point
        int spawnIndex = GetAvailableSpawnPoint();
        if (spawnIndex == -1)
        {
            Debug.LogError("No available spawn points!");
            return;
        }
        
        // Mark spawn point as occupied
        occupiedSpawnPoints[spawnIndex] = true;
        
        // Instantiate player at spawn point
        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        Quaternion spawnRotation = spawnPoints[spawnIndex].rotation;
        
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        
        // Spawn with ownership assigned to the client
        networkObject.SpawnWithOwnership(clientId);
    }
    
    private int GetAvailableSpawnPoint()
    {
        // Find first unoccupied spawn point
        foreach (var kvp in occupiedSpawnPoints)
        {
            if (!kvp.Value)
                return kvp.Key;
        }
        
        // If all spawn points are occupied, return -1
        return -1;
    }
    
    // Call this when a player disconnects to free up their spawn point
    public void FreeSpawnPoint(int spawnIndex)
    {
        if (occupiedSpawnPoints.ContainsKey(spawnIndex))
        {
            occupiedSpawnPoints[spawnIndex] = false;
        }
    }
}
