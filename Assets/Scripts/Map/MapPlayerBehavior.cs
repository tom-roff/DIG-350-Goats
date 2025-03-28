using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class MapPlayerBehavior : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;

    void OnEnable()
    {
        // if (MapManager.Instance.players == null)
        // {
        //     CreatePlayerQueue();
        // }
        // else
        // {
        //     foreach (var player in MapManager.Instance.players)
        //     {
        //         InstantiatePlayer(player.position, player);
        //     }
        // }
    }

    void CreatePlayerQueue()
    {
        MapManager.Instance.players = new MapPlayer[2]; // replace with # of players in lobby
        for (int i = 0; i < 2; i++)
        {
            MapManager.Instance.players[i] = new MapPlayer(i.ToString());
        }
    }


    public void StartMap()
    {
        if (MapManager.Instance.players == null)
        {
            CreatePlayerQueue();
        }
        else
        {
            foreach (var player in MapManager.Instance.players)
            {
                InstantiatePlayer(player.position, player);
            }
        }
        if (MapManager.Instance.currentPlayer == -1)
        {
            SpawnPlayers(FindStartPosition());
        }
        MapManager.Instance.Play();
        MapManager.Instance.NextPlayer();
    }

    private Vector2 FindStartPosition()
    {
        for (int i = 0; i < MapManager.Instance.mapHeight; i++)
        {
            for (int j = 0; j < MapManager.Instance.mapWidth; j++)
            {
                if (MapManager.Instance.map[i, j] == MapManager.Tiles.Start)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.positiveInfinity;
    }

    void SpawnPlayers(Vector2 startPosition)
    {
        foreach (var player in MapManager.Instance.players)
        {
            InstantiatePlayer(startPosition, player);
        }
    }

    

    void InstantiatePlayer(Vector2 startPosition, MapPlayer player)
    {
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerInstance.name = "player " + player.playerID;
        playerInstance.transform.SetParent(MapManager.Instance.tiles[(int)startPosition.x, (int)startPosition.y].transform);
        playerInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerInstance.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        player.SetBody(playerInstance);
        player.SetPosition(startPosition);
        MapHelpers.CheckPosition(MapManager.Instance.map, MapManager.Instance.tiles, (int)startPosition.x, (int)startPosition.y);
    }

    void MovePlayer(float x, float y)
    {
        int i = (int)x;
        int j = (int)y;
        if (InBounds(i, j) && MapManager.Instance.map[i, j] != MapManager.Tiles.Wall)
        {
            int currentPlayer = MapManager.Instance.currentPlayer;
            MapManager.Instance.players[currentPlayer].body.transform.SetParent(MapManager.Instance.tiles[i, j].transform);
            MapManager.Instance.players[currentPlayer].body.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            MapManager.Instance.players[currentPlayer].SetPosition(new Vector2(i, j));

            MapHelpers.CheckPosition(MapManager.Instance.map, MapManager.Instance.tiles, i, j);
            // MapAudioManager.playerMovementAudio.Play();
            MapManager.Instance.moves--;
        }
    }

    
    void Update()
    {
        if (MapManager.Instance.playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
        {
            // if playerID == currentPlayer
            // try move, successful moves-- && set MapPlayer position etc...
            // else do nothing 

            MapPlayer currentPlayer = MapManager.Instance.players[MapManager.Instance.currentPlayer];

            if (currentPlayer.playerID == "0")
            {
                if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(currentPlayer.position.x, currentPlayer.position.y + 1);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(currentPlayer.position.x, currentPlayer.position.y - 1);
                if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(currentPlayer.position.x + 1, currentPlayer.position.y);
                if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(currentPlayer.position.x - 1, currentPlayer.position.y);
            }
            else if (currentPlayer.playerID == "1")
            {
                if (Input.GetKeyDown("d")) MovePlayer(currentPlayer.position.x, currentPlayer.position.y + 1);
                if (Input.GetKeyDown("a")) MovePlayer(currentPlayer.position.x, currentPlayer.position.y - 1);
                if (Input.GetKeyDown("w")) MovePlayer(currentPlayer.position.x + 1, currentPlayer.position.y);
                if (Input.GetKeyDown("s")) MovePlayer(currentPlayer.position.x - 1, currentPlayer.position.y);
            }

            // if moves == 0
            // requeue currentPlayer, dequeue next player into currentPlayer
            // roll dice mechanism? 

            if (MapManager.Instance.moves < 1)
            {
                MapManager.Instance.NextPlayer();
            }
        }
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < MapManager.Instance.mapHeight && j > -1 && j < MapManager.Instance.mapWidth) return true;
        return false;
    }


}
