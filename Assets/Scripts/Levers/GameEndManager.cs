using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    private CameraController cameraController;
    public GameObject winCanvas;
    public GameObject loseCanvas;

    void Start()
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
    }

    public void OnGameLose()
    {
        loseCanvas.SetActive(true);
    }

}
