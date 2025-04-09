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
    public GameObject clientJoinedUI;
    public GameObject clientStartUI;
    public TMP_InputField nameInput;
    public Button confirmNameButton;
    public TMP_Text nameText;
    public TMP_Text waitingForHostToStartText;
    public TMP_Text joinText;
    public PlayerUIEntry[] playerEntries = new PlayerUIEntry[6];
    [SerializeField] private OurNetwork ourNetwork;
    [SerializeField] private LobbyManager lobbyManager;

    private void Start()
    {
        if (ourNetwork == null)
        {
            ourNetwork = FindObjectOfType<OurNetwork>();
            if (ourNetwork == null)
            {
                Debug.LogError("ourNetwork not found in the scene!");
                return;
            }
        }

        ourNetwork.Initialize(this);
        lobbyManager.Initialize(this, ourNetwork);

        hostButton.onClick.AddListener(lobbyManager.HostGame);
        joinButton.onClick.AddListener(() => joinButtonClicked());
        startGameButton.onClick.AddListener(lobbyManager.StartGame);
        confirmNameButton.onClick.AddListener(() => lobbyManager.OnNameInput(nameInput.text));
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

    public void joinButtonClicked(){
        lobbyManager.JoinGame(joinCodeInput.text);
    }
}
