using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior_Map : MonoBehaviour
{
    // public Vector2 playerPosition; // will need some variation of this for multiplayer
    public bool playing = false;



    public void StartMap()
    {
        if (MapManager.Instance.playerPosition.x == Vector2.positiveInfinity.x)
        {

            for (int i = 0; i < MapManager.Instance.mapHeight; i++)
            {
                for (int j = 0; j < MapManager.Instance.mapWidth; j++)
                {
                    if (MapManager.Instance.map[i, j] == MapManager.Tiles.Start)
                    {
                        MovePlayer(i, j);
                    }

                }
            }
        }
        else
        {
            MovePlayer((int)MapManager.Instance.playerPosition.x, (int)MapManager.Instance.playerPosition.y);
        }
        playing = true;
        //StartButton.SetActive(false);
    }

    void MovePlayer(float x, float y)
    {
        int i = (int)x;
        int j = (int)y;
        if (InBounds(i, j) && MapManager.Instance.map[i, j] != MapManager.Tiles.Wall)
        {
            this.transform.SetParent(MapManager.Instance.tiles[i, j].transform);
            MapManager.Instance.SetPlayerPosition(i, j);
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            MapHelpers.CheckPosition(MapManager.Instance.map, MapManager.Instance.tiles, (int)MapManager.Instance.playerPosition.x, (int)MapManager.Instance.playerPosition.y);
            MapAudioManager.playerMovementAudio.Play();
        }
    }

    public void TryMove(string direction)
    {
        switch (direction)
        {
            case "up":
                MovePlayer(MapManager.Instance.playerPosition.x + 1, MapManager.Instance.playerPosition.y);
                break;
            case "down":
                MovePlayer(MapManager.Instance.playerPosition.x - 1, MapManager.Instance.playerPosition.y);
                break;
            case "left":
                MovePlayer(MapManager.Instance.playerPosition.x, MapManager.Instance.playerPosition.y - 1);
                break;
            case "right":
                MovePlayer(MapManager.Instance.playerPosition.x, MapManager.Instance.playerPosition.y + 1);
                break;
        
        }
    }

    // void Update()
    // {
    //     if(playing)
    //     {
    //         if(Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(MapManager.Instance.playerPosition.x, MapManager.Instance.playerPosition.y+1);
    //         if(Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(MapManager.Instance.playerPosition.x, MapManager.Instance.playerPosition.y-1);
    //         if(Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(MapManager.Instance.playerPosition.x+1, MapManager.Instance.playerPosition.y);
    //         if(Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(MapManager.Instance.playerPosition.x-1, MapManager.Instance.playerPosition.y);
    //     }
        
    // }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < MapManager.Instance.mapHeight && j > -1 && j < MapManager.Instance.mapWidth) return true;
        return false;
    }


}
