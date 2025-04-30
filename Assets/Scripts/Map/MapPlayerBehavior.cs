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
    [Header("References")]
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject hostUI;
    [SerializeField] public GameObject playerUI;
    [SerializeField] public MapUI mapUI;

    [Header("Status")]
    public ulong currentPlayerId = ulong.MinValue;
    public bool rerollAvailable = false;
    public bool host;

    // other references used locally 
    int mapWidth;
    int mapHeight;
    ulong clientId;




    //////////////////////////////////////////////////////
    //////////////////// ASSIGN REFS /////////////////////
    //////////////////////////////////////////////////////
    void OnEnable()
    {
        mapWidth = GameManager.Instance.MapManager.MapWidth();
        mapHeight = GameManager.Instance.MapManager.MapHeight();

        EventManager.StartListening("Preparing", Preparing);
        EventManager.StartListening("Rolling", Rolling);
        EventManager.StartListening("Moving", Moving);
    }

    void OnDisable()
    {
        EventManager.StopListening("Preparing", Preparing);
        EventManager.StopListening("Rolling", Rolling);
        EventManager.StopListening("Moving", Moving);
    }

    public override void OnNetworkSpawn()
    {
        CheckHost();
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




    //////////////////////////////////////////////////////
    ///////////////////// PREPARING //////////////////////
    ////////////////////////////////////////////////////// 
    public void Preparing()
    {
        if (!host)
        {
            EventManager.TriggerEvent("NextState");
            return;
        }

        if (GameManager.Instance.MapManager.players == null) CreatePlayerQueue();
        SpawnPlayers();
        EventManager.TriggerEvent("NextState");
    }

    void CreatePlayerQueue()
    {
        int playerCount = GameManager.Instance.OurNetwork.playerInfoList.Count;
        GameManager.Instance.MapManager.players = new MapPlayer[playerCount - 1]; // replace with # of players in lobby

        int i = 0;
        foreach (PlayerInfo entry in GameManager.Instance.OurNetwork.playerInfoList)
        {
            if (i != 0)
                GameManager.Instance.MapManager.players[i - 1] = new MapPlayer((ulong)i, entry);

            i++;
        }

        Debug.Log("Players in queue: " + GameManager.Instance.MapManager.players.Length);
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



    //////////////////////////////////////////////////////
    ////////////////////// ROLLING ///////////////////////
    ////////////////////////////////////////////////////// 
    public void Rolling()
    {
        if (!host) return;
        if (GameManager.Instance.MapManager.moves > 0)
        {
            ResumeTurn();
            return;
        }

        GameManager.Instance.MapManager.NextPlayer();
        mapUI.DisplayText(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].name + " rolled a " + GameManager.Instance.MapManager.moves);
        mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
        SendCurrentPlayerRpc(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID);
        CheckRerollsRpc(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].rerolls);
    }

    void ResumeTurn()
    {
        mapUI.DisplayText(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].name + " has " + GameManager.Instance.MapManager.moves + " remaining moves");
        mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
        SendCurrentPlayerRpc(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID);
        SendRerolls();
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
            rerollAvailable = false;
            mapUI.DisableRerolling();
            mapUI.SetRerollText(rerolls);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void SendCurrentPlayerRpc(ulong currentPlayerId)
    {
        this.currentPlayerId = currentPlayerId;
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

    public void SendRerolls()
    {
        for (int i = 0; i < GameManager.Instance.MapManager.players.Length; i++)
        {
            SendRerollToClientRpc(i + 1, GameManager.Instance.MapManager.players[i].rerolls);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void SendRerollToClientRpc(int client, int rerolls)
    {
        if ((ulong)client == clientId)
        {
            mapUI.SetRerollText(rerolls);
        }
    }


    //////////////////////////////////////////////////////
    ////////////////////// MOVING ////////////////////////
    ////////////////////////////////////////////////////// 

    public void Moving()
    {
        rerollAvailable = false;
    }
    
    public void MoveRight()
    {
        if (currentPlayerId == clientId)
        {
            if (GameManager.Instance.MapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(0, 1);
            }
            else if (GameManager.Instance.MapManager.currentState == MapManager.States.Moving) MovePlayerRpc(0, 1);
        }
    }

    public void MoveLeft()
    {
        if (currentPlayerId == clientId)
        {
            if (GameManager.Instance.MapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(0, -1);
            }
            else if (GameManager.Instance.MapManager.currentState == MapManager.States.Moving) MovePlayerRpc(0, -1);
        }
    }

    public void MoveUp()
    {
        if (currentPlayerId == clientId)
        {
            if (GameManager.Instance.MapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(1, 0);
            }
            else if (GameManager.Instance.MapManager.currentState == MapManager.States.Moving) MovePlayerRpc(1, 0);
        }
    }

    public void MoveDown()
    {
        if (currentPlayerId == clientId)
        {
            if (GameManager.Instance.MapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(-1, 0);
            }
            else if (GameManager.Instance.MapManager.currentState == MapManager.States.Moving) MovePlayerRpc(-1, 0);
        }
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerRpc(int x, int y)
    {
        Vector2 currentPlayerPosition = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].position;
        int i = (int)currentPlayerPosition.x + x;
        int j = (int)currentPlayerPosition.y + y;

        if (InBounds(i, j) && GameManager.Instance.MapManager.map[i, j] != MapManager.Tiles.Wall
            && GameManager.Instance.MapManager.map[i, j] != MapManager.Tiles.CoveredEnd)
        {
            MapPlayer currentPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];
            currentPlayer.body.transform.SetParent(GameManager.Instance.MapManager.tiles[i, j].transform);
            currentPlayer.body.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            currentPlayer.SetPosition(new Vector2(i, j));

            CheckSceneChange(i, j);
            MapHelpers.CheckPosition(GameManager.Instance.MapManager.map, GameManager.Instance.MapManager.tiles, i, j);

            // MapAudioManager.playerMovementAudio.Play();

            GameManager.Instance.MapManager.moves--;
            mapUI.SetMovesText(GameManager.Instance.MapManager.moves);

            if (GameManager.Instance.MapManager.moves < 1)
            {
                EventManager.TriggerEvent("NextState");
            }
        }
    }

     bool InBounds(int i, int j)
    {
        if (i > -1 && i < mapHeight && j > -1 && j < mapWidth) return true;
        return false;
    }

    void CheckSceneChange(int x, int y)
    {
        if (GameManager.Instance.MapManager.map[x, y] == MapManager.Tiles.PeakedMinigame)
        {
            GameManager.Instance.MapManager.map[x, y] = MapManager.Tiles.ExploredMinigame;
            GameManager.Instance.MapManager.PlayMinigame();
        }
    }

   
    
    //////////////////////////////////////////////////////
    ///////////////////// FUNCTIONS //////////////////////
    //////////////////////////////////////////////////////

    public void SkipPlayer()
    {
        GameManager.Instance.MapManager.NextPlayer();
        mapUI.DisplayText(GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].name + " rolled a " + GameManager.Instance.MapManager.moves);
        mapUI.SetMovesText(GameManager.Instance.MapManager.moves);
    }
}
