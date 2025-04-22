using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class OurNetwork : NetworkBehaviour
{
   
    private Lobby currentLobby;
    public bool isHost = false;
    private MenuManager menuManager;

    

    // Mapping from authentication to player index
    public NetworkList<PlayerInfo> playerInfoList = new NetworkList<PlayerInfo>();
    // Mapping from authentication to netcode id
    public Dictionary<string, ulong> playerIdToClientIdMap = new Dictionary<string, ulong>();


    public override void OnNetworkSpawn() {
        if (IsClient) {
            playerInfoList.OnListChanged += OnPlayerInfoListChanged;
        }
    }

    private void OnPlayerInfoListChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        playerInfoList.OnListChanged += change => {
            Debug.Log($"[Client] ChangeType: {change.Type}, Player: {change.Value.playerName}, Color: {change.Value.playerColor.colorRGB}");
        };
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

    /**
    Server RPCs live in this section. If you call these from the server nothing will happen, so call them from the clients!
    **/

    [Rpc(SendTo.Server)]
    public void UpdatePlayerNameRpc(int clientId, string name){
        PlayerInfo updatedInfo = playerInfoList[clientId];
        updatedInfo.playerName = name;
        playerInfoList[clientId] = updatedInfo;
        menuManager = FindObjectOfType<MenuManager>();
        if(menuManager != null){
            Debug.Log("Setting Client ID " + clientId + "To name: "+name);
            menuManager.playerEntries[clientId - 1].SetNameAndColor(name, playerInfoList[clientId].playerColor);
        }
        
    }

    [Rpc(SendTo.Server)]
    public void IncrementPlayerScoreRpc(int clientId){
        PlayerInfo updatedInfo = playerInfoList[clientId];
        updatedInfo.treasuresCollected++;
        playerInfoList[clientId] = updatedInfo;
    }

    [Rpc(SendTo.Server)]
    public void SetPlayerScoreRpc(int clientId, int scoreToSet){
        PlayerInfo updatedInfo = playerInfoList[clientId];
        updatedInfo.treasuresCollected = scoreToSet;
        playerInfoList[clientId] = updatedInfo;
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
