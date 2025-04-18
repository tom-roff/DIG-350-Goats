using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class VibrationManager : NetworkBehaviour
{
    private OurNetwork network;
    public GameObject leverBlocker;

    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>();
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }

        int numPlayers = network.playerInfoList.Count;
    }

    public void TriggerVibration(ulong clientId)
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

    public IEnumerator StartVibrationSequence(List<int> leverOrder)
    {
        foreach (int playerNum in leverOrder)
        {
            if (playerNum == 0)
            {
                Debug.Log("Skipped the host for vibration");
                continue;
            }

            // Vibrate the correct player's phone
            TriggerVibration((ulong)playerNum);

            // Delay between vibrations
            yield return new WaitForSeconds(1.5f); // Adjust delay based on difficulty
        }

        Debug.Log("Vibration sequence complete. Players should now pull levers.");
        leverBlocker.SetActive(false);
    }

}
