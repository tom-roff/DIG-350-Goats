using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LaserUI : NetworkBehaviour
{
    [SerializeField] private LaserManager laserManager;
    [SerializeField] private GameObject scoresParent;

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
        foreach (TMP_Text scoreText in scoresParent.GetComponentsInChildren<TMP_Text>())
        {
            scoreText.text = $"Score: {laserManager.GetScore()}";
        }
    }
}
