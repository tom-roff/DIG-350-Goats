using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class MicrophoneBehavior : NetworkBehaviour
{
    [Header("Audio Detector")]
    [SerializeField] public AudioLoudnessDetection detector;

    public float loudnessSensitivity = 100;
    public float quietThreshold = .75f;
    public float loudThreshold = 2f;

    public float listeningTime = 5f;
    public float timeIncorrect = 0f;
    public bool listening = false;
    public bool beLoud = false;

    [Header("Canvas Objects")]
    [SerializeField] public GameObject startButton;
    [SerializeField] public GameObject mapButton;
    [SerializeField] public TMP_Text timeText;
    [SerializeField] public GameObject background;

    [Header("Host")]
    public bool host = false;
    public int readyCount = 0;
    public int playerCount = 0;

    public Dictionary<int, float> finalScores = new Dictionary<int, float>();
    public int scoresReceived = 0;


    public void Start()
    {
        mapButton.SetActive(false);
    }


    public override void OnNetworkSpawn()
    {
        playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count-1;
        CheckHost();
        ChangeColor();
        base.OnNetworkSpawn();
    }

    void CheckHost()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if (clientId == GameManager.Instance.MapManager.hostId) // main screen
        {
            startButton.SetActive(false);
            host = true;
        }
        else
            host = false;
    }

    void FixedUpdate()
    {
        if (listening)
        {
            if (!host)
            {
                ComputeLoudness(Time.deltaTime);
            }
        }
    }

    public void ComputeLoudness(float deltaTime)
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensitivity;

        if (!beLoud)
        {
            if (loudness > quietThreshold)
            {
                Debug.Log("Too loud!: " + loudness);
                timeIncorrect += deltaTime;
                SendTimeRpc(deltaTime);
            }
        }
        else
        {
            if (loudness < loudThreshold)
            {
                Debug.Log("Too quiet!: " + loudness);
                timeIncorrect += deltaTime;
                SendTimeRpc(deltaTime);
            }
        }


    }

    [Rpc(SendTo.Server)]
    public void SendTimeRpc(float deltaTime)
    {
        timeIncorrect += deltaTime;
        timeText.text = "Total: " + timeIncorrect;
        SendTimeNotServerRpc(timeIncorrect);
    }

    [Rpc(SendTo.NotServer)]
    public void SendTimeNotServerRpc(float totalTime)
    {
        timeText.text = "Total: " + totalTime;
    }

    [Rpc(SendTo.Server)]
    public void SendReadyRpc()
    {
        readyCount++;
        if (playerCount != 0 && readyCount == playerCount)
        {
            StartGameRpc();
            listening = true;
            float timeUntilSwitch = UnityEngine.Random.Range(2f, 5f);
            Invoke("SwitchLoudness", timeUntilSwitch);
        }
    }


    public void SwitchLoudness()
    {
        if (listening)
        {

            if (beLoud)
                beLoud = false;
            else
                beLoud = true;

            ChangeColor();
            SwitchRpc();

            float timeUntilSwitch = UnityEngine.Random.Range(2f, 5f);
            Invoke("SwitchLoudness", timeUntilSwitch);
        }


    }

    [Rpc(SendTo.NotServer)]
    public void SwitchRpc()
    {
        if (beLoud)
            beLoud = false;
        else
            beLoud = true;

        ChangeColor();
    }

    [Rpc(SendTo.NotServer)]
    public void StartGameRpc()
    {
        listening = true;
        timeIncorrect = 0f;
        Invoke("StopListening", listeningTime);
    }


    public void StartListening()
    {
        if (!listening && !host)
        {
            SendReadyRpc();
            startButton.SetActive(false);

        }
    }

    [Rpc(SendTo.Server)]
    public void StopListeningRpc()
    {
        listening = false;
    }

    [Rpc(SendTo.Server)]
    public void SendFinalScoreRpc(int player, float score)
    {
        finalScores.Add(player, score);
        scoresReceived++;
        if (scoresReceived == playerCount)
        {
            PrintScores();
        }
    }

    public void PrintScores()
    {
        Dictionary<int, float> sortedScores = finalScores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        int i = 0;
        foreach (KeyValuePair<int, float> score in sortedScores)
        {
            Debug.Log("Player " + score.Key + " scored " + score.Value);
            if (4 - i > 0)
            {
                GameManager.Instance.MapManager.players[score.Key - 1].AddRerolls(3 - i);
            }
            i++;
        }

    }

    void StopListening()
    {
        listening = false;
        StopListeningRpc();
        Debug.Log("Incorrect loudness for: " + timeIncorrect + " seconds");
        SendFinalScoreRpc((int)NetworkManager.Singleton.LocalClientId, timeIncorrect);
        mapButton.SetActive(true);
    }

    void ChangeColor()
    {
        if (!beLoud)
            background.GetComponent<Image>().color = Color.red;
        else
            background.GetComponent<Image>().color = Color.green;

    }

    public void ReturnToMap()
    {
        ToMapRpc();
    }

    [Rpc(SendTo.Server)]
    public void ToMapRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }
}