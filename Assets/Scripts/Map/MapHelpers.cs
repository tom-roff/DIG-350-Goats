using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public static class MapHelpers
{
    public static void ChangeColor(MapManager.Tiles[,] map, GameObject[,] tiles, Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case MapManager.Tiles.Peaked:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.gray;
                break;
            case MapManager.Tiles.Explored:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.white;
                // MapAudioManager.stepAudio.Play();
                break;
            case MapManager.Tiles.PeakedMinigame:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(255, 0, 183);
                break;
            case MapManager.Tiles.ExploredMinigame:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(135, 0, 97);
                break;
            case MapManager.Tiles.PeakedItem:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.yellow;
                break;
            case MapManager.Tiles.ExploredItem:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = new Color(168, 107, 0);
                // MapAudioManager.collectItemAudio.Play();
                break;
            case MapManager.Tiles.Start:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.green;
                break;
            case MapManager.Tiles.PeakedEnd:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.red;
                break;
            default:
                tiles[(int)pos.x, (int)pos.y].GetComponent<Image>().color = Color.black;
                break;
        }
    }


    public static void ExploredCounter(MapManager.Tiles[,] map, GameObject[,] tiles)
    {
        int total = 0;
        int explored = 0;
        int map_discovered = 0;
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (tiles[i, j] != null)
                {
                    total += 1;
                }
                if (map[i, j].ToString().Contains("Explored"))
                {
                    explored += 1;
                }
            }
        }
        total -= 2; // end and start tiles
        map_discovered = explored / total;
        if (map_discovered == 1) Debug.Log("You discovered the whole map !!");

    }

    public static void CheckVisited(MapManager.Tiles[,] map, GameObject[,] tiles, Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case MapManager.Tiles.Peaked:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.Explored;
                break;
            case MapManager.Tiles.PeakedMinigame:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.ExploredMinigame;
                // NetworkManager.Singleton.SceneManager.LoadScene("TEST_MapMinigame", LoadSceneMode.Single);
                break;
            case MapManager.Tiles.PeakedItem:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.ExploredItem;
                Debug.Log("Item");
                break;
            default:
                break;
        }
        ChangeColor(map, tiles, pos);
    }

    public static void CheckSurrounding(MapManager.Tiles[,] map, GameObject[,] tiles, Vector2 pos)
    {
        switch (map[(int)pos.x, (int)pos.y])
        {
            case MapManager.Tiles.Unexplored:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.Peaked;
                break;
            case MapManager.Tiles.UnexploredMinigame:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.PeakedMinigame;
                break;
            case MapManager.Tiles.UnexploredItem:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.PeakedItem;
                break;
            case MapManager.Tiles.UnexploredEnd:
                map[(int)pos.x, (int)pos.y] = MapManager.Tiles.PeakedEnd;
                break;
            default:
                break;
        }
        ChangeColor(map, tiles, pos);
    }

    public static void CheckPosition(MapManager.Tiles[,] map, GameObject[,] tiles, int x, int y)
    {
        if (InBounds(map, x + 1, y) && tiles[x + 1, y] != null) CheckSurrounding(map, tiles, new Vector2(x + 1, y)); // above
        if (InBounds(map, x - 1, y) && tiles[x - 1, y] != null) CheckSurrounding(map, tiles, new Vector2(x - 1, y)); // below
        if (InBounds(map,x, y + 1) && tiles[x, y + 1] != null) CheckSurrounding(map, tiles, new Vector2(x, y + 1)); // left
        if (InBounds(map,x, y - 1) && tiles[x, y - 1] != null) CheckSurrounding(map, tiles, new Vector2(x, y - 1)); // left

        CheckVisited(map, tiles, new Vector2(x, y));
        ExploredCounter(map, tiles);
    }

    public static bool InBounds(MapManager.Tiles[,] map, int i, int j)
    {
        if (i > -1 && i < map.GetLength(0) && j > -1 && j < map.GetLength(1)) return true;
        return false;
    }

}