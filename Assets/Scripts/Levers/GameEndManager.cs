using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    private CameraController cameraController;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    private CountdownTimer countdownTimer;

    void Start()
    {
        cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
            return;
        }

        countdownTimer = FindFirstObjectByType<CountdownTimer>();
        if (countdownTimer == null)
        {
            Debug.LogError("CountdownTimer not found in the scene!");
        }
    }

    public void OnGameWin()
    {
        winCanvas.SetActive(true);
        countdownTimer.StopTimer();
        foreach (var player in GameManager.Instance.MapManager.players)
        {
            player.SetRerolls(player.rerolls + 1);
        }
    }

    public void OnGameLose()
    {
        loseCanvas.SetActive(true);
        countdownTimer.StopTimer();
        foreach (var player in GameManager.Instance.MapManager.players)
        {
            player.SetRerolls(player.rerolls - 1);
        }
        GameManager.Instance.MapManager.TimedReturnToMap();
    }

}
