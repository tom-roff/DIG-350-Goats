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


   
void CreatePlayerQueue()
    {
        GameManager.Instance.MapManager.players = new MapPlayer[NetworkManager.Singleton.ConnectedClientsIds.Count]; // replace with # of players in lobby
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
        {
            if (NetworkManager.Singleton.ConnectedClientsIds[i] != NetworkManager.Singleton.LocalClientId)
            {
                GameManager.Instance.MapManager.players[i] = new MapPlayer(NetworkManager.Singleton.ConnectedClientsIds[i]);
                Debug.Log(NetworkManager.Singleton.ConnectedClientsIds[i]);
            }
        }
    }

    public void StartMap()
    {
        CreatePlayerQueue();
        if (GameManager.Instance.MapManager.currentPlayer == -1)
        {
            SpawnPlayers(FindStartPosition());
        }
        else
        {
            foreach (var player in GameManager.Instance.MapManager.players)
            {
                InstantiatePlayer(player.position, player);
            }
        }
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

    void SpawnPlayers(Vector2 startPosition)
    {
        foreach (var player in GameManager.Instance.MapManager.players)
        {
            InstantiatePlayer(startPosition, player);
        }
    }

    

    void InstantiatePlayer(Vector2 startPosition, MapPlayer player)
    {
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // playerInstance.name = "player " + player.playerID;
        playerInstance.transform.SetParent(GameManager.Instance.MapManager.tiles[(int)startPosition.x, (int)startPosition.y].transform);
        playerInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        playerInstance.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        player.SetBody(playerInstance);
        player.SetPosition(startPosition);
        MapHelpers.CheckPosition(GameManager.Instance.MapManager.map, GameManager.Instance.MapManager.tiles, (int)startPosition.x, (int)startPosition.y);
    }

    public static void MovePlayer(float x, float y)
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
            GameManager.Instance.MapManager.moves--;
        }
    }

    
    // void Update()
    // {
    //     if (GameManager.Instance.MapManager.playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
    //     {
    //         // if playerID == currentPlayer
    //         // try move, successful moves-- && set MapPlayer position etc...
    //         // else do nothing 

    //         MapPlayer currentPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];

    //         if (currentPlayer.playerID == "0")
    //         {
    //             if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(currentPlayer.position.x, currentPlayer.position.y + 1);
    //             if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(currentPlayer.position.x, currentPlayer.position.y - 1);
    //             if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(currentPlayer.position.x + 1, currentPlayer.position.y);
    //             if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(currentPlayer.position.x - 1, currentPlayer.position.y);
    //         }
    //         else if (currentPlayer.playerID == "1")
    //         {
    //             if (Input.GetKeyDown("d")) MovePlayer(currentPlayer.position.x, currentPlayer.position.y + 1);
    //             if (Input.GetKeyDown("a")) MovePlayer(currentPlayer.position.x, currentPlayer.position.y - 1);
    //             if (Input.GetKeyDown("w")) MovePlayer(currentPlayer.position.x + 1, currentPlayer.position.y);
    //             if (Input.GetKeyDown("s")) MovePlayer(currentPlayer.position.x - 1, currentPlayer.position.y);
    //         }

    //         // if moves == 0
    //         // requeue currentPlayer, dequeue next player into currentPlayer
    //         // roll dice mechanism? 

    //         if (GameManager.Instance.MapManager.moves < 1)
    //         {
    //             GameManager.Instance.MapManager.NextPlayer();
    //         }
    //     }
    // }

    static bool InBounds(int i, int j)
    {
        if(i > -1 && i < GameManager.Instance.MapManager.mapHeight && j > -1 && j < GameManager.Instance.MapManager.mapWidth) return true;
        return false;
    }


}
