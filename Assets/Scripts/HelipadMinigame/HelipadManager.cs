using UnityEngine;
using UnityEngine.SceneManagement;

public class HelipadManager : MonoBehaviour
{
    public static HelipadManager Instance;

    public GameObject StartUI;
    public GameObject EndGameUI;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // instructions before game starts
        Time.timeScale = 0f;
        StartUI.SetActive(true);
        EndGameUI.SetActive(false);
    }

    public void StartGame()
    {
        StartUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Collision -Game Over!");

        // TODO: Show Game Over UI here
        // gameOverUI.SetActive(true);

        // TEMP: Restart after delay
        Time.timeScale = 0f;
        EndGameUI.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
