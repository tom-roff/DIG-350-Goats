using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MapManager : MonoBehaviour
{


    public ulong hostId;
    public GameObject[,] tiles;


    public MapPlayer[] players = null;
    public int currentPlayer = -1;
    public int moves;
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




    public int MapWidth()
    {
        return map.GetLength(1);
    }

    public int MapHeight()
    {
        return map.GetLength(0);
    }

    public void Play()
    {
        playing = true;
    }

    public void Pause()
    {
        playing = false;
    }

    public void NextPlayer()
    {
        if (moves < 1) // necessary when called from StartMap()
        {
            if (currentPlayer + 1 == players.GetLength(0)) currentPlayer = 0;
            else currentPlayer++;


            // roll moves
            System.Random rnd = new System.Random();
            moves = rnd.Next(1, 7);
            Debug.Log("Player " + players[currentPlayer].playerID + " rolled a " + moves);
        }
    }
    
    
}
