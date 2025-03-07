using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public Button hostButton;
    public TMP_Text joinCodeText;
    public TMP_InputField joinCodeInput;
    public Button joinButton;
    public TMP_Text playerCountText;
    public Button startGameButton;

    [SerializeField] private NetworkManager networkManager;

    private void Start()
    {
        if (networkManager == null)
        {
            networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager == null)
            {
                Debug.LogError("NetworkManager not found in the scene!");
                return;
            }
        }

        networkManager.Initialize(this);

        hostButton.onClick.AddListener(networkManager.HostGame);
        joinButton.onClick.AddListener(() => networkManager.JoinGame(joinCodeInput.text));
        startGameButton.onClick.AddListener(networkManager.StartGame);
        startGameButton.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
    }

    public void UpdateJoinCodeDisplay(string code)
    {
        joinCodeText.text = $"Join Code: {code}";
    }

    public void UpdatePlayerCountDisplay(int currentPlayers, int maxPlayers)
    {
        playerCountText.text = $"Players: {currentPlayers}/{maxPlayers}";
        playerCountText.gameObject.SetActive(true);
    }

    public void ShowStartButton(bool show)
    {
        startGameButton.gameObject.SetActive(show);
    }
}
