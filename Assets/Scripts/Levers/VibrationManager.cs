using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class VibrationManager : NetworkBehaviour
{
    private OurNetwork network;
    public CountdownTimer timer;

    public GameObject introText;


    void Start()
    {
        network = GameManager.Instance.OurNetwork;
    }

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

            // Vibrate the correct player's phone
            Debug.Log(playerNum);
                    // Vibrate the correct player's phone (i + 1) times
            for (int j = 0; j < i + 1; j++)
            {
                Debug.Log($"{playerNum} will vibrate {i} times");
                TriggerVibration((ulong)playerNum);
                yield return new WaitForSeconds(0.75f); // Shorter delay between multi-vibrations
            }

            // Delay between vibrations
            yield return new WaitForSeconds(1.5f); // Adjust delay based on difficulty
        }

        Debug.Log("Vibration sequence complete. Players should now pull levers.");
        introText.SetActive(false);

        if (timer != null)
        {
            timer.StartTimer(60f);
        }
        else
        {
            Debug.LogWarning("Timer not assigned in VibrationManager.");
        }
    }

}
