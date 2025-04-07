using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;
using System.Linq;

public class MicrophoneBehavior : NetworkBehaviour
{
    [SerializeField] public AudioLoudnessDetection detector;

    public float loudnessSensitivity = 100;
    public float threshold = 0.1f;

    public float listeningTime = 30f;
    public float timeTooLoud = 0f;
    public bool listening = false;
    [SerializeField] public GameObject startButton;
    [SerializeField] public TMP_Text timeText;
    public float loudnessThreshold = 1f;
    public bool host = false;
    public int readyCount = 0;
    public int playerCount = 0;


    void Start()
    {
        startButton.GetComponent<Image>().color = Color.green;
        
    }

    public override void OnNetworkSpawn()
    {
        playerCount = GameManager.Instance.OurNetwork.playerIndexMap.Count;
        CheckHost();
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
        {
            host = false;
        }
    }

    // Update is called once per frame
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

    [Rpc(SendTo.Server)]
    public void SendTimeRpc(float deltaTime)
    {
        timeTooLoud += deltaTime;
        timeText.text = "Total: " + timeTooLoud;
        SendTimeNotServerRpc(timeTooLoud);
    }

    [Rpc(SendTo.NotServer)]
    public void SendTimeNotServerRpc(float totalTime)
    {
        timeText.text = "Total: " + totalTime;
    }


    public void ComputeLoudness(float deltaTime)
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensitivity;
        if (loudness < threshold)
            loudness = 0;

        if (loudness > loudnessThreshold)
        {
            Debug.Log("Too loud!: " + loudness);
            timeTooLoud += deltaTime;
            SendTimeRpc(deltaTime);
        }
        
    }

    [Rpc(SendTo.Server)]
    public void SendReadyRpc()
    {
        readyCount++;
        if (playerCount != 0 && readyCount == playerCount)
        {
            StartGameRpc();
        }
    }

    [Rpc(SendTo.NotServer)]
    public void StartGameRpc()
    {
        listening = true;
        timeTooLoud = 0f;
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

    void StopListening()
    {
        listening = false;
        Debug.Log("Too loud for: " + timeTooLoud + " seconds");
        // startButton.GetComponent<Image>().color = Color.green;
    }
}