using UnityEngine;
using TMPro;
using Unity.Netcode;

public class WrongLeverMessage : NetworkBehaviour
{
    public GameObject messageCanvas; // Assign in Inspector
    public TextMeshProUGUI messageText; // Assign in Inspector

    private OurNetwork network;

    void Start()
    {
        if (!IsHost) return; // Ensure only the host runs this

        network = FindFirstObjectByType<OurNetwork>();
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }
    }

    public void ShowMessageForClient(int clientIndex)
    {
        if (!IsHost) return; // Only run on the host

        // Validate the clientId
        if (clientIndex >= network.playerInfoList.Count)
        {
            Debug.LogError($"Invalid clientId {clientIndex} for playerInfoList with count {network.playerInfoList.Count}");
            return;
        }

        string playerName = network.playerInfoList[clientIndex].playerName.ToString();
        messageText.text = $"{playerName} pulled the wrong lever!";
        messageCanvas.SetActive(true);
    }
}
