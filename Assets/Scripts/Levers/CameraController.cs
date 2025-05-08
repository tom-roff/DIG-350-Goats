using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CameraController : NetworkBehaviour
{
    private OurNetwork network;
    public VibrationManager vibration;
    public GameEndManager gameEndManager;
    private ulong playerId;

    public GameObject gameCamera;
    public Transform[] playerCameraPositions;
    public Transform[] hostCameraPositions;

    public GameObject readyUpScreen;
    public GameObject readyButton;
    private int readyCount = 0;
    private int playerCount = 0;

    private int[] leverOrder; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;

    // private LightBehavior lightController;

    void Start()
    {
        network = GameManager.Instance.OurNetwork;
        playerId = NetworkManager.Singleton.LocalClientId;
        // lightController = GetComponent<LightBehavior>();

        StartTutorial();
        AssignLeverOrder();
        AssignCamera();
    }

    void StartTutorial()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count - 1;
        Debug.Log($"Player Count = {playerCount}");

        if (!IsServer)
        {
            readyUpScreen.SetActive(true);
            readyButton.SetActive(true);

            // Hook up the button click listener
            readyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("ReadyButton clicked");
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
            StartVibrationSequence();       
        }
    }

    [ClientRpc]
    private void HideReadyUpScreenClientRpc()
    {
        readyUpScreen.SetActive(false);
    }

    void AssignLeverOrder() 
    {
        int numPlayers = network.playerInfoList.Count - 1;
        leverOrder = new int[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            leverOrder[i] = i + 1; // Players start from 1 (since 0 is host)
        }
    }

    void AssignCamera()
    {
        if(playerId == 0) // host camera position
        {
            int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
            Transform desiredHostPos = hostCameraPositions[numPlayers - 2];
            gameCamera.transform.position = desiredHostPos.position;
            return;
        }
        if (playerId < 0 || (int)playerId >= playerCameraPositions.Length)
        {
            Debug.LogError($"Invalid player index {playerId}");
            return;
        }

        Transform desiredPos = playerCameraPositions[playerId - 1]; // needs to be -1 because the host is Id 0
        if (desiredPos != null)
        {
            gameCamera.transform.position = desiredPos.position;
            //gameCamera.transform.rotation = desiredPos.rotation;
            Debug.Log($"Camera set to position {playerId}");
        }
        else
        {
            Debug.LogError($"Camera position for player {playerId} not found.");
        }
    }

    // Sends vibration to the correct player
    void StartVibrationSequence()
    {
        if (!IsServer) return;
        Debug.Log("Lever Order pre VibrationSeqence Function Call: " + string.Join(", ", leverOrder));
        StartCoroutine(vibration.StartVibrationSequence(new List<int>(leverOrder)));
    }
    

    // Called by the lever when it is pulled correctly
    public void OnLeverPulled(int pulledLeverIndex)
    {
        Debug.Log($"Pulled Lever Index = {pulledLeverIndex} and current lever Index = {currentLeverIndex}");
        if (pulledLeverIndex == leverOrder[currentLeverIndex])
        {
            Debug.Log($"Lever {pulledLeverIndex} pulled correctly!");
            currentLeverIndex++;
            Debug.Log($"currentLeverIndex is now {currentLeverIndex}, leverOrder.Length is {leverOrder.Length}");

            if (currentLeverIndex >= leverOrder.Length)
            {
                Debug.Log("All levers pulled correctly! Game complete.");
                // Trigger win condition
                gameEndManager.OnGameWin();
            }
        }

        else
        {
            Debug.Log($"Wrong Lever, {currentLeverIndex} should have been pulled!");
            OnWrongLeverPulled();
        }
    }


    void OnWrongLeverPulled()
    {
        Debug.Log("Incorrect lever pulled! It was Player " + currentLeverIndex + " turn to pull");
        gameEndManager.OnGameLose(); 
    }
}
