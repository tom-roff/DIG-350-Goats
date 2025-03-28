using UnityEngine;
using Unity.Netcode;

public class LaserManager : NetworkBehaviour
{
    public int score;
    public GameObject laserPrefab;
    private float yPosition = 2f;
    private float zPosition = 10f;
    public float minX = -5f;
    public float maxX = 5f;
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    private float nextSpawnTime;

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
        Debug.Log(score);
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

    public void AddScore()
    {
        score++;
    }

    public int GetScore()
    {
        return score;
    }
}
