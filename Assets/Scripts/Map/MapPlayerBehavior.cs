using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class MapPlayerBehavior : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject hostUI;
    [SerializeField] public GameObject playerUI;
    [SerializeField] public MapUI mapUI;
    [SerializeField] public GameObject playerBackground;
    [SerializeField] public TMP_Text playerName;
    [SerializeField] public GameObject tilesObj;
    [SerializeField] public GameObject skipObj;
    MapManager mapManager;
    OurNetwork ourNetwork;

    [Header("Status")]
    public PlayerUIEntry[] leaderboardEntries = new PlayerUIEntry[6];

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
        mapManager = GameManager.Instance.MapManager;
        ourNetwork = GameManager.Instance.OurNetwork; ;
        mapWidth = mapManager.MapWidth();
        mapHeight = mapManager.MapHeight();

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
        SetLeaderboard();
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void SetLeaderboard() {

        // for (int i = 1; i < ourNetwork.playerInfoList.Count; i++) {
        //     leaderboardEntries[i - 1].gameObject.SetActive(true);
        //     //...this code might be inefficient.
        //     leaderboardEntries[i - 1].SetNameAndColorAndPoints(ourNetwork.playerInfoList[i].playerName.ToString(), ourNetwork.playerInfoList[i].playerColor, ourNetwork.playerInfoList[i].treasuresCollected);
        // }

        // ^ this version leaves dummy text out when there are less than 6
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            if (i < ourNetwork.playerInfoList.Count -1 )
            {
                leaderboardEntries[i].gameObject.SetActive(true);
                //...this code might be inefficient.
                leaderboardEntries[i].SetNameAndColorAndPoints(
                    ourNetwork.playerInfoList[i + 1].playerName.ToString(),
                    ourNetwork.playerInfoList[i + 1].playerColor,
                    ourNetwork.playerInfoList[i + 1].treasuresCollected
                );
            }
            else
            {
                leaderboardEntries[i].gameObject.SetActive(false);
            }
        }
    }

    void CheckHost()
    {
        clientId = NetworkManager.Singleton.LocalClientId;
        if (clientId == mapManager.hostId) // main screen
        {
            playerUI.SetActive(false);
            host = true;
        }
        else
        {
            hostUI.SetActive(false);
            playerBackground.GetComponent<Image>().color = ourNetwork.playerInfoList[(int)clientId].playerColor.colorRGB;
            playerName.text = ourNetwork.playerInfoList[(int)clientId].playerName.ToString();
            GameObject.Find("Leaderboard_Responsive").SetActive(false);
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

        if (mapManager.players == null) CreatePlayerQueue();
        SpawnPlayers();
        EventManager.TriggerEvent("NextState");
    }

    void CreatePlayerQueue()
    {
        int playerCount = ourNetwork.playerInfoList.Count;
        mapManager.players = new MapPlayer[playerCount - 1]; // replace with # of players in lobby

        int i = 0;
        foreach (PlayerInfo entry in ourNetwork.playerInfoList)
        {
            if (i != 0)
                mapManager.players[i - 1] = new MapPlayer((ulong)i, entry);

            i++;
        }

        Debug.Log("Players in queue: " + mapManager.players.Length);
    }

    void SpawnPlayers()
    {
        if (mapManager.currentPlayer == -1)
        {
            Vector2 startPosition = FindStartPosition();
            foreach (var player in mapManager.players)
            {
                InstantiatePlayer(startPosition, player);
            }
        }
        else
        {
            foreach (var player in mapManager.players)
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
                if (mapManager.map[i, j] == MapManager.Tiles.Start)
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
        playerInstance.transform.SetParent(mapManager.tiles[(int)startPosition.x, (int)startPosition.y].transform);
        playerInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerInstance.GetComponent<Image>().color = player.color;
        playerInstance.GetComponentInChildren<TMP_Text>().text = player.name.Substring(0, 1);
        player.body = playerInstance;
        player.SetPosition(startPosition);
        MapHelpers.CheckPosition(mapManager.map, mapManager.tiles, (int)startPosition.x, (int)startPosition.y);
        ArrangePlayersOnTile((int)startPosition.x, (int)startPosition.y);
    }



    //////////////////////////////////////////////////////
    ////////////////////// ROLLING ///////////////////////
    ////////////////////////////////////////////////////// 
    public void Rolling()
    {
        if (!host) return;
        if (mapManager.moves > 0)
        {
            ResumeTurn();
            return;
        }

        mapManager.NextPlayer();
        mapUI.DisplayText(mapManager.players[mapManager.currentPlayer].name + " rolled a " + mapManager.moves);
        mapUI.SetMovesText(mapManager.moves);
        SendCurrentPlayerRpc(mapManager.players[mapManager.currentPlayer].playerID);
        CheckRerollsRpc(mapManager.players[mapManager.currentPlayer].rerolls);
    }

    void ResumeTurn()
    {
        mapUI.DisplayText(mapManager.players[mapManager.currentPlayer].name + " has " + mapManager.moves + " remaining moves");
        mapUI.SetMovesText(mapManager.moves);
        SendCurrentPlayerRpc(mapManager.players[mapManager.currentPlayer].playerID);
        SendRerolls();
        Moving();
        mapManager.currentState = MapManager.States.Moving;
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
        int nextMoves = rnd.Next(1, 7);
        MapPlayer currentPlayer = mapManager.players[mapManager.currentPlayer];
        currentPlayer.AddRerolls(-1);
        if (nextMoves > mapManager.moves)
        {
            mapManager.moves = nextMoves;
            mapUI.DisplayText("Rerolled moves: " + mapManager.moves);
            mapUI.SetMovesText(mapManager.moves);
        }
        else
        {
            mapUI.DisplayText("Rerolled moves: " + nextMoves + " keeping " + mapManager.moves);
        }
        

        CheckRerollsRpc(currentPlayer.rerolls);
    }

    public void Reroll()
    {
        if (!rerollAvailable) return;
        RerollRpc();
    }

    public void SendRerolls()
    {
        for (int i = 0; i < mapManager.players.Length; i++)
        {
            SendRerollToClientRpc(i + 1, mapManager.players[i].rerolls);
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
        DisableRerollingRpc();
    }

    [Rpc(SendTo.NotServer)]
    private void DisableRerollingRpc()
    {
        rerollAvailable = false;
        mapUI.DisableRerolling();
    }

    public void MoveRight()
    {
        if (currentPlayerId == clientId)
        {
            if (mapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(0, 1);
            }
            else if (mapManager.currentState == MapManager.States.Moving) MovePlayerRpc(0, 1);
        }
    }

    public void MoveLeft()
    {
        if (currentPlayerId == clientId)
        {
            if (mapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(0, -1);
            }
            else if (mapManager.currentState == MapManager.States.Moving) MovePlayerRpc(0, -1);
        }
    }

    public void MoveUp()
    {
        if (currentPlayerId == clientId)
        {
            if (mapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(1, 0);
            }
            else if (mapManager.currentState == MapManager.States.Moving) MovePlayerRpc(1, 0);
        }
    }

    public void MoveDown()
    {
        if (currentPlayerId == clientId)
        {
            if (mapManager.currentState == MapManager.States.Rolling)
            {
                EventManager.TriggerEvent("NextState");
                MovePlayerRpc(-1, 0);
            }
            else if (mapManager.currentState == MapManager.States.Moving) MovePlayerRpc(-1, 0);
        }
    }

    [Rpc(SendTo.Server)]
    public void MovePlayerRpc(int x, int y)
    {
        Vector2 currentPlayerPosition = mapManager.players[mapManager.currentPlayer].position;
        int i = (int)currentPlayerPosition.x + x;
        int j = (int)currentPlayerPosition.y + y;
        int originalX = (int)currentPlayerPosition.x;
        int originalY = (int)currentPlayerPosition.y;

        if (InBounds(i, j) && mapManager.map[i, j] != MapManager.Tiles.Wall
            && mapManager.map[i, j] != MapManager.Tiles.CoveredEnd)
        {
            MapPlayer currentPlayer = mapManager.players[mapManager.currentPlayer];
            currentPlayer.body.transform.SetParent(mapManager.tiles[i, j].transform);
            currentPlayer.body.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            currentPlayer.SetPosition(new Vector2(i, j));
            ArrangePlayersOnTile(i, j);
            ArrangePlayersOnTile(originalX, originalY);

            CheckSceneChange(i, j);
            MapHelpers.CheckPosition(mapManager.map, mapManager.tiles, i, j);

            // MapAudioManager.playerMovementAudio.Play();

            mapManager.moves--;
            mapUI.SetMovesText(mapManager.moves);

            if (mapManager.moves < 1)
            {
                mapManager.currentState = MapManager.States.Rolling;
                Rolling();

                // EventManager.TriggerEvent("NextState");
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
        if (mapManager.map[x, y] == MapManager.Tiles.PeakedMinigame)
        {
            mapManager.map[x, y] = MapManager.Tiles.ExploredMinigame;
            mapManager.PlayMinigame();
        }
        // For ending the game
        if (mapManager.map[x, y] == MapManager.Tiles.UncoveredEnd)
        {
            EndGame();
        }
    }

    public void EndGame()
        {

            int pointsToGive = 5;

            OurNetwork ourNetwork = GameManager.Instance.OurNetwork;
            ourNetwork.SetPlayerScoreRpc(mapManager.currentPlayer, ourNetwork.playerInfoList[mapManager.currentPlayer].treasuresCollected + pointsToGive);

            int topScore = 0;
            string victorName = "";
            for(int i = 1; i < ourNetwork.playerInfoList.Count; i++){
                if(ourNetwork.playerInfoList[i].treasuresCollected > topScore){
                    topScore = ourNetwork.playerInfoList[i].treasuresCollected;
                    victorName = ourNetwork.playerInfoList[i].playerName.ToString();
                }
            }
            SetLeaderboard();
            mapUI.displayText.text = victorName + " is the winner!";
            tilesObj.SetActive(false);
            mapUI.rerollText.gameObject.SetActive(false);
            skipObj.gameObject.SetActive(false);

        }

    void ArrangePlayersOnTile(int x, int y) // x and y is tile position
    {
        int playerCount = mapManager.players.Length;
        int playersOnTile = 0;
        int[] playersOnTileRefs = new int[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            if (mapManager.players[i].position == new Vector2(x, y))
            {
                playersOnTileRefs[playersOnTile] = i;
                playersOnTile++;
            }
        }

        float spacing = 1f / playersOnTile;

        for (int i = 0; i < playersOnTile; i++)
        {
            mapManager.players[playersOnTileRefs[i]].body.GetComponent<RectTransform>().anchorMin = new Vector2((spacing*((float)i+1f))-((spacing/2f)), .5f);
            mapManager.players[playersOnTileRefs[i]].body.GetComponent<RectTransform>().anchorMax = new Vector2((spacing*((float)i+1f))-((spacing/2f)), .5f);
        }
    }



    //////////////////////////////////////////////////////
    ///////////////////// FUNCTIONS //////////////////////
    //////////////////////////////////////////////////////

    public void SkipPlayer()
    {
        mapManager.NextPlayer();
        mapUI.DisplayText(mapManager.players[mapManager.currentPlayer].name + " rolled a " + mapManager.moves);
        mapUI.SetMovesText(mapManager.moves);
        SendCurrentPlayerRpc(mapManager.players[mapManager.currentPlayer].playerID);
        CheckRerollsRpc(mapManager.players[mapManager.currentPlayer].rerolls);
    }
}
