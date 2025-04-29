using UnityEngine;
using Unity.Netcode;

public class GameEndManager : NetworkBehaviour
{
    private CameraController cameraController;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    [SerializeField] private CountdownTimer countdownTimer;

    public override void OnNetworkSpawn()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
            return;
        }

        // countdownTimer = FindFirstObjectByType<CountdownTimer>();
        // if (countdownTimer == null)
        // {
        //     Debug.LogError("CountdownTimer not found in the scene!");
        // }
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
                player.SetRerolls(player.rerolls - 1);
            }
        }
        else
        {
            Debug.LogWarning("Players list not ready on loss.");
        }

        GameManager.Instance.MapManager.TimedReturnToMap();
    }

}
