using UnityEngine;
using Unity.Netcode;

public class LocalScoreManager : MonoBehaviour
{
    public static double score = 0;
    public float gameDuration = 30f;

    private float timer;
    private bool gameEnded = false;

    void Start()
    {
        score = 0;
        timer = gameDuration;
    }

    void Update()
    {
        if (gameEnded) return;

        timer -= Time.deltaTime;

        if (timer <= 0f || score == 10)
        {
            gameEnded = true;
            Debug.Log("Game ended locally.");

            // Send final score (redundant if AddScore already does this)
            if (ScoreManager.Instance != null && ScoreManager.Instance.IsSpawned)
            {
                ScoreManager.Instance.SendScoreToServerRpc((float)score);
                ScoreManager.Instance.NotifyGameEndedServerRpc();
            }
        }
    }
    public static void AddScore(double points)
    {
        score += points;
        Debug.Log($"Score: {score}");
        if (ScoreManager.Instance != null && ScoreManager.Instance.IsSpawned)
        {
            ScoreManager.Instance.SendScoreToServerRpc((float)score);
        }
    }
}