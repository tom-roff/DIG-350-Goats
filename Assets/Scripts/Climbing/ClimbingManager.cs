using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ClimbingManager : NetworkBehaviour
{
    public int finishLine = 30;
    public List<ulong> leaderboard = new List<ulong>();

    [SerializeField] private GameObject playerUI;
    
    private Dictionary<ulong, float> playerHeights = new Dictionary<ulong, float>();
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private EndLevel endLevel;
    private OurNetwork ourNetwork;
    private bool hasEnded = false;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId) continue;

                playerHeights[clientId] = 0f;
            }
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        ourNetwork = GameManager.Instance.OurNetwork;
    }

    private void Start()
    {
        ourNetwork = GameManager.Instance.OurNetwork;
    }

    // Called by player controllers to update their position
    [Rpc(SendTo.Server)]
    public void UpdatePlayerHeightRpc(ulong clientId, float height)
    {
        playerHeights[clientId] = height;
    }

    public float GetPlayerHeight(ulong clientId)
    {
        if (playerHeights.ContainsKey(clientId))
        {
            return playerHeights[clientId];
        }
        return 0f;
    }

    public float GetFinishHeight()
    {
        return finishLine;
    }
    
    public void PlayerFinished(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != 0)
        {
            UpdateScoringRpc(clientId);
            playerUI.gameObject.SetActive(true);
        }
    }

    [Rpc(SendTo.Server)]
    public void UpdateScoringRpc(ulong clientId)
    {
        if (!leaderboard.Contains(clientId))
        {
            leaderboard.Add(clientId);
        }
        if (leaderboard.Count == NetworkManager.Singleton.ConnectedClients.Count - 1)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (hasEnded) return;
        hasEnded = true;
        endLevel.leaderboard = leaderboard;
        endUI.SetActive(true);
        gameUI.SetActive(false);
    }
}
