using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] public Material[] colorMaterials = new Material[6];
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private OurNetwork ourNetwork;
    private int spawnsUsed = 0;
    ulong hostId;
    
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        hostId = NetworkManager.Singleton.LocalClientId;

        ourNetwork = FindFirstObjectByType<OurNetwork>();
    }

    public void SpawnPlayers()
    {
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
        //Pretty sure this should be clientId - 1 because clientId starts at 0 (host) but we dont spawn the host.
        playerInstance.GetComponent<Renderer>().material = colorMaterials[(int)clientId - 1];
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(clientId);
        NetworkObjectReference objRef = new NetworkObjectReference(playerInstance.GetComponent<NetworkObject>());
        SetCapsuleColorRpc((int)clientId, objRef);
    }

    [Rpc(SendTo.Everyone)]
    void SetCapsuleColorRpc(int clientId, NetworkObjectReference instance){

        if (instance.TryGet(out NetworkObject targetNetObj))
            {
                GameObject targetGO = targetNetObj.gameObject;
                targetGO.GetComponent<Renderer>().material = colorMaterials[clientId - 1];
                // Apply color change or other logic here
            }

    }
}
