using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class OurNetwork : NetworkBehaviour
{
    private Lobby currentLobby;
    public bool isHost = false;
    private MenuManager menuManager;

    

    // Mapping from authentication to player index
    public Dictionary<ulong, PlayerInfo> playerIndexMap = new Dictionary<ulong, PlayerInfo>();
    // Mapping from authentication to netcode id
    public Dictionary<string, ulong> playerIdToClientIdMap = new Dictionary<string, ulong>();



    public async void Initialize(MenuManager manager)
    {
        menuManager = manager;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    

    private ulong GetClientIdByPlayerId(string playerId)
    {
        if (playerIdToClientIdMap.TryGetValue(playerId, out ulong clientId))
        {
            return clientId;
        }
        else
        {
            Debug.LogError($"Client ID not found for playerId: {playerId}");
            return 0;
        }
    }
    // [Unity.Netcode.ClientRpc]
    // public void RpcVibratePhoneClientRpc(ClientRpcParams clientRpcParams)
    // {
    // #if UNITY_ANDROID || UNITY_IOS
    //     Handheld.Vibrate();
    //     Debug.Log("Vibrating phone...");
    // #endif
    // }
}
