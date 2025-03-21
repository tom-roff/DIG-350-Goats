using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OurNetwork : MonoBehaviour
{
    private Lobby currentLobby;
    public bool isHost = false;
    private MenuManager menuManager;

    // Mapping from authentication to player index
    private Dictionary<string, int> playerIndexMap = new Dictionary<string, int>();
    // Mapping from authentication to netcode id
    private Dictionary<string, ulong> playerIdToClientIdMap = new Dictionary<string, ulong>();

    public async void Initialize(MenuManager manager)
    {
        menuManager = manager;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void SendVibrationSignal(int playerToVibrate)
    {
        // foreach (var player in playerIndexMap)
        // {
        //     string playerID = player.Key;
        //     int playerIndex = player.Value;

        //     if (playerIndex == playerToVibrate)
        //     {
        //         // Find the targeted client's network ID
        //         ulong targetClientId = GetClientIdByPlayerId(playerID);
        //         if (targetClientId != 0)
        //         {
        //             ClientRpcParams clientRpcParams = new ClientRpcParams
        //             {
        //                 Send = new ClientRpcSendParams
        //                 {
        //                     TargetClientIds = new ulong[] { targetClientId }
        //                 }
        //             };

        //             RpcVibratePhoneClientRpc(clientRpcParams);
        //         }
        //     }
        // }
    }

    private ulong GetClientIdByPlayerId(string playerId)
    {
        return playerIdToClientIdMap[playerId];
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
