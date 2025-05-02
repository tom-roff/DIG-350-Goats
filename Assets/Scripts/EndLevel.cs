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
        Invoke("UpdateUI", 0.1f);
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
            Debug.Log($"Player: {clientId}, place: {playerIndex + 1}");
            scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}";
            playerIndex++;
        }
    }

    private void BackToMap()
    {
        if (!IsServer)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }
}
