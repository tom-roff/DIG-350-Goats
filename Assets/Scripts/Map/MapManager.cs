using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MapManager : MonoBehaviour
{

    private static MapManager _Instance;
    public static MapManager Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<MapManager>();
                // name it for easy recognition
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }


    public GameObject[,] tiles;
    public Vector2 playerPosition = Vector2.positiveInfinity;
    // list of players
    public int mapWidth;
    public int mapHeight;


    private Queue<MapPlayer> players;
    private MapPlayer currentPlayer;
    private int moves;
    public bool playing;


    /*
        Old Map Syntax:
        -1 = wall
        0 = unexplored path
        1 = explored path
        2 = minigame
        3 = item
        4 = start
        5 = end
        
        This may be changed later
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
        UnexploredEnd,
        PeakedEnd
    }

    public Tiles[,] map = {{ Tiles.Wall, Tiles.Wall, Tiles.Start,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored},
                  { Tiles.UnexploredItem, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Wall,Tiles.Wall,Tiles.Unexplored,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored,Tiles.Unexplored,Tiles.Unexplored,Tiles.UnexploredMinigame,Tiles.Wall,Tiles.Wall,Tiles.Wall},
                  { Tiles.Wall, Tiles.Wall, Tiles.UnexploredEnd,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall,Tiles.Wall}};

    // public Tiles[,] map = {{ Tiles.Unexplored, Tiles.UnexploredItem, Tiles.Wall, Tiles.Start, Tiles.Wall, Tiles.UnexploredEnd, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored},
    //               { Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.UnexploredItem, Tiles.Wall, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.Wall},
    //               { Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Unexplored, Tiles.Wall, Tiles.Wall, Tiles.Unexplored, Tiles.UnexploredItem}};



    void Awake()
    {
        mapWidth = map.GetLength(1);
        mapHeight = map.GetLength(0);
        // create player queue
    }

    public void Play()
    {
        playing = true;
    }

    public void Pause()
    {
        playing = false;
    }

    
    public void SetPlayerPosition(int x, int y)
    {
        playerPosition.x = x;
        playerPosition.y = y;
    }

    void Update()
    {
        if (playing) // && recieve information from player? will it get scrambled if they do it perfectly at the same time?
        {
            // if playerID == currentPlayer
            // try move, successful moves-- && set MapPlayer position etc...
            // else do nothing 

            // if moves == 0
            // requeue currentPlayer, dequeue next player into currentPlayer
            // roll dice mechanism? 
        }
    }



}
