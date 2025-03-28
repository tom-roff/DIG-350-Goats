using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;


public class LobbyManager : MonoBehaviour
{
    public bool isHost = false;
    private MenuManager menuManager;
    private OurNetwork ourNetwork;
    private string joinCode;
    private int maxPlayers = 8;
    private int currentPlayerCount = 0;
    
    // Dictionary to track connected players
    private Dictionary<string, int> playerIndexMap = new Dictionary<string, int>();

    public async void Initialize(MenuManager manager, OurNetwork network)
    {
        menuManager = manager;
        ourNetwork = network;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void HostGame()
    {
        try {
            // Create a Relay allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            
            // Get the join code
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            // Set up the Relay connection data on the NetworkManager
            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            // Start the host
            NetworkManager.Singleton.StartHost();
            
            // Update UI
            menuManager.UpdateJoinCodeDisplay(joinCode);
            Debug.Log($"Relay allocation created with join code: {joinCode}");
            
            isHost = true;
            UpdatePlayerCount();
            menuManager.ShowStartButton(true);
            
            // Set up connection event handlers
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to start host: {e.Message}");
        }
    }

    public async void JoinGame(string code)
    {
        try {
            // Join the Relay allocation using the provided join code
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            
            // Set up the Relay connection data on the NetworkManager
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            // Start the client
            NetworkManager.Singleton.StartClient();
            
            Debug.Log($"Joined game with code: {code}");
            joinCode = code;
            isHost = false;
            
            // Set up connection event handlers
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to join game: {e.Message}");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // A new client connected
        currentPlayerCount++;
        UpdatePlayerCount();
        
        // You'll need to implement a way to share player IDs and assign indices
        // This could be done with RPCs after connection
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // A client disconnected
        currentPlayerCount--;
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        menuManager.UpdatePlayerCountDisplay(currentPlayerCount, maxPlayers);
    }

    public void StartGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("LaserMinigame", LoadSceneMode.Single);
        }
    }

    // Public method to get PlayerIndex - you'll need to implement a system to track this
    public int GetPlayerIndex(string playerId)
    {
        return playerIndexMap.ContainsKey(playerId) ? playerIndexMap[playerId] : -1;
    }

    // Public method to get local player's ID
    public string GetLocalPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}
