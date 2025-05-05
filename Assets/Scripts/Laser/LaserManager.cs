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
    private float zPosition = 25f;
    private float yPosition = 2f;
    private float minX = -5f;
    private float maxX = 5f;
    private int spawnInterval = 3;
    private float nextSpawnTime;
    
    // Laser set tracking variables
    private int currentLaserCount = 1;
    private int setsSpawnedAtCurrentCount = 0;

    // Game timing and state
    private ulong hostId;
    private float gameTimer = 0f;

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
        
        // Initialize game state
        gameTimer = 0f;
        
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
        gameTimer += Time.deltaTime;
        HandleLaserSpawning();
    }

    private void HandleLaserSpawning()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnLaserSet();
            SetNextSpawnTime();
        }
    }

    private void SpawnLaserSet()
    {
        for (int i = 0; i < currentLaserCount; i++)
        {
            float randomX = Random.Range(minX, maxX);
            Vector3 spawnPosition = new Vector3(randomX, yPosition, zPosition);

            GameObject laser = Instantiate(laserPrefab, spawnPosition, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            NetworkObject networkObject = laser.GetComponent<NetworkObject>();
            networkObject.Spawn();
        }

        setsSpawnedAtCurrentCount++;
        
        if (setsSpawnedAtCurrentCount >= 2)
        {
            currentLaserCount++;
            setsSpawnedAtCurrentCount = 0;
        }
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + spawnInterval;
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
