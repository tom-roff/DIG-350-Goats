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
    [SerializeField] public MapUI mapUI;
    
    public ulong currentPlayerId = ulong.MinValue;
    int mapWidth;
    int mapHeight;
    public bool rerollAvailable = false;


    void OnEnable()
    {
        mapWidth = GameManager.Instance.MapManager.MapWidth();
        mapHeight = GameManager.Instance.MapManager.MapHeight();
    }

    public void StartMap()
    {
        CheckHost();
        if (!host)
        {
            GameManager.Instance.MapManager.Play();
            mapUI.DisableRerolling();
            return;
        }

        if (GameManager.Instance.MapManager.players == null) CreatePlayerQueue();

        SpawnPlayers();
        GameManager.Instance.MapManager.Play();
        GameManager.Instance.MapManager.NextPlayer();
        mapUI.DisplayText(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].name + " rolled a " + GameManager.Instance.MapManager.moves);
        mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
    }

    public override void OnNetworkSpawn()
    {
        CheckHost();
        if (!host)
            return;

        AskForCurrentPlayerRpc();
        base.OnNetworkSpawn();
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
        int playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count;
        GameManager.Instance.MapManager.players = new MapPlayer[playerCount-1]; // replace with # of players in lobby

        int i = 0;
        foreach (PlayerInfo entry in GameManager.Instance.OurNetwork.playerInfoList)
        {
            if(i != 0)
                GameManager.Instance.MapManager.players[i-1] = new MapPlayer((ulong)i,entry);
            
            i++;
        }

        Debug.Log("Players in queue: " + GameManager.Instance.MapManager.players.Length);
    }

   

    private Vector2 FindStartPosition()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
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
        playerInstance.name = player.name;
        playerInstance.transform.SetParent(GameManager.Instance.MapManager.tiles[(int)startPosition.x, (int)startPosition.y].transform);
        playerInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerInstance.GetComponent<Image>().color = player.color;
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
            mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
            DisableRerollingRpc();

            if (GameManager.Instance.MapManager.moves < 1)
            {
                GameManager.Instance.MapManager.NextPlayer();
                mapUI.DisplayText(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].name + " rolled a " + GameManager.Instance.MapManager.moves);
                mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
                AskForCurrentPlayerRpc();
            }
        }
    }

    void CheckSceneChange(int x, int y)
    {
        if (GameManager.Instance.MapManager.map[x, y] == MapManager.Tiles.PeakedMinigame)
        {
            GameManager.Instance.MapManager.map[x, y] = MapManager.Tiles.ExploredMinigame; // I think this is needed? 
            NetworkManager.Singleton.SceneManager.LoadScene("MicrophoneMinigame", LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void SendCurrentPlayerRpc(ulong currentPlayerId)
    {
        this.currentPlayerId = currentPlayerId;
    }

    [Rpc(SendTo.NotServer)]
    public void CheckRerollsRpc(int rerolls)
    {
        if (currentPlayerId == clientId && rerolls > 0)
        {
            rerollAvailable = true;
            mapUI.EnableRerolling();
            mapUI.SetRerollText(rerolls);
        }
        else
        {
            Debug.Log("unavailable");
            rerollAvailable = false;
            mapUI.DisableRerolling();
            mapUI.SetRerollText(rerolls);

        }

    }

    [Rpc(SendTo.NotServer)]
    public void DisableRerollingRpc()
    {
        Debug.Log("unavailable");
        rerollAvailable = false;
        mapUI.DisableRerolling();
    }



    [Rpc(SendTo.Server)]
    public void AskForCurrentPlayerRpc()
    {
        if (GameManager.Instance.MapManager.currentPlayer == -1) return;
        ulong currentPlayerId = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID;
        SendCurrentPlayerRpc(currentPlayerId);
        CheckRerollsRpc(GameManager.Instance.MapManager.players[(int)currentPlayerId-1].rerolls);
    }


    bool InBounds(int i, int j)
    {
        if (i > -1 && i < mapHeight && j > -1 && j < mapWidth) return true;
        return false;
    }


    [Rpc(SendTo.Server)]
    void RerollRpc()
    {
        System.Random rnd = new System.Random();
        GameManager.Instance.MapManager.moves = rnd.Next(1, 7);
        MapPlayer currentPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];
        currentPlayer.AddRerolls(-1);
        mapUI.DisplayText("Rerolled moves: " + GameManager.Instance.MapManager.moves);
        mapUI.SetMovesText(GameManager.Instance.MapManager.moves);

        CheckRerollsRpc(currentPlayer.rerolls);
    }

    public void Reroll()
    {
        RerollRpc();
    }



    // BUTTON ARROW KEY AREA
    public void MoveRight()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(0, 1);
            // else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveLeft()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(0, -1);
            // else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveUp()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(1, 0);
            // else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }

    public void MoveDown()
    {
        if (GameManager.Instance.MapManager.playing)
        {
            if (currentPlayerId != ulong.MinValue && currentPlayerId == clientId) MovePlayerRpc(-1, 0);
            // else if (currentPlayerId == ulong.MinValue) AskForCurrentPlayerRpc();
        }
    }


}
