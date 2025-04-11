using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

public class MapPlayerBehavior : NetworkBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    public ulong clientId;
    public bool host = false;
    [SerializeField] public GameObject hostUI;
    [SerializeField] public GameObject playerUI;
    public ulong currentPlayerId = ulong.MinValue;

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
        if (!host)
        {
            GameManager.Instance.MapManager.Play();
            return;
        }
        if (GameManager.Instance.MapManager.players == null)
            CreatePlayerQueue();
        SpawnPlayers();
        GameManager.Instance.MapManager.Play();
        GameManager.Instance.MapManager.NextPlayer();
        // SendCurrentPlayerRpc(currentPlayerId);


    }

    public override void OnNetworkSpawn()
    {
        if (!host)
            AskForCurrentPlayerRpc();
        if (GameManager.Instance.MapManager.currentPlayer == -1) return;
        ulong currentPlayerId = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID;
        SendCurrentPlayerRpc(currentPlayerId);
        base.OnNetworkSpawn();
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
    public void MovePlayerRpc(int x, int y)
    {
        Vector2 currentPlayerPosition = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].position;
        int i = (int)currentPlayerPosition.x + x;
        int j = (int)currentPlayerPosition.y + y;
        if (InBounds(i, j) && GameManager.Instance.MapManager.map[i, j] != MapManager.Tiles.Wall)
        {
            int currentPlayer = GameManager.Instance.MapManager.currentPlayer;
            GameManager.Instance.MapManager.players[currentPlayer].body.transform.SetParent(GameManager.Instance.MapManager.tiles[i, j].transform);
            GameManager.Instance.MapManager.players[currentPlayer].body.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            GameManager.Instance.MapManager.players[currentPlayer].SetPosition(new Vector2(i, j));

            CheckSceneChange(i,j);
            MapHelpers.CheckPosition(GameManager.Instance.MapManager.map, GameManager.Instance.MapManager.tiles, i, j);
            // MapAudioManager.playerMovementAudio.Play();

            GameManager.Instance.MapManager.moves--;
            if (GameManager.Instance.MapManager.moves < 1)
            {
                GameManager.Instance.MapManager.NextPlayer();
                SendCurrentPlayerRpc(currentPlayerId);
                // SendCurrentPlayerRpc(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID);
            }
        }
    }

    void CheckSceneChange(int x, int y)
    {
        if (GameManager.Instance.MapManager.map[x, y] == MapManager.Tiles.PeakedMinigame)
        {
            GameManager.Instance.MapManager.map[x, y] = MapManager.Tiles.ExploredMinigame; // I think this is needed? 
            NetworkManager.Singleton.SceneManager.LoadScene("TEST_MapMinigame", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void SendCurrentPlayerRpc(ulong currentPlayerId)
    {
        this.currentPlayerId = currentPlayerId;
    }

    [Rpc(SendTo.Server)]
    public void AskForCurrentPlayerRpc()
    {
        if (GameManager.Instance.MapManager.currentPlayer == -1) return;
        ulong currentPlayerId = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID;
        SendCurrentPlayerRpc(currentPlayerId);
    }


    // void Update()
    // {
    //     if (GameManager.Instance.MapManager.playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
    //     {


    //         // client does this? 
    //         if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId)
    //         {
    //             if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayerRpc(0, 1);
    //             if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayerRpc(0, -1);
    //             if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayerRpc(1, 0);
    //             if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayerRpc(-1, 0);
    //         }
    //         else if (currentPlayerId == ulong.MinValue)
    //         {
    //             AskForCurrentPlayerRpc();
    //         }



    //     }
    // }

    bool InBounds(int i, int j)
    {
        if (i > -1 && i < GameManager.Instance.MapManager.mapHeight && j > -1 && j < GameManager.Instance.MapManager.mapWidth) return true;
        return false;
    }



    // BUTTON ARROW KEY AREA
    public void MoveRight()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(0, 1);
            else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveLeft()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(0, -1);
            else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveUp()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(1, 0);
            else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveDown()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(-1, 0);
            else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }


}
