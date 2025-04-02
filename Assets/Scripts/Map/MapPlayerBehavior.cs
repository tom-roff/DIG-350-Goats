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
    private ulong clientId;
    public bool host = false;
    [SerializeField] public GameObject hostUI;
    [SerializeField] public GameObject playerUI;
    public ulong currentPlayerId;
    public Vector2 currentPlayerPosition;

    void OnEnable()
    {
        CheckHost();
    }

    void CheckHost()
    {
        clientId = NetworkManager.Singleton.LocalClientId;
        if (clientId == GameManager.Instance.MapManager.hostId) // main screen
        {
            playerUI.SetActive(false);
            host = true;
        }
        else
        {
            hostUI.SetActive(false);
            host = false;
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
            if (entry.Key != clientId)
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
        CheckHost();
        Debug.Log("startmap");
        if (!host) return;
        Debug.Log("doing host stuff");
        CreatePlayerQueue();
        SpawnPlayers();
        GameManager.Instance.MapManager.Play();
        GameManager.Instance.MapManager.NextPlayer();
        // MapPlayer tempPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];
        SendCurrentPlayerRpc(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID,
                            GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].position);
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
            // MapAudioManager.playerMovementAudio.Play();
            Debug.Log("moved");
            GameManager.Instance.MapManager.moves--;
            if (GameManager.Instance.MapManager.moves < 1)
            {
                GameManager.Instance.MapManager.NextPlayer();
                MapPlayer tempPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];
                SendCurrentPlayerRpc(tempPlayer.playerID, tempPlayer.position);
            }
        }
    }

    [Rpc(SendTo.NotServer)]
    public void SendCurrentPlayerRpc(ulong currentPlayerId, Vector2 currentPlayerPosition)
    {
        this.currentPlayerId = currentPlayerId;
        this.currentPlayerPosition = currentPlayerPosition;
    }

    
    void Update()
    {
        if (GameManager.Instance.MapManager.playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
        {
            

            // client does this? 
            if (currentPlayerId == clientId)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayerRpc(currentPlayerPosition.x, currentPlayerPosition.y + 1);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayerRpc(currentPlayerPosition.x, currentPlayerPosition.y - 1);
                if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayerRpc(currentPlayerPosition.x + 1, currentPlayerPosition.y);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayerRpc(currentPlayerPosition.x - 1, currentPlayerPosition.y);
            }


            
        }
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < GameManager.Instance.MapManager.mapHeight && j > -1 && j < GameManager.Instance.MapManager.mapWidth) return true;
        return false;
    }


}
