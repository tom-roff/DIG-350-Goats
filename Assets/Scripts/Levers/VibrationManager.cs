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
    public GameObject mobileCheck;
    public GameObject computerCheck;


    void Start()
    {
        network = GameManager.Instance.OurNetwork;
        

        mobileCheck.SetActive(false);
        computerCheck.SetActive(false);
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
            
            Debug.Log($"About to call the vibration rpc function {clientId}");
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
        Debug.Log("We arrived to virbatio rpc");
        #if UNITY_ANDROID || UNITY_IOS
            mobileCheck.SetActive(true);
            Debug.Log("Mobile check set active");
            Handheld.Vibrate();
            Debug.Log("Vibrating phone...");
        #else
            computerCheck.SetActive(true);
            Debug.Log("Computer check set active");
            Debug.Log("Vibration not supported on this platform");
        #endif

    }

    public IEnumerator StartVibrationSequence(List<int> leverOrder)
    {
        Debug.Log("Lever Order: " + string.Join(", ", leverOrder));
        foreach (int playerNum in leverOrder)
        {
            if (playerNum == 0)
            {
                Debug.Log("Skipped the host for vibration");
                continue;
            }

            // Vibrate the correct player's phone
            Debug.Log(playerNum);
            TriggerVibration((ulong)playerNum);

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
