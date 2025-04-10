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
    private int maxPlayers = 6;
    private int currentPlayerCount = 0;

    public List<PlayerColor> possibleColors = new List<PlayerColor>();


    public Material[] colorMats = new Material[6];

    private void Start()
    {
        possibleColors.Add(new PlayerColor(colorEnumerator.DarkBlue, new Color32 (25, 25, 112, 255), colorMats[0]));
        possibleColors.Add(new PlayerColor(colorEnumerator.DarkGreen, new Color32 (0, 100, 0, 255), colorMats[1]));
        possibleColors.Add(new PlayerColor(colorEnumerator.Fuchsia, new Color32 (190, 0, 255, 255), colorMats[2]));
        possibleColors.Add(new PlayerColor(colorEnumerator.Gold, new Color32 (255, 215, 0, 255), colorMats[3]));
        possibleColors.Add(new PlayerColor(colorEnumerator.LightBlue, new Color32 (30, 156, 255, 255), colorMats[4]));
        possibleColors.Add(new PlayerColor(colorEnumerator.Lime, new Color32 (0, 225, 0, 255), colorMats[5]));
    }

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
            menuManager.hostButton.gameObject.SetActive(false);
            
            // Set up connection event handlers
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to start host: {e.Message}");
        }
    }

    public void OnNameInput(string name){
        if(name.Length != 0){
            menuManager.nameText.text = name;
            ourNetwork.UpdatePlayerNameRpc((int)NetworkManager.Singleton.LocalClientId, name);
            menuManager.waitingForHostToStartText.text = "Waiting for host to start game...";
            menuManager.confirmNameButton.gameObject.SetActive(false);
            menuManager.nameInput.gameObject.SetActive(false);
            Debug.Log(ourNetwork.playerInfoList[ourNetwork.playerInfoList.Count - 1]);
        }
        else{
            menuManager.nameText.text = "Please type in a name.";
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

            menuManager.clientStartUI.SetActive(false);
            menuManager.clientJoinedUI.SetActive(true);
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to join game: {e.Message}");
            menuManager.joinText.text = "Connection failed, try again.";
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // A new client connected
        currentPlayerCount++;
        UpdatePlayerCount();

        // Add the connected player to our playerIndexMap in the ourNetwork script
        

        // This function updates the UI when a player joins
        if(isHost){
            ourNetwork.playerInfoList.Add(new PlayerInfo("Player Joining", possibleColors[currentPlayerCount - 1], 0));
            menuManager.playerEntries[ourNetwork.playerInfoList.Count - 1].gameObject.SetActive(true);
            Debug.Log(ourNetwork.playerInfoList[ourNetwork.playerInfoList.Count - 1].playerColor.colorRGB);
            menuManager.playerEntries[ourNetwork.playerInfoList.Count - 1].SetNameAndColor(ourNetwork.playerInfoList[ourNetwork.playerInfoList.Count - 1].playerName.ToString(), ourNetwork.playerInfoList[ourNetwork.playerInfoList.Count - 1].playerColor);
        }
        
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
    public void StartLeverGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("LeversMinigame", LoadSceneMode.Single);
        }
    }
    public void StartLaserGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("LaserMinigame", LoadSceneMode.Single);
        }
    }
    public void StartClimbGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("ClimbingMinigame", LoadSceneMode.Single);
        }
    }
    public void StartHackGame()
    {
        if (isHost)
        {
            // Use NetworkManager to load the scene on all clients
            NetworkManager.Singleton.SceneManager.LoadScene("HackingMinigame", LoadSceneMode.Single);
        }
    }


    // Public method to get local player's ID
    public string GetLocalPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}
