using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MapMinigameTest : MonoBehaviour
{
    public void OnClick()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);

    }
}
