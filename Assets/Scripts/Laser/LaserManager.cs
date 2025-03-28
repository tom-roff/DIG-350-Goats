using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class LaserManager : NetworkBehaviour
{
    public Dictionary<ulong, int> scores = new Dictionary<ulong, int>();
    public Dictionary<ulong, bool> alive = new Dictionary<ulong, bool>();

    public GameObject laserPrefab;
    private float yPosition = 2f;
    private float zPosition = 10f;
    public float minX = -5f;
    public float maxX = 5f;
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    private float nextSpawnTime;
    private ulong hostId;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        hostId = NetworkManager.Singleton.LocalClientId;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != hostId)
            {
                scores[clientId] = 0;
                alive[clientId] = true;
            }
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnLaser();
            SetNextSpawnTime();
        }
    }

    private void SpawnLaser()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, yPosition, zPosition);
        
        GameObject laser = Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
        NetworkObject networkObject = laser.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    public void UpdateScores()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if ((clientId != hostId) && (alive[clientId] != false))
            {
                scores[clientId]++;
            }
        }
    }

    public int GetScore(ulong clientId)
    {
        return scores[clientId];
    }

    public void KillPlayer(ulong ClientId)
    {
        alive[ClientId] = false;
    }
}
