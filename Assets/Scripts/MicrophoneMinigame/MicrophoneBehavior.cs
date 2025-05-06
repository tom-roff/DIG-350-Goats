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
    public float loudThreshold = 1.25f;

    public float listeningTime = 10f;
    public float timeIncorrect = 0f;
    public bool listening = false;
    public bool beLoud = false;

    [Header("Canvas Objects")]
    [SerializeField] public GameObject tutorialObjects;
    [SerializeField] public TMP_Text finalScoreText;
    [SerializeField] public Light indicatorLight;

    [Header("Host")]
    public bool host = false;
    public int readyCount = 0;
    public int playerCount = 0;

    public Dictionary<int, float> finalScores = new Dictionary<int, float>();
    public int scoresReceived = 0;
    private GameObject[] loudnessMeters;
    [SerializeField] GameObject loudnessMeterPrefab;
    [SerializeField] GameObject meterLocation;
    [SerializeField] GameObject backButton;

    int clientId;


    void Start()
    {
        backButton.SetActive(false);
    }


    public override void OnNetworkSpawn()
    {
        playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count - 1;
        clientId = (int)NetworkManager.Singleton.LocalClientId;
        loudnessMeters = new GameObject[playerCount];
        CheckHost();
        ChangeColor();
        base.OnNetworkSpawn();
    }

    void CheckHost()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if (clientId == GameManager.Instance.MapManager.hostId) // main screen
        {
            tutorialObjects.transform.Find("Start").gameObject.SetActive(false);
            host = true;
            BuildMeters();
        }
        else
            host = false;
    }

    void BuildMeters()
    {
        float anchorOffset = 1f / (float)playerCount;
        //build all the meter things
        for (int i = 0; i < playerCount; i++)
        {
            GameObject newMeter = Instantiate(loudnessMeterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newMeter.transform.SetParent(meterLocation.transform);
            newMeter.GetComponent<RectTransform>().anchorMin = new Vector2(anchorOffset * (float)i, 0);
            newMeter.GetComponent<RectTransform>().anchorMax = new Vector2(anchorOffset * (float)(i + 1), 0);
            newMeter.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            newMeter.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            newMeter.GetComponent<Image>().color = GameManager.Instance.OurNetwork.playerInfoList[i + 1].playerColor.colorRGB;

            loudnessMeters[i] = newMeter;
        }
    }

    public void SendReady()
    {
        if (!listening && !host)
        {
            SendReadyRpc();
            tutorialObjects.transform.Find("Start").gameObject.SetActive(false);
        }
    }

    [Rpc(SendTo.Server)]
    public void SendReadyRpc()
    {
        readyCount++;
        if (playerCount != 0 && readyCount == playerCount)
        {
            StartGameRpc();
            tutorialObjects.SetActive(false);
            listening = true;
            float timeUntilSwitch = UnityEngine.Random.Range(2f, 5f);
            Invoke("SwitchLoudness", timeUntilSwitch);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void StartGameRpc()
    {
        tutorialObjects.SetActive(false);
        listening = true;
        timeIncorrect = 0f;
        Invoke("StopListening", listeningTime);
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
                timeIncorrect += deltaTime;
            }
        }
        else
        {
            if (loudness < loudThreshold)
            {
                timeIncorrect += deltaTime;
            }
        }

        SendLoudnessRpc(clientId, loudness);


    }


    [Rpc(SendTo.Server)]
    public void SendLoudnessRpc(int player, float loudness)
    {
        //loudness meter scale adjustment 
        Vector2 tempVect = loudnessMeters[player - 1].GetComponent<RectTransform>().anchorMax;
        loudnessMeters[player - 1].GetComponent<RectTransform>().anchorMax = new Vector2(tempVect.x, loudness);
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

            float timeUntilSwitch = UnityEngine.Random.Range(.5f, 4f);
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
        string finalResults = "";
        foreach (KeyValuePair<int, float> score in sortedScores)
        {
            finalResults += GameManager.Instance.MapManager.players[score.Key - 1].name + " got " + (10 - i) + " points <br>";
            if (4 - i > 0)
            {
                GameManager.Instance.MapManager.players[score.Key - 1].AddRerolls(3 - i);
            }
            GameManager.Instance.OurNetwork.SetPlayerScoreRpc(score.Key, 10 - i);
            i++;
        }


        finalScoreText.text = finalResults;
        meterLocation.SetActive(false);
        // GameManager.Instance.MapManager.TimedReturnToMap();
        

    }

    public void BackToMap()
    {
        GameManager.Instance.MapManager.ReturnToMap();
    }

    void StopListening()
    {
        listening = false;
        StopListeningRpc();
        Debug.Log("Incorrect loudness for: " + timeIncorrect + " seconds");
        SendFinalScoreRpc((int)NetworkManager.Singleton.LocalClientId, timeIncorrect);
        backButton.SetActive(true);
        
    }

    void ChangeColor()
    {
        if (!beLoud)
            indicatorLight.color = Color.red;
        else
            indicatorLight.color = Color.green;

    }


}