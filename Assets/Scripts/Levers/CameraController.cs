using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CameraController : NetworkBehaviour
{
    private OurNetwork network;
    private VibrationManager vibration;
    private GameEndManager gameEndManager;
    private ulong playerId;

    public GameObject gameCamera;
    public Transform[] playerCameraPositions;
    public Transform[] hostCameraPositions;

    private int[] leverOrder; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;

    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>(); // Find the network script
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }

        AssignLeverOrder();

        vibration = FindFirstObjectByType<VibrationManager>();
        if (vibration == null)
        {
            Debug.LogError("VibrationManager not found in the scene!");
            return;
        }

        gameEndManager = FindFirstObjectByType<GameEndManager>(); // Find the network script
        if (gameEndManager == null)
        {
            Debug.LogError("GameEndManager instance not found!");
            return;
        }

        playerId = NetworkManager.Singleton.LocalClientId;

        AssignCamera();

        StartVibrationSequence();
    }

    void AssignLeverOrder() 
    {
        int numPlayers = network.playerInfoList.Count - 1;
        leverOrder = new int[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            leverOrder[i] = i;
        }
    }

    void AssignCamera()
    {
        if(playerId == 0) // host camera position
        {
            int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
            Debug.Log(numPlayers);
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

        StartCoroutine(vibration.StartVibrationSequence(new List<int>(leverOrder)));

    }
    

    // Called by the lever when it is pulled correctly
    public void OnLeverPulled(int pulledLeverIndex)
    {
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
        var wrongLeverMessage = FindFirstObjectByType<WrongLeverMessage>();
        wrongLeverMessage.ShowMessageForClient(currentLeverIndex);  
        gameEndManager.OnGameLose(); 
        // logic to reset or provide retries
    }
}
