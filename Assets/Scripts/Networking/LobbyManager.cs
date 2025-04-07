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
using Unity.Services.Lobbies.Models;


// No longer using Unity Lobbies, just Unity Relay
public class LobbyManager : MonoBehaviour
{
    public bool isHost = false;
    private MenuManager menuManager;
    private OurNetwork ourNetwork;
    private string joinCode;
    private int maxPlayers = 8;
    private int currentPlayerCount = 0;
    
    public PlayerColor[] colorsArray = new PlayerColor[] {PlayerColor.DarkBlue, PlayerColor.DarkGreen, PlayerColor.Fuchsia, PlayerColor.Gold, PlayerColor.LightBlue, PlayerColor.LightPink, PlayerColor.Lime, PlayerColor.Red};

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

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

        // Add the connected player to our playerIndexMap in the ourNetwork script
        ourNetwork.playerIndexMap.Add(clientId, new PlayerInfo(ourNetwork.playerIndexMap.Count, "Name Placeholder", colorsArray[currentPlayerCount - 1], 0));
        
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
            NetworkManager.Singleton.SceneManager.LoadScene("ClimbingMinigame", LoadSceneMode.Single);
        }
    }
    public void StartLeverGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("LeversMinigame", LoadSceneMode.Single);
        }
    }


    // Public method to get local player's ID
    public string GetLocalPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}
