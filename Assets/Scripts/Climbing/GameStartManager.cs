using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class GameStartManager : NetworkBehaviour
{
    public GameObject readyUpScreen;
    public GameObject readyButton;
    private int readyCount = 0;
    private int playerCount = 0;


    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
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
            readyUpScreen.SetActive(false);
            HideReadyUpScreenClientRpc();     
        }
    }

    [ClientRpc]
    private void HideReadyUpScreenClientRpc()
    {
        readyUpScreen.SetActive(false);
    }
}
