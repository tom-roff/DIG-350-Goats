using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class VibrationManager : NetworkBehaviour
{
    // Method called by OurNetwork to trigger vibration for a specific client
    private OurNetwork network;
    public void TriggerVibration(ulong clientId)
    {
        if (!IsServer)
        {
            Debug.LogWarning("TriggerVibration called on a client, but should only be run on the server.");
            return;
        }
        else if (IsServer)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            
            VibratePhoneClientRpc(clientRpcParams);
        }
    }
    
    [ClientRpc]
    private void VibratePhoneClientRpc(ClientRpcParams clientRpcParams = default)
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            Debug.Log("Vibrating phone...");
        #else
            Debug.Log("Vibration not supported on this platform");
        #endif
    }

    public void SendVibrationSignal(int playerToVibrate)
    {
        foreach (var player in network.playerIndexMap)
        {
            string playerID = player.Key;
            int playerIndex = player.Value;

            if (playerIndex == playerToVibrate)
            {
                // Find the targeted client's network ID
                ulong targetClientId = GetClientIdByPlayerId(playerID);
                if (targetClientId == 0)
                {
                    Debug.LogError($"Invalid client ID for player {playerID}. Vibration not sent.");
                    return;
                }
                else if (targetClientId != 0)
                {
                    ClientRpcParams clientRpcParams = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { targetClientId }
                        }
                    };

                    VibratePhoneClientRpc(clientRpcParams);
                }
            }
        }
    }

    private ulong GetClientIdByPlayerId(string pID)
    {
        Dictionary<string, ulong> IdMap = network.playerIdToClientIdMap;

        if(!IdMap.ContainsKey(pID))
        {
            Debug.Log($"Player ID {pID} not found in IdMap.");
            return 0;
        }
        else
        {
            return IdMap[pID];
        }
    }
}
