using UnityEngine;
using UnityEngine.SceneManagement;

public class HelipadManager : MonoBehaviour
{
    public static HelipadManager Instance;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Collision -Game Over!");

        // TODO: Show Game Over UI here
        // gameOverUI.SetActive(true);

        // TEMP: Restart after delay
        Invoke("RestartScene", 2f);
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
