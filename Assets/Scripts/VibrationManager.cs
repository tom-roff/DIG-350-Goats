using Unity.Netcode;
using UnityEngine;

public class VibrationManager : NetworkBehaviour
{
    // Method called by OurNetwork to trigger vibration for a specific client
    public void TriggerVibration(ulong clientId)
    {
        if (IsServer)
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
}
