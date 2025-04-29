using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndLevel : NetworkBehaviour
{
    [SerializeField] private GameObject scoresParent;
    private OurNetwork ourNetwork;
    private ulong hostId;
    public List<ulong> leaderboard = new List<ulong>();


    private void OnEnable()
    {
        hostId = NetworkManager.Singleton.LocalClientId;
        ourNetwork = GameManager.Instance.OurNetwork;
        UpdateUI();
        Invoke("BackToMap", 10f);
    }

    private void UpdateUI()
    {
        TMP_Text[] scoreTexts = scoresParent.GetComponentsInChildren<TMP_Text>();
        int playerIndex = 0;

        foreach (ulong clientId in leaderboard)
        {
            if (clientId == hostId)
            {
                continue;
            }
            scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}";
            playerIndex++;
        }
    }

    private void BackToMap()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }
}
