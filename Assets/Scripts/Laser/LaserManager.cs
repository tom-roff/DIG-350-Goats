using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class LaserManager : NetworkBehaviour
{
    // UI Elements
    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject directionsUI;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private EndLevel endLevel;

    // Player and game state data
    public List<ulong> leaderboard = new List<ulong>();
    public Dictionary<ulong, int> scores = new Dictionary<ulong, int>();
    public Dictionary<ulong, bool> alive = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playersReady = new Dictionary<ulong, bool>();

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
    private bool gameActive = false;
    private float gameTimer = 0f;
    private bool suddenDeathActive = false;
    public float suddenDeathStartTime = 20f;
    public float suddenDeathSpeedupInterval = 1.5f;
    private float suddenDeathTimer = 0f;
    public float suddenDeathSpeedupRate = 0.9f;

    public override void OnNetworkSpawn()
    {
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

        gameTimer = 0f;
        suddenDeathTimer = 0f;
        SetNextSpawnTime();

        hostUI.SetActive(false);
        directionsUI.SetActive(false);
        endUI.SetActive(false);
        deathUI.SetActive(false);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void InitializeClient()
    {
        directionsUI.SetActive(true);
        hostUI.SetActive(false);
        endUI.SetActive(false);
        deathUI.SetActive(false);
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
        playersReady[clientId] = false;
    }

    private void Update()
    {
        if (!gameActive)
        {
            CheckAllPlayersReady();
            return;
        }

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

    public void PlayerReady()
    {
        PlayerReadyRpc(NetworkManager.Singleton.LocalClientId);
        readyButton.SetActive(false);
    }

    [Rpc(SendTo.Server)]
    private void PlayerReadyRpc(ulong clientId)
    {
        if (playersReady.ContainsKey(clientId))
        {
            playersReady[clientId] = true;
        }
    }

    private void CheckAllPlayersReady()
    {
        bool allReady = true;
        foreach (var kvp in playersReady)
        {
            if (!kvp.Value)
            {
                allReady = false;
                break;
            }
        }

        if (allReady && playersReady.Count > 0)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        gameActive = true;
        gameTimer = 0f;

        if (directionsUI != null)
            directionsUI.SetActive(false);
        if (hostUI != null)
            hostUI.SetActive(true);

        StartGameRpc();
    }

    [Rpc(SendTo.NotServer)]
    private void StartGameRpc()
    {
        if (IsServer) return;

        if (directionsUI != null)
            directionsUI.SetActive(false);
        if (hostUI != null)
            hostUI.SetActive(true);
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

    private void EndGame()
    {
        leaderboard.Reverse();
        endLevel.leaderboard = leaderboard;
        endUI.SetActive(true);
        hostUI.SetActive(false);
        gameActive = false;
    }
}
