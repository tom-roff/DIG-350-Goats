using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LaserUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;
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
                if (laserManager.IsAlive(clientId))
                {
                    scoreTexts[playerIndex].text = $"Player {clientId}: Alive :)";
                } else {
                    scoreTexts[playerIndex].text = $"Player {clientId}: Dead :(";
                }
                playerIndex++;
            }
        }
        
        for (int i = playerIndex; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].gameObject.SetActive(false);
        }
    }
}
