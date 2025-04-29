using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

public class MapTileBehavior : MonoBehaviour
{
    public int playerCount;
    public List<GameObject> players;

    [SerializeField] GameObject twoPlayers;
    [SerializeField] GameObject threePlayers;
    [SerializeField] GameObject fourPlayers;
    [SerializeField] GameObject fivePlayers;
    [SerializeField] GameObject sixPlayers;

    void Start()
    {
        playerCount = 0;
        players = new List<GameObject>();
    }

    public void AddPlayer(int i, int j)
    {
        playerCount++;
        int currentPlayer = GameManager.Instance.MapManager.currentPlayer;
        players[playerCount - 1] = GameManager.Instance.MapManager.players[currentPlayer].body;
        GameManager.Instance.MapManager.players[currentPlayer].SetPosition(new Vector2(i, j));
        PlayerPositions();
    }

    public void AddPlayer(GameObject newPlayer)
    {
        players[playerCount] = newPlayer;
        playerCount++;
        PlayerPositions();
    }

    public void RemovePlayer()
    {
        playerCount--;
        int currentPlayer = GameManager.Instance.MapManager.currentPlayer;
        players[playerCount - 1] = null;
        PlayerPositions();
    }

    void PlayerPositions()
    {
        switch (playerCount)
        {
            case 1:
                players[0].transform.SetParent(this.transform);
                players[0].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                break;
            case 2:
                ArrangePlayers(twoPlayers);
                break;
            case 3:
                ArrangePlayers(threePlayers);
                break;
            case 4:
                ArrangePlayers(fourPlayers);
                break;
            case 5:
                ArrangePlayers(fivePlayers);
                break;
            case 6:
                ArrangePlayers(sixPlayers);
                break;
        }
    }

    void ArrangePlayers(GameObject parentSpace)
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.SetParent(parentSpace.transform.Find((i+1).ToString()));
            players[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
    }
}
