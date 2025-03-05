using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public Button hostButton;
    public TMP_Text joinCodeText;
    public TMP_InputField joinCodeInput;
    public Button joinButton;
    public TMP_Text playerCountText;
    public Button startGameButton;
    private int playerCount = 0;
    private Lobby currentLobby;
    private bool isHost = false;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
        startGameButton.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
    }

    private async void HostGame()
    {
        var lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = true
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("My Game", 8, lobbyOptions);
        joinCodeText.text = $"Join Code: {currentLobby.LobbyCode}";
        Debug.Log($"Lobby created with code: {currentLobby.LobbyCode}");
        SubscribeToLobbyEvents();
        isHost = true;
        UpdatePlayerCountDisplay();
        startGameButton.gameObject.SetActive(true);
        playerCountText.gameObject.SetActive(true);
    }

    private async void JoinGame()
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCodeInput.text);
            Debug.Log($"Joined lobby: {currentLobby.Name}");
            SubscribeToLobbyEvents();
            isHost = false;
            UpdatePlayerCountDisplay();
            playerCountText.gameObject.SetActive(true);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }
    }

    private async void SubscribeToLobbyEvents()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        callbacks.PlayerJoined += OnPlayerJoined;
        callbacks.PlayerLeft += OnPlayerLeft;

        try
        {
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to subscribe to lobby events: {e.Message}");
        }
    }

    private void OnLobbyChanged(ILobbyChanges changes)
    {
        if (changes.PlayerJoined.Changed || changes.PlayerLeft.Changed)
        {
            UpdatePlayerCountDisplay();
        }
    }

    private void OnPlayerJoined(List<LobbyPlayerJoined> newPlayers)
    {
        foreach (var player in newPlayers)
        {
            Debug.Log($"Player joined: {player.Player.Id} at index {player.PlayerIndex}");
        }
        UpdatePlayerCountDisplay();
    }

    private void OnPlayerLeft(List<int> leftPlayerIndices)
    {
        foreach (var playerIndex in leftPlayerIndices)
        {
            Debug.Log($"Player left at index: {playerIndex}");
        }
        UpdatePlayerCountDisplay();
    }

    private void UpdatePlayerCountDisplay()
    {
        playerCount = currentLobby.Players.Count;
        int maxPlayers = isHost ? currentLobby.MaxPlayers - 1 : currentLobby.MaxPlayers;
        playerCountText.text = $"Players: {playerCount}/{maxPlayers}";
    }
}
