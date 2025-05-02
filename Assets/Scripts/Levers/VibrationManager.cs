using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class VibrationManager : NetworkBehaviour
{
    public CountdownTimer timer;

    public void TriggerVibration(ulong clientId)
    {
        try
        {
            if (!IsServer)
            {
                Debug.LogWarning("TriggerVibration called on a client, but should only be run on the server.");
                return;
            }

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            
            VibratePhoneClientRpc(clientRpcParams);  
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    [ClientRpc(RequireOwnership = false)]
    private void VibratePhoneClientRpc(ClientRpcParams clientRpcParams = default)
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            Debug.Log("Vibrating phone...");
        #else
            Debug.Log("Vibration not supported on this platform");
        #endif

    }

    public IEnumerator StartVibrationSequence(List<int> leverOrder)
    {
        for (int i = 0; i < leverOrder.Count; i++)
        {
            int playerNum = leverOrder[i];

            if (playerNum == 0)
            {
                Debug.Log("Skipped the host for vibration");
                continue;
            }

            // Start a coroutine for each player to vibrate (i + 1) times
            StartCoroutine(VibratePlayerMultipleTimes((ulong)playerNum, i + 1));
        }

        // Wait long enough to ensure all vibrations are likely complete
        float totalWaitTime = leverOrder.Count * 0.75f + 1.5f;
        yield return new WaitForSeconds(totalWaitTime);

        Debug.Log("Vibration sequence complete. Players should now pull levers.");

        if (timer != null)
        {
            timer.StartTimer(60f);
        }
        else
        {
            Debug.LogWarning("Timer not assigned in VibrationManager.");
        }
    }

    private IEnumerator VibratePlayerMultipleTimes(ulong clientId, int vibrationCount)
    {
        Debug.Log($"{clientId} will vibrate {vibrationCount} times");

        for (int j = 0; j < vibrationCount; j++)
        {
            TriggerVibration(clientId);
            yield return new WaitForSeconds(0.75f);
        }
    }


}
