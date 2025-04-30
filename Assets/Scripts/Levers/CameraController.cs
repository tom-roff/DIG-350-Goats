using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CameraController : NetworkBehaviour
{
    public OurNetwork network;
    public VibrationManager vibration;
    public GameEndManager gameEndManager;
    private ulong playerId;

    public GameObject gameCamera;
    public Transform[] playerCameraPositions;
    public Transform[] hostCameraPositions;

    private int[] leverOrder; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;

    void Start()
    {
        network = GameManager.Instance.OurNetwork;

        AssignLeverOrder();

        playerId = NetworkManager.Singleton.LocalClientId;

        AssignCamera();

        StartVibrationSequence();
    }

    void AssignLeverOrder() 
    {
        int numPlayers = network.playerInfoList.Count - 1;
        leverOrder = new int[numPlayers];
        for (int i = 1; i <= numPlayers; i++)
        {
            Debug.Log("Current Player = " + i.ToString());
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
        if (!IsServer) return;
        Debug.Log("Lever Order pre VibrationSeqence Function Call: " + string.Join(", ", leverOrder));
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
        gameEndManager.OnGameLose(); 
    }
}
