using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClimbingUI : NetworkBehaviour
{
    [SerializeField] private ClimbingManager climbingManager;
    [SerializeField] private GameObject scoresParent;
    private ulong hostId;
    private OurNetwork ourNetwork;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            gameObject.SetActive(false);
        } else {
            hostId = NetworkManager.Singleton.LocalClientId;
        }

        ourNetwork = GameManager.Instance.OurNetwork;
        
    }

    void Start()
    {
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
            if (clientId == hostId)
                continue;
                
            if (playerIndex < scoreTexts.Length)
            {
                float percentage = 100 * climbingManager.GetPlayerHeight(clientId) / climbingManager.GetFinishHeight();
                if (percentage < 100)
                {
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}: {percentage.ToString("F2")}%";
                }
                else {
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}: Finished!";
                    // climbingManager.PlayerFinished(clientId);
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
