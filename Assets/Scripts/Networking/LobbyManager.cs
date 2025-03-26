using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyManager : MonoBehaviour
{
    private Lobby currentLobby;
    public bool isHost = false;
    private MenuManager menuManager;
    private OurNetwork ourNetwork;
    
    // Mapping from authentication to player index
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
        var lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = true
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("My Game", 8, lobbyOptions);
        menuManager.UpdateJoinCodeDisplay(currentLobby.LobbyCode);
        Debug.Log($"Lobby created with code: {currentLobby.LobbyCode}");
        SubscribeToLobbyEvents();
        isHost = true;
        UpdatePlayerCount();
        menuManager.ShowStartButton(true);
    }

    public async void JoinGame(string joinCode)
    {
        currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);
        Debug.Log($"Joined lobby: {currentLobby.Name}");
        SubscribeToLobbyEvents();
        isHost = false;
        UpdatePlayerCount();
    }

    private async void SubscribeToLobbyEvents()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        callbacks.PlayerJoined += OnPlayerJoined;
        callbacks.PlayerLeft += OnPlayerLeft;

        await LobbyService.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callbacks);
    }

    private void OnPlayerJoined(List<LobbyPlayerJoined> newPlayers)
    {
        foreach (var player in newPlayers)
        {
            Debug.Log($"Player joined: {player.Player.Id} at index {player.PlayerIndex}");
            playerIndexMap[player.Player.Id] = player.PlayerIndex;
        }
        UpdatePlayerCount();
    }

    private void OnPlayerLeft(List<int> leftPlayerIndices)
    {
        foreach (var playerIndex in leftPlayerIndices)
        {
            Debug.Log($"Player left at index: {playerIndex}");
        }
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        int playerCount = currentLobby.Players.Count - 1;
        int maxPlayers = currentLobby.MaxPlayers;
        menuManager.UpdatePlayerCountDisplay(playerCount, maxPlayers);
    }

    public async void StartGame()
    {
        var updateOptions = new UpdateLobbyOptions();
        updateOptions.Data = new Dictionary<string, DataObject>()
        {
            {"GameStarted", new DataObject(DataObject.VisibilityOptions.Member, "true")}
        };
        await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateOptions);
    }

    private void OnLobbyChanged(ILobbyChanges changes)
    {
        if (changes.Data.Changed)
        {
            if (currentLobby.Data.ContainsKey("GameStarted") && 
                currentLobby.Data["GameStarted"].Value == "true")
            {
                // NetworkManager.Singleton.SceneManager.LoadScene("LaserMinigame", LoadSceneMode.Single);
                SceneManager.LoadScene("LaserMinigame");
            }
        }

        if (changes.PlayerJoined.Changed || changes.PlayerLeft.Changed)
        {
            UpdatePlayerCount();
        }
    }

    // Public method to get PlayerIndex
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
