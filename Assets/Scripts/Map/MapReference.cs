using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapReference : MonoBehaviour
{

    private static MapReference _Instance;
    public static MapReference Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<MapReference>();
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
    public int mapWidth;
    public int mapHeight;


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
    }

    public void ChangeColor(Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case Tiles.Peaked:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.gray;
                break;
            case Tiles.Explored:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.white;
                break;
            case Tiles.PeakedMinigame:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(255, 0, 183);
                break;
            case Tiles.ExploredMinigame:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(135, 0, 97);
                break;
            case Tiles.PeakedItem:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.yellow;
                break;
            case Tiles.ExploredItem:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(168, 107, 0);
                break;
            case Tiles.Start:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.green;
                break;
            case Tiles.PeakedEnd:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.red;
                break;
            default:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.black;
                break;
        }
    }


    // double ExploredCounter(int[,] map)
    // {
    //     int total = 0;
    //     int explored = 0;
    //     int map_discovered = 0;
    //     for (int i = 0; i < map.GetLength(1); i++)
    //     {
    //         for (int j = 0; j < map.GetLength(0); j++)
    //         {
    //             if (map[i, j] == Tiles.Unexplored || map[i, j] == Tiles.Explored || map[i,j] == Tiles.Peaked)
    //             {
    //                 total += 1;
    //             }
    //             if (map[i, j] == Tiles.Explored)
    //             {
    //                 explored += 1;
    //             }
    //         }
    //     }
    //     map_discovered = explored / total;
    //     if (map_discovered == 1)
    //     {
    //         Debug.Log("You discovered the whole map !!");
    //     }
    //     return map_discovered;
    // }

    void CheckVisited(Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case Tiles.Peaked:
                map[(int)pos.x, (int)pos.y] = Tiles.Explored;
                break;
            case Tiles.PeakedMinigame:
                map[(int)pos.x, (int)pos.y] = Tiles.ExploredMinigame;
                ChangeColor(pos);
                SceneManager.LoadScene("TEST_MapMinigame");
                break;
            case Tiles.PeakedItem:
                map[(int)pos.x, (int)pos.y] = Tiles.ExploredItem;
                ChangeColor(pos);
                Debug.Log("Item");
                break;
            default:
                break;
        }
        ChangeColor(pos);
    }

    void CheckSurrounding(Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case Tiles.Unexplored:
                map[(int)pos.x, (int)pos.y] = Tiles.Peaked;
                break;
            case Tiles.UnexploredMinigame:
                map[(int)pos.x, (int)pos.y] = Tiles.PeakedMinigame;
                break;
            case Tiles.UnexploredItem:
                map[(int)pos.x, (int)pos.y] = Tiles.PeakedItem;
                break;
            case Tiles.UnexploredEnd:
                map[(int)pos.x, (int)pos.y] = Tiles.PeakedEnd;
                break;
            default:
                break;
        }
        ChangeColor(pos);
    }

    public void CheckPosition(int x, int y)
    {
        if (InBounds(x + 1, y) && tiles[x + 1, y] != null) CheckSurrounding(new Vector2(x + 1, y)); // above
        if (InBounds(x - 1, y) && tiles[x - 1, y] != null) CheckSurrounding(new Vector2(x - 1, y)); // below
        if (InBounds(x, y + 1) && tiles[x, y + 1] != null) CheckSurrounding(new Vector2(x, y + 1)); // left
        if (InBounds(x, y - 1) && tiles[x, y - 1] != null) CheckSurrounding(new Vector2(x, y - 1)); // left

        CheckVisited(new Vector2(x, y));
    }

    bool InBounds(int i, int j)
    {
        if (i > -1 && i < map.GetLength(0) && j > -1 && j < map.GetLength(1)) return true;
        return false;
    }

    public void SetPlayerPosition(int x, int y)
    {
        playerPosition.x = x;
        playerPosition.y = y;
    }

}
