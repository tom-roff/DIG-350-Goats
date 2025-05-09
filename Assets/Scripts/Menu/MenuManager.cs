using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MenuManager : MonoBehaviour
{
    public Button hostButton;
    public TMP_Text joinCodeText;
    public TMP_InputField joinCodeInput;
    public Button joinButton;
    public TMP_Text playerCountText;
    public Button startGameButton;
    public Button startLeverGameButton;

    public Button startMagnetButton;
    public Button startLaserButton;
    public Button startClimbButton;
    public Button startHackButton;
    public Button phoneModeButton;
    public GameObject hostUI;
    public GameObject clientJoinedUI;
    public GameObject clientStartUI;
    public TMP_InputField nameInput;
    public Button confirmNameButton;
    public TMP_Text nameText;
    public TMP_Text waitingForHostToStartText;
    public TMP_Text joinText;
    public PlayerUIEntry[] playerEntries = new PlayerUIEntry[6];
    [SerializeField] public OurNetwork ourNetwork;
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private GameManager networkPrefab;
    public GameObject startMenu;

    void Awake()
    {
        startMenu.SetActive(true);
        Screen.orientation = ScreenOrientation.Portrait;        
    }

    private void Start()
    {
        // if (ourNetwork == null)
        // {
        //     ourNetwork = FindObjectOfType<OurNetwork>();
        //     if (ourNetwork == null)
        //     {
        //         Debug.LogError("ourNetwork not found in the scene!");
        //         return;
        //     }
        // }

        hostButton.onClick.AddListener(lobbyManager.HostGame);
        joinButton.onClick.AddListener(() => joinButtonClicked());
        startGameButton.onClick.AddListener(lobbyManager.StartGame);
        confirmNameButton.onClick.AddListener(() => lobbyManager.OnNameInput(nameInput.text));
        startGameButton.gameObject.SetActive(false);
        startLeverGameButton.onClick.AddListener(lobbyManager.StartLeverGame);
        startLaserButton.onClick.AddListener(lobbyManager.StartLaserGame);
        startClimbButton.onClick.AddListener(lobbyManager.StartClimbGame);
        startHackButton.onClick.AddListener(lobbyManager.StartHackGame);
        startMagnetButton.onClick.AddListener(lobbyManager.StartMagnetGame);
        playerCountText.gameObject.SetActive(false);
        phoneModeButton.onClick.AddListener(EnterPhoneMode);

        if(Structs.IsComputer == true){
            hostUI.gameObject.SetActive(true);
            clientStartUI.gameObject.SetActive(false);
        }
        else{
            hostUI.gameObject.SetActive(false);
            clientStartUI.gameObject.SetActive(true);
        }
    }

    void Update(){
        if(ourNetwork != null){
            UpdatePlayerCountDisplay(6);
        }
    }

    // Start menu buttons
    public void StartMenu_OnHostButton()
    {
        Debug.Log("StartMenu_OnHostButton");
        // set host
        lobbyManager.isHost = true;
        Structs.IsComputer = true;
        startMenu.SetActive(false);
        lobbyManager.HostGame();
    }
    public void StartMenu_OnJoinButton()
    {
        Debug.Log("StartMenu_OnJoinButton");
        // set client
        lobbyManager.isHost = false;
        Structs.IsComputer = false;
        startMenu.SetActive(false);
        EnterPhoneMode();
    }
    // \Start menu buttons

    public void EnterPhoneMode()
    {
        hostUI.gameObject.SetActive(false);
        clientStartUI.gameObject.SetActive(true);
    }

    public void UpdateJoinCodeDisplay(string code)
    {
        joinCodeText.text = $"Join Code: {code}";
    }

    public void UpdatePlayerCountDisplay(int maxPlayers)
    {
        if(ourNetwork != null && playerCountText != null){
            int playersCountRightNow = ourNetwork.playerInfoList.Count - 1;
            playerCountText.text = $"Players: {playersCountRightNow}/{maxPlayers}";
            playerCountText.gameObject.SetActive(true);
        }
        else{
            Debug.Log("No ourNetwork found!");
        }
    }

    public void ShowStartButton(bool show)
    {
        startGameButton.gameObject.SetActive(show);
    }

    public void joinButtonClicked(){
        Debug.Log("Join Button Clicked");
        lobbyManager.JoinGame(joinCodeInput.text);
    }
}
