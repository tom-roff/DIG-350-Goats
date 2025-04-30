using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class ReturnToMap : NetworkBehaviour
{
    [SerializeField] public GameObject backButton;
    public override void OnNetworkSpawn()
    {
        CheckHost();
        base.OnNetworkSpawn();
    }

    void CheckHost()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if (clientId == GameManager.Instance.MapManager.hostId)
            backButton.SetActive(true);
        else
            backButton.SetActive(false);

    }

    public void BackToMap()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }
}