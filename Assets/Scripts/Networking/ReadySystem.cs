using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class ReadySystem : NetworkBehaviour
{
    // UI Elements
    [SerializeField] private GameObject directionsUI;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private PlayerSpawner playerSpawner;
    
    // Events that minigames can subscribe to
    public event Action OnAllPlayersReady;
    
    // Player readiness tracking
    private Dictionary<ulong, bool> playersReady = new Dictionary<ulong, bool>();
    private bool gameStarted = false;
    
    public override void OnNetworkSpawn()
    {
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
        // Register all currently connected players
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            RegisterPlayer(clientId);
        }

        if (directionsUI != null)
            directionsUI.SetActive(true);
            readyButton.SetActive(false);
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
    
    private void InitializeClient()
    {
        if (directionsUI != null)
            directionsUI.SetActive(true);
    }
    
    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            RegisterPlayer(clientId);
        }
    }
    
    private void RegisterPlayer(ulong clientId)
    {
        if (clientId == 0)
        {
            playersReady[clientId] = true;
        }
        else {
            playersReady[clientId] = false;
        }
    }
    
    private void Update()
    {
        if (!IsServer || gameStarted) return;
        
        CheckAllPlayersReady();
    }
    
    // Called by UI button
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
        if (playersReady.Count == 0) return;
        
        foreach (var kvp in playersReady)
        {
            if (!kvp.Value) return;
        }
        
        // All players are ready
        playerSpawner.SpawnPlayers();
        StartGame();
    }
    
    private void StartGame()
    {
        gameStarted = true;
        
        if (directionsUI != null)
            directionsUI.SetActive(false);
        
        StartGameRpc();
        
        // Notify any listeners that all players are ready
        OnAllPlayersReady?.Invoke();
    }
    
    [Rpc(SendTo.NotServer)]
    private void StartGameRpc()
    {
        if (IsServer) return;
        
        if (directionsUI != null)
            directionsUI.SetActive(false);
    }
    
    // Public API
    public bool IsGameStarted() => gameStarted;
    public GameObject GetDirectionsUI() => directionsUI;
}
