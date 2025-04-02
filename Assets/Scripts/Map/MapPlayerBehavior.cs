using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;

public class MapPlayerBehavior : NetworkBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    private ulong hostId;
    [SerializeField] public GameObject hostUI;
    [SerializeField] public GameObject playerUI;


    void Start()
    {
        hostId = NetworkManager.Singleton.LocalClientId;
        if (hostId == GameManager.Instance.MapManager.hostId) // main screen
        {
            playerUI.SetActive(false);
        }
        else
        {
            hostUI.SetActive(false);
        }
    }

    void CreatePlayerQueue()
    {
        int playerCount = GameManager.Instance.OurNetwork.playerIndexMap.Count;
        GameManager.Instance.MapManager.players = new MapPlayer[playerCount]; // replace with # of players in lobby

        int i = 0;
        foreach (KeyValuePair<ulong, PlayerInfo> entry in GameManager.Instance.OurNetwork.playerIndexMap)
        {
            // do something with entry.Value or entry.Key
            if (entry.Key != GameManager.Instance.MapManager.hostId)
            {
                GameManager.Instance.MapManager.players[i] = new MapPlayer(entry.Key);
            }
            else
            {
                Debug.Log("host found?");
            }
            i++;
        }
        
        Debug.Log("Players in queue: " + GameManager.Instance.MapManager.players.Length);
    }

    public void StartMap()
    {
        if (hostId != GameManager.Instance.MapManager.hostId) return;
        CreatePlayerQueue();
        SpawnPlayers();
        GameManager.Instance.MapManager.Play();
        GameManager.Instance.MapManager.NextPlayer();
    }

    private Vector2 FindStartPosition()
    {
        for (int i = 0; i < GameManager.Instance.MapManager.mapHeight; i++)
        {
            for (int j = 0; j < GameManager.Instance.MapManager.mapWidth; j++)
            {
                if (GameManager.Instance.MapManager.map[i, j] == MapManager.Tiles.Start)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.positiveInfinity;
    }

    void SpawnPlayers()
    {
        if (GameManager.Instance.MapManager.currentPlayer == -1)
        {
            Vector2 startPosition = FindStartPosition();
            foreach (var player in GameManager.Instance.MapManager.players)
            {
                InstantiatePlayer(startPosition, player);
            }
        }
        else
        {
            foreach (var player in GameManager.Instance.MapManager.players)
            {
                InstantiatePlayer(player.position, player);
            }
        }
    }

    

    void InstantiatePlayer(Vector2 startPosition, MapPlayer player)
    {
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerInstance.name = "player";
        playerInstance.transform.SetParent(GameManager.Instance.MapManager.tiles[(int)startPosition.x, (int)startPosition.y].transform);
        playerInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerInstance.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        player.body = playerInstance;
        player.SetPosition(startPosition);
        MapHelpers.CheckPosition(GameManager.Instance.MapManager.map, GameManager.Instance.MapManager.tiles, (int)startPosition.x, (int)startPosition.y);
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerRpc(float x, float y)
    {
        int i = (int)x;
        int j = (int)y;
        if (InBounds(i, j) && GameManager.Instance.MapManager.map[i, j] != MapManager.Tiles.Wall)
        {
            int currentPlayer = GameManager.Instance.MapManager.currentPlayer;
            GameManager.Instance.MapManager.players[currentPlayer].body.transform.SetParent(GameManager.Instance.MapManager.tiles[i, j].transform);
            GameManager.Instance.MapManager.players[currentPlayer].body.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            GameManager.Instance.MapManager.players[currentPlayer].SetPosition(new Vector2(i, j));

            MapHelpers.CheckPosition(GameManager.Instance.MapManager.map, GameManager.Instance.MapManager.tiles, i, j);
            GameManager.Instance.MapManager.moves--;
        }
    }

    
    void Update()
    {
        if (GameManager.Instance.MapManager.playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
        {
            // if playerID == currentPlayer
            // try move, successful moves-- && set MapPlayer position etc...
            // else do nothing 

            MapPlayer currentPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];

            if (currentPlayer.playerID == hostId)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayerRpc(currentPlayer.position.x, currentPlayer.position.y + 1);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayerRpc(currentPlayer.position.x, currentPlayer.position.y - 1);
                if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayerRpc(currentPlayer.position.x + 1, currentPlayer.position.y);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayerRpc(currentPlayer.position.x - 1, currentPlayer.position.y);
            }

            // if moves == 0
            // requeue currentPlayer, dequeue next player into currentPlayer
            // roll dice mechanism? 

            if (GameManager.Instance.MapManager.moves < 1)
            {
                GameManager.Instance.MapManager.NextPlayer();
            }
        }
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < GameManager.Instance.MapManager.mapHeight && j > -1 && j < GameManager.Instance.MapManager.mapWidth) return true;
        return false;
    }


}
