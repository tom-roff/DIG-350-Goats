using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class CameraController : NetworkBehaviour
{
    private OurNetwork network;
    private VibrationManager vibration;
    private int playerIndex;

    public GameObject gameCamera;
    public Transform[] playerCameraPositions;

    private int[] leverOrder = {0, 1, 2, 3, 4, 5}; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;

    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>(); // Find the network script
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }

        vibration = FindFirstObjectByType<VibrationManager>();
        if (vibration == null)
        {
            Debug.LogError("VibrationManager not found in the scene!");
            return;
        }

        ulong playerId = NetworkManager.Singleton.LocalClientId;
        playerIndex = network.playerIndexMap[playerId].playerIndex;
    

        AssignCamera();

        StartVibrationSequence();
    }

    void AssignCamera()
    {
        if (playerIndex < 0 || playerIndex >= playerCameraPositions.Length)
        {
            Debug.LogError($"Invalid player index {playerIndex}");
            return;
        }

        Transform desiredPos = playerCameraPositions[playerIndex];
        if (desiredPos != null)
        {
            gameCamera.transform.position = desiredPos.position;
            gameCamera.transform.rotation = desiredPos.rotation;
            Debug.Log($"Camera set to position {playerIndex}");
        }
        else
        {
            Debug.LogError($"Camera position for player {playerIndex} not found.");
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
            Debug.Log($"Wrong Lever, {pulledLeverIndex} should have been pulled!");
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
        Debug.Log("Incorrect lever pulled! Resetting sequence or allow retries.");
        // logic to reset or provide retries
    }
}
