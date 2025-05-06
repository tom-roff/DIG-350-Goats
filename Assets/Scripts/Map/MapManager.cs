using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class MapManager : NetworkBehaviour
{


    public ulong hostId;
    public GameObject[,] tiles;


    public MapPlayer[] players = null;
    public int currentPlayer = -1;
    public int moves;
    public bool playing;

    public enum States
    {
        None,
        Building,
        Preparing,
        Rolling,
        Moving,
        End
    }

    public States currentState = States.None;


    /*
        Old Map Syntax:
        -1 = wall
        0 = unexplored path
        1 = explored path
        2 = minigame
        3 = item
        4 = start
        5 = end
   */
    public enum Tiles
    {
        Wall,
        Unexplored,
        Peaked,
        Explored,
        UnexploredMinigame,
        PeakedMinigame,
        ExploredMinigame,
        UnexploredItem,
        PeakedItem,
        ExploredItem,
        Start,
        CoveredEnd,
        UncoveredEnd
    }

    public Tiles[,] map = {{ Tiles.Wall, Tiles.Wall, Tiles.Start,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.UnexploredMinigame, Tiles.Unexplored,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.UnexploredMinigame},
                  { Tiles.UnexploredMinigame, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.UnexploredMinigame,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Wall, Tiles.CoveredEnd,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall}};

    // public Tiles[,] map = {{ Tiles.Unexplored, Tiles.UnexploredItem, Tiles.Wall, Tiles.Start, Tiles.Wall, Tiles.UnexploredEnd, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored},
    //               { Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.UnexploredItem, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.UnexploredItem}};

    public List<string> minigameSceneNames = new List<string> { "MicrophoneMinigame", "LaserMinigame", "ClimbingMinigame", "LeversMinigame"};

    private int nextMinigame = -1;

    public override void OnNetworkSpawn()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        MapHelpers.Shuffle(minigameSceneNames);
    }

    public int MapWidth()
    {
        return map.GetLength(1);
    }

    public int MapHeight()
    {
        return map.GetLength(0);
    }


    public void NextPlayer()
    {
        if (currentPlayer + 1 == players.GetLength(0)) currentPlayer = 0;
        else currentPlayer++;

        // roll moves
        System.Random rnd = new System.Random();
        moves = rnd.Next(1, 7);
        // SceneManager.LoadScene("Test_DiceReroll", LoadSceneMode.Additive);
        // make sure it throws immediately
    }

    public void SendRoll(int roll)
    {
        moves = roll;
        SceneManager.UnloadSceneAsync("Test_DiceReroll");
    }


    public void TimedReturnToMap(float time = 5f)
    {
        Invoke("ReturnToMap", time);
    }

    void ReturnToMap()
    {
        MapSceneChangeRpc();
    }

    [Rpc(SendTo.Server)]
    public void MapSceneChangeRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }

    public void PlayMinigame()
    {
        nextMinigame++;
        if (nextMinigame == minigameSceneNames.Count - 1)
        {
            RevealEnd();
        }
        NetworkManager.Singleton.SceneManager.LoadScene(minigameSceneNames[nextMinigame], LoadSceneMode.Single);
        // NetworkManager.Singleton.SceneManager.LoadScene("MicrophoneMinigame", LoadSceneMode.Single);
    }

    void RevealEnd()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == Tiles.CoveredEnd)
                {
                    map[i, j] = Tiles.UncoveredEnd;
                    tiles[i,j].GetComponent<Image>().color = Color.red;
                }
            }
        }
    }
}
