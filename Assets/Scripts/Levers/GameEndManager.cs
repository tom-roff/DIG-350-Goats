using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    private CameraController cameraController;

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
        
    }

    public void OnGameLose()
    {
        
    }

}
