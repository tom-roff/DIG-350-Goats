using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    private Lobby currentLobby;
    private bool isHost = false;
    private MenuManager menuManager;

    public async void Initialize(MenuManager manager)
    {
        menuManager = manager;

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
                SceneManager.LoadScene("LaserMinigame");
            }
        }

        if (changes.PlayerJoined.Changed || changes.PlayerLeft.Changed)
        {
            UpdatePlayerCount();
        }
    }
}
