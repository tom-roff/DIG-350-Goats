using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class GameStartManager : NetworkBehaviour
{
    public GameObject readyUpScreen;
    public GameObject readyButton;
    public GameObject hostUI;
    public GameObject player;
    private int readyCount = 0;
    private int playerCount = 0;
    [SerializeField] public Material[] colorMaterials = new Material[6];


    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (IsServer)
        {
            readyButton.SetActive(false);
            hostUI.SetActive(true);
        }
        StartTutorial();
    }

    void StartTutorial()
    {
        playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count - 1;

        if (!IsServer)
        {
            readyUpScreen.SetActive(true);
            readyButton.SetActive(true);

            // Hook up the button click listener
            readyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SendReady();  // Call local function
                readyButton.SetActive(false);  // Disable button after clicking
            });
        }
        if (IsServer)
        {
            readyButton.SetActive(false);
            hostUI.SetActive(true);
        }
    }
    public void SendReady()
    {
        if (!IsServer)
        {
            SendReadyRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void SendReadyRpc()
    {
        readyCount++;

        if (readyCount == playerCount && playerCount > 0)
        {
            SpawnPlayersRpc();
            readyUpScreen.SetActive(false);
            HideReadyUpScreenRpc();     
        }
    }

    [Rpc(SendTo.NotServer)]
    private void HideReadyUpScreenRpc()
    {
        readyUpScreen.SetActive(false);
    }

    [Rpc(SendTo.NotServer)]
    private void SpawnPlayersRpc()
    {
        player.GetComponent<Renderer>().material = colorMaterials[(int)NetworkManager.LocalClientId - 1];
        player.SetActive(true);
    }
}
