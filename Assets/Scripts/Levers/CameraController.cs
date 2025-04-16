using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CameraController : NetworkBehaviour
{
    private OurNetwork network;
    private VibrationManager vibration;
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

        playerId = NetworkManager.Singleton.LocalClientId;

        AssignCamera();

        StartVibrationSequence();
    }

    void AssignLeverOrder() 
    {
        // Just as an example – this assumes Start() runs after Awake()
        int numPlayers = network.playerInfoList.Count;
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
            if (currentLeverIndex >= leverOrder.Length)
            {
                Debug.Log("All levers pulled correctly! Game complete.");
                // Trigger win condition
                OnGameWin();
            }
        }

        else
        {
            Debug.Log($"Wrong Lever, {currentLeverIndex} should have been pulled!");
            OnWrongLeverPulled();
        }
    }

    void OnGameWin()
    {
        Debug.Log("Congratulations! All levers were pulled in the correct order.");
        // game end logic
    }

    void OnWrongLeverPulled()
    {
        Debug.Log("Incorrect lever pulled! Player " + currentLeverIndex + " pulled the wrong lever");
        var wrongLeverMessage = FindFirstObjectByType<WrongLeverMessage>();
        wrongLeverMessage.ShowMessageForClient(currentLeverIndex);   
        // logic to reset or provide retries
    }
}
