using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int playerIndex;
    private string playerId;
    private OurNetwork network;
    private LobbyManager lobbyManager;
    private VibrationManager vibration;


    public GameObject[] playerCameras; // Array of cameras for each player index

    public GameObject TVCamera;
    private bool isHost;


    public int[] leverOrder; // Array that holds the lever order (0, 1, 2, 3, ...)
    private int currentLeverIndex = 0;
    void Start()
    {
        for (int i = 0; i < playerCameras.Length; i++){
            playerCameras[i].SetActive(false);
        }
        TVCamera.SetActive(false);

        network = FindFirstObjectByType<OurNetwork>(); // Find the network script
        lobbyManager = FindFirstObjectByType<LobbyManager>(); // Find lobby manager
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
            playerId = lobbyManager.GetLocalPlayerId();
            playerIndex = lobbyManager.GetPlayerIndex(playerId);
        
            AssignCamera();
        }

        StartLeverSequence();
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

                // Start listening for vibration if this is the next lever to be pulled
        if (playerIndex == leverOrder[currentLeverIndex])
        {
            VibratePhone();
        }
    }

    // Host starts the sequence
    void StartLeverSequence()
    {
        Debug.Log("Starting lever sequence...");
        currentLeverIndex = 0;
        SendVibrationToPlayer(leverOrder[currentLeverIndex]);
    }

    // Sends vibration to the correct player
    void SendVibrationToPlayer(int playerToVibrate)
    {
        vibration.SendVibrationSignal(playerToVibrate); // Send signal to vibrate
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
                SendVibrationToPlayer(leverOrder[currentLeverIndex]);
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


    void VibratePhone()
    {
#if UNITY_ANDROID
        Handheld.Vibrate(); // Vibrates on Android devices
#endif
        Debug.Log($"Phone vibrated for player {playerIndex}");
    }
}
