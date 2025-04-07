using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClimbingUI : NetworkBehaviour
{
    [SerializeField] private ClimbingManager climbingManager;
    [SerializeField] private GameObject scoresParent;
    private ulong hostId;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            this.gameObject.SetActive(false);
        }

        hostId = NetworkManager.Singleton.LocalClientId;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        TMP_Text[] scoreTexts = scoresParent.GetComponentsInChildren<TMP_Text>();
        int playerIndex = 0;
        
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                continue;
                
            if (playerIndex < scoreTexts.Length)
            {
                scoreTexts[playerIndex].text = $"Player {clientId}: {climbingManager.GetPlayerHeight(clientId) * 4}%";
                playerIndex++;
            }
        }
        
        for (int i = playerIndex; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].gameObject.SetActive(false);
        }
    }
}
