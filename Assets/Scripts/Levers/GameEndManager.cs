using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;

public class GameEndManager : NetworkBehaviour
{
    private CameraController cameraController;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    [SerializeField] private CountdownTimer countdownTimer;
    private ulong playerId;
    private OurNetwork network;

    void Start()
    {
        network = GameManager.Instance.OurNetwork;
        playerId = NetworkManager.Singleton.LocalClientId;
    }

    public override void OnNetworkSpawn()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
            return;
        }
    }

    public void OnGameWin()
    {
        winCanvas.SetActive(true);
        countdownTimer.StopTimer();

        if (GameManager.Instance?.MapManager?.players != null)
        {
            foreach (var player in GameManager.Instance.MapManager.players)
            {
                player.SetRerolls(player.rerolls + 1);
                
            }
        }
        else
        {
            Debug.LogWarning("Players list not ready on win.");
        }

        if(playerId != 0){
            try
            {
                network.IncrementPlayerScoreRpc((int)playerId);
            }
            catch(Exception e)
            {
                Debug.Log("An error occurred: " + e.Message);
            }

        }
        

        GameManager.Instance.MapManager.TimedReturnToMap();
    }

    public void OnGameLose()
    {
        loseCanvas.SetActive(true);
        countdownTimer.StopTimer();

        if (GameManager.Instance?.MapManager?.players != null)
        {
            foreach (var player in GameManager.Instance.MapManager.players)
            {
                if(player.rerolls > 0)
                {
                    player.SetRerolls(player.rerolls - 1);
                }
            }
        }
        else
        {
            Debug.LogWarning("Players list not ready on loss.");
        }

        GameManager.Instance.MapManager.TimedReturnToMap();
    }

}
