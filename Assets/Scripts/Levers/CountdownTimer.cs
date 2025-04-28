using TMPro;
using UnityEngine;
using Unity.Netcode;


public class CountdownTimer : NetworkBehaviour
{
    public float timeRemaining = 60f;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timerText;
    public GameObject tutorialText;

    private ulong playerId;
    private GameEndManager gameEndManager;

    void Start()
    {

        playerId = NetworkManager.Singleton.LocalClientId;

        if (playerId != 0)
        {
            timerText.gameObject.SetActive(false);
            tutorialText.SetActive(false);
    
            this.enabled = false;
            return;
        }

        UpdateTimerDisplay(timeRemaining);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                HandleTimeOut();
            }
        }
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay); // Avoid negative values
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);

        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    void HandleTimeOut()
    {
        Debug.Log("Players failed to finish in time — punishment triggered.");
        gameEndManager.OnGameLose();
    }

    // Optional: Public methods to control the timer externally
    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
