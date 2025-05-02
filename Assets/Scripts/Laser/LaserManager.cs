using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class LaserManager : ManagerBase
{
    // UI Elements
    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private EndLevel endLevel;

    // Player and game state data
    public List<ulong> leaderboard = new List<ulong>();
    public Dictionary<ulong, int> scores = new Dictionary<ulong, int>();
    public Dictionary<ulong, bool> alive = new Dictionary<ulong, bool>();

    // Laser spawning
    public GameObject laserPrefab;
    private float yPosition = 2f;
    private float zPosition = 10f;
    public float minX = -5f;
    public float maxX = 5f;
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;
    private float nextSpawnTime;

    // Game timing and state
    private ulong hostId;
    private float gameTimer = 0f;
    private bool suddenDeathActive = false;
    public float suddenDeathStartTime = 20f;
    public float suddenDeathSpeedupInterval = 1.5f;
    private float suddenDeathTimer = 0f;
    public float suddenDeathSpeedupRate = 0.9f;

    public override void OnNetworkSpawn()
    {
        // Call base implementation first to set up ReadySystem
        base.OnNetworkSpawn();
        
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        if (IsServer)
        {
            InitializeServer();
        }
        else
        {
            InitializeClient();
        }
    }

    private void InitializeServer()
    {
        hostId = NetworkManager.Singleton.LocalClientId;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != hostId)
            {
                RegisterPlayer(clientId);
            }
        }

        SetNextSpawnTime();
        
        hostUI.SetActive(false);
        deathUI.SetActive(false);
        endUI.SetActive(false);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void InitializeClient()
    {
        hostUI.SetActive(false);
        deathUI.SetActive(false);
        endUI.SetActive(false);
        readySystem.GetDirectionsUI().SetActive(true);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer && clientId != hostId)
        {
            RegisterPlayer(clientId);
        }
    }

    private void RegisterPlayer(ulong clientId)
    {
        scores[clientId] = 0;
        alive[clientId] = true;
    }

    protected override void OnGameStart()
    {
        Debug.Log("On game start called");
        
        // Initialize game state
        gameTimer = 0f;
        suddenDeathTimer = 0f;
        
        if (hostUI != null)
            hostUI.SetActive(true);
    }

    protected override void OnGameEnd()
    {
        leaderboard.Reverse();
        endLevel.leaderboard = leaderboard;
        
        if (endUI != null)
            endUI.SetActive(true);
        if (hostUI != null)
            hostUI.SetActive(false);
    }

    // Replace Update with GameUpdate for game-specific logic
    protected override void GameUpdate()
    {
        UpdateGameTimers();
        HandleLaserSpawning();
    }

    private void UpdateGameTimers()
    {
        gameTimer += Time.deltaTime;

        if (!suddenDeathActive && gameTimer >= suddenDeathStartTime)
        {
            ActivateSuddenDeath();
        }

        if (suddenDeathActive)
        {
            suddenDeathTimer += Time.deltaTime;

            if (suddenDeathTimer >= suddenDeathSpeedupInterval)
            {
                IncreaseLaserSpawnRate();
                suddenDeathTimer = 0f;
            }
        }
    }

    private void HandleLaserSpawning()
    {
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
    }

    private void IncreaseLaserSpawnRate()
    {
        minSpawnInterval *= suddenDeathSpeedupRate;
        maxSpawnInterval *= suddenDeathSpeedupRate;

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
        if (!IsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if ((clientId != hostId) && (alive[clientId] != false))
            {
                scores[clientId]++;
            }
        }
    }

    public int GetScore(ulong clientId) => scores[clientId];

    public bool IsAlive(ulong clientId) => alive[clientId];

    public void KillPlayer(ulong clientId)
    {
        alive[clientId] = false;
        leaderboard.Add(clientId);
        ShowDeathUIRpc(clientId);

        if (!alive.ContainsValue(true))
        {
            EndGame();
        }
    }

    [Rpc(SendTo.NotServer)]
    private void ShowDeathUIRpc(ulong deadClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == deadClientId)
        {
            deathUI.SetActive(true);
        }
    }

    // Changed to protected to explicitly match the base class method
    protected override void EndGame()
    {
        base.EndGame();
    }
}
