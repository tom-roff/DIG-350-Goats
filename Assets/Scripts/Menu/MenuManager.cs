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

    [SerializeField] private OurNetwork ourNetwork;

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

        hostButton.onClick.AddListener(ourNetwork.HostGame);
        joinButton.onClick.AddListener(() => ourNetwork.JoinGame(joinCodeInput.text));
        startGameButton.onClick.AddListener(ourNetwork.StartGame);
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
