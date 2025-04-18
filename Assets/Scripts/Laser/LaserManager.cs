using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class LaserManager : NetworkBehaviour
{
    public List<ulong> leaderboard = new List<ulong>();
    public Dictionary<ulong, int> scores = new Dictionary<ulong, int>();
    public Dictionary<ulong, bool> alive = new Dictionary<ulong, bool>();
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject deathUI;

    public GameObject laserPrefab;
    private float yPosition = 2f;
    private float zPosition = 10f;
    public float minX = -5f;
    public float maxX = 5f;
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    private float nextSpawnTime;
    private ulong hostId;
    
    // Sudden death mode variables
    private float gameTimer = 0f;
    private bool suddenDeathActive = false;
    public float suddenDeathStartTime = 20f;  // Time in seconds when sudden death begins
    public float suddenDeathSpeedupInterval = 1.5f;  // How often to increase spawn rate during sudden death
    private float suddenDeathTimer = 0f;
    public float suddenDeathSpeedupRate = 0.9f;  // Rate to multiply spawn intervals by (lower = faster spawns)
    private bool gameOver = false;
    
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
        
        // Initialize timers
        gameTimer = 0f;
        suddenDeathTimer = 0f;
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (gameOver)
        {
            return;
        }

        // Update game timer
        gameTimer += Time.deltaTime;
        
        // Check if it's time to activate sudden death mode
        if (!suddenDeathActive && gameTimer >= suddenDeathStartTime)
        {
            ActivateSuddenDeath();
        }
        
        // Handle sudden death progression
        if (suddenDeathActive)
        {
            suddenDeathTimer += Time.deltaTime;
            
            // Periodically increase spawn rate during sudden death
            if (suddenDeathTimer >= suddenDeathSpeedupInterval)
            {
                IncreaseLaserSpawnRate();
                suddenDeathTimer = 0f;
            }
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnLaser();
            SetNextSpawnTime();
        }
    }

    private void ActivateSuddenDeath()
    {
        suddenDeathActive = true;
        suddenDeathTimer = 0f;
        Debug.Log("SUDDEN DEATH MODE ACTIVATED!");
        
        // You could add visual effects or sounds here to indicate sudden death mode
    }
    
    private void IncreaseLaserSpawnRate()
    {
        // Decrease the spawn intervals to make lasers spawn faster
        minSpawnInterval *= suddenDeathSpeedupRate;
        maxSpawnInterval *= suddenDeathSpeedupRate;
        
        // Set minimum limits to prevent too rapid spawning that might break the game
        minSpawnInterval = Mathf.Max(minSpawnInterval, 0.05f);
        maxSpawnInterval = Mathf.Max(maxSpawnInterval, 0.1f);
        
        Debug.Log($"Laser spawn rate increased! Interval: {minSpawnInterval:F2}s - {maxSpawnInterval:F2}s");
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

    public bool IsAlive(ulong clientId)
    {
        return alive[clientId];
    }

    public void KillPlayer(ulong clientId)
    {
        alive[clientId] = false;
        leaderboard.Add(clientId);
        ShowDeathUIRpc(clientId);

        if (alive.ContainsValue(true) == false)
        {
            EndGame();
        }
    }

    [Rpc(SendTo.NotServer)]
    private void ShowDeathUIRpc(ulong deadClientId)
    {
        // Check if this is the client that died
        if (NetworkManager.Singleton.LocalClientId == deadClientId)
        {
            deathUI.SetActive(true);
        }
    }

    private void EndGame()
    {
        leaderboard.Reverse();
        Debug.Log(leaderboard);
        endUI.SetActive(true);
        gameUI.SetActive(false);
        gameOver = true;
        // Send Data to GameManager
    }
}
