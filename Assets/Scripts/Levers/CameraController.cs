using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class CameraController : NetworkBehaviour
{
    private OurNetwork network;
    private VibrationManager vibration;
    private int playerIndex;

    public GameObject gameCamera;
    public Transform[] playerCameraPositions;

    public int[] leverOrder; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;

    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>(); // Find the network script
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }

        ulong playerId = NetworkManager.Singleton.LocalClientId;
        playerIndex = network.playerIndexMap[playerId].playerIndex;
    

        AssignCamera();

        StartVibrationSequence();

        // StartLeverSequence();
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
        int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
        for(int i = 0; i < numPlayers; i++)
        {
            vibration.SendVibrationSignal(i);
        }   
    }
    

    // Called by the lever when it is pulled correctly
    public void OnLeverPulled(int pulledLeverIndex)
    {
        if (pulledLeverIndex == leverOrder[currentLeverIndex])
        {
            Debug.Log($"Lever {pulledLeverIndex} pulled correctly!");
            currentLeverIndex++;

            if (currentLeverIndex < leverOrder.Length)
            {
                // SendVibrationToPlayer(leverOrder[currentLeverIndex]);
            }
            else
            {
                Debug.Log("All levers have been pulled in order!");
            }
        }
        else
        {
            Debug.LogWarning("Incorrect lever pulled! Try again.");
        }
    }
}
