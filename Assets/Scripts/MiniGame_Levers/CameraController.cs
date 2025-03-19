using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int playerIndex;
    private string playerId;
    private OurNetwork network;

    public GameObject[] playerCameras; // Array of cameras for each player index

    public GameObject TVCamera;
    private bool isHost;
    void Start()
    {
        for (int i = 0; i < playerCameras.Length; i++){
            playerCameras[i].SetActive(false);
        }
        TVCamera.SetActive(false);

        network = FindFirstObjectByType<OurNetwork>(); // Find the network script
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }


        if (network.isHost == true){
            isHost = true;
            TVCamera.SetActive(true);
        }

        else{
            playerId = network.GetLocalPlayerId();
            playerIndex = network.GetPlayerIndex(playerId);
        
            AssignCamera();
        }
    }
    void AssignCamera()
    {
        if (playerIndex < 0 || playerIndex >= playerCameras.Length)
        {
            Debug.LogError($"Invalid player index {playerIndex}");
            return;
        }

        for (int i = 0; i < playerCameras.Length; i++)
        {
            playerCameras[i].SetActive(i == playerIndex);
        }

        Debug.Log($"Camera assigned for player index {playerIndex}");
    }
}
