using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Restart : NetworkBehaviour
{
    public void GameRestart()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LaserMinigame", LoadSceneMode.Single);
    }
}
