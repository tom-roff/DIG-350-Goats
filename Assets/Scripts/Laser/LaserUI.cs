using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LaserUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;
    [SerializeField] private GameObject scoresParent;
    [SerializeField] private UnityEngine.UI.Image[] colorTexts;
    private OurNetwork ourNetwork;
    private ulong hostId;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            gameObject.SetActive(false);
        }

        hostId = NetworkManager.Singleton.LocalClientId;

        ourNetwork = GameManager.Instance.OurNetwork;

        SetColors();
    }

    private void Start()
    {
        ourNetwork = GameManager.Instance.OurNetwork;
    }

    void SetColors(){
        

        for(int i = 1; i < GameManager.Instance.OurNetwork.playerInfoList.Count; i++){
            colorTexts[i-1].gameObject.SetActive(true);
            colorTexts[i-1].color = ourNetwork.playerInfoList[i].playerColor.colorRGB;
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // so to run scene (and drag laser prefabs in to test sound)
        if (NetworkManager.Singleton == null) return;
     
        TMP_Text[] scoreTexts = scoresParent.GetComponentsInChildren<TMP_Text>();
        int playerIndex = 0;
        
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId == hostId)
                continue;
                
            if (playerIndex < scoreTexts.Length)
            {
                if (laserManager.IsAlive(clientId))
                {
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}: Alive :)";
                } else {
                    scoreTexts[playerIndex].text = $"{ourNetwork.playerInfoList[(int)clientId].playerName}: Dead :(";
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
