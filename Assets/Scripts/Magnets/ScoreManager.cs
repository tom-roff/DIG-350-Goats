// using UnityEngine;
// using System.Collections.Generic;
// using Unity.Netcode;
// using System.Linq;

// public class ScoreManager : NetworkBehaviour
// {
//     [Header("Host")]
//     public bool host = false;
//     public int readyCount = 0;
//     public int playerCount = 0;



//     public Dictionary<ulong, float> playerScores = new Dictionary<ulong, float>();
//     public int scoresReceived = 0;

//     int clientId;
//     public override void OnNetworkSpawn()
//     {
//         playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count - 1;
//         clientId = (int)NetworkManager.Singleton.LocalClientId;
//         base.OnNetworkSpawn();
//         if (IsServer)
//         {
//             foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
//             {
//                 playerScores[client] = 0f;
//             }
//         }
//     }
    
//     [Rpc(SendTo.Server)]
//     public void SendFinalScoreRpc(int player, float score)
//     {
//         playerScores.Add(player, score); 
//         scoresReceived++;
//         if (scoresReceived == playerCount)
//         {
//             PrintScores();
//         }
//     }
//         public void PrintScores()
//     {
//         Dictionary<int, float> sortedScores = playerScores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
//         int i = 0;
//         string finalResults = "";
//         foreach (KeyValuePair<int, float> score in sortedScores)
//         {
//             finalResults += GameManager.Instance.MapManager.players[score.Key - 1].name + " got " + (10-i) + " points <br>";
//             if (4 - i > 0)
//             {
//                 GameManager.Instance.MapManager.players[score.Key - 1].AddRerolls(3 - i);
//             }
//             GameManager.Instance.OurNetwork.SetPlayerScoreRpc(score.Key, 10-i);
//             i++;
//         }
//     }
//}

using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    private Dictionary<ulong, float> playerScores = new Dictionary<ulong, float>();

    [Header("Timer Settings")]
    public float gameDuration = 10f; // seconds
    private float timer;
    // private bool gameEnded = false; // not used?



    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientId == NetworkManager.Singleton.LocalClientId) continue;

                playerScores[clientId] = 0f;
                Debug.Log($"Player {clientId} initialized with score: {playerScores[clientId]}");
            }
        }
    }

    private HashSet<ulong> playersFinished = new HashSet<ulong>();

    [ServerRpc(RequireOwnership = false)]
    public void NotifyGameEndedServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!playersFinished.Contains(clientId))
        {
            playersFinished.Add(clientId);
            Debug.Log($"Player {clientId} has finished their game.");
        }

        if (playersFinished.Count == NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            Debug.Log("All players finished! Returning to Map.");
            //NetworkManager.SceneManager.LoadScene("Map", LoadSceneMode.Single);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendScoreToServerRpc(float score, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!playerScores.ContainsKey(clientId))
        {
            playerScores[clientId] = 0f;
        }

        playerScores[clientId] = score; // Update the player's score on the server
        Debug.Log($"Player {clientId} sent score: {score}");

        // Optionally, broadcast updated scores to all clients
        UpdateScoresClientRpc(clientId, playerScores[clientId]);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateScoresClientRpc(ulong clientId, float score)
    {
        playerScores[clientId] = score;
    }

    public Dictionary<ulong, float> GetScores()
    {
        return playerScores;
    }
}
