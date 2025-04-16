using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LaserUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;
    [SerializeField] private GameObject scoresParent;
    private OurNetwork ourNetwork;
    private ulong hostId;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            this.gameObject.SetActive(false);
        }

        hostId = NetworkManager.Singleton.LocalClientId;

        ourNetwork = GameManager.Instance.OurNetwork;
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
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId - 1].playerName}: Alive :)";
                } else {
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId - 1].playerName}: Dead :(";
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
