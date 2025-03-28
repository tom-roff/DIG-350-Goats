using Unity.Netcode;
using UnityEngine;

public class LaserUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        
        UpdateScores();
    }

    private void UpdateScores()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != hostId)
            {
                scoreText.text = laserManager.GetScore(playerId);
            }
        }
    }
}
