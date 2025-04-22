using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LaserEndUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;
    [SerializeField] private GameObject scoresParent;
    private OurNetwork ourNetwork;
    private ulong hostId;


    private void OnEnable()
    {
        hostId = NetworkManager.Singleton.LocalClientId;
        ourNetwork = GameManager.Instance.OurNetwork;
        UpdateUI();
    }

    private void UpdateUI()
    {
        TMP_Text[] scoreTexts = scoresParent.GetComponentsInChildren<TMP_Text>();
        int playerIndex = 0;


        foreach (ulong clientId in laserManager.leaderboard)
        {
            scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}";
            playerIndex++;
        }
    }
}
