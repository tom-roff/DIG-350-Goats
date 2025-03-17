using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior_Map : MonoBehaviour
{
    // public Vector2 playerPosition; // will need some variation of this for multiplayer
    public bool playing = false;

    //[SerializeField] GameObject StartButton;



    public void StartMap()
    {
        if (MapReference.Instance.playerPosition.x == Vector2.positiveInfinity.x)
        {

            for (int i = 0; i < MapReference.Instance.mapHeight; i++)
            {
                for (int j = 0; j < MapReference.Instance.mapWidth; j++)
                {
                    if (MapReference.Instance.map[i, j] == MapReference.Tiles.Start)
                    {
                        MovePlayer(i, j);
                    }

                }
            }
        }
        else
        {
            MovePlayer((int)MapReference.Instance.playerPosition.x, (int)MapReference.Instance.playerPosition.y);
        }
        playing = true;
        //StartButton.SetActive(false);
    }

    void MovePlayer(float x, float y)
    {
        int i = (int)x;
        int j = (int)y;
        if (InBounds(i, j) && MapReference.Instance.map[i, j] != MapReference.Tiles.Wall)
        {
            this.transform.SetParent(MapReference.Instance.tiles[i, j].transform);
            MapReference.Instance.SetPlayerPosition(i, j);
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            MapReference.Instance.CheckPosition((int)MapReference.Instance.playerPosition.x, (int)MapReference.Instance.playerPosition.y);
        }
    }

    void Update()
    {
        if(playing)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(MapReference.Instance.playerPosition.x, MapReference.Instance.playerPosition.y+1);
            if(Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(MapReference.Instance.playerPosition.x, MapReference.Instance.playerPosition.y-1);
            if(Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(MapReference.Instance.playerPosition.x+1, MapReference.Instance.playerPosition.y);
            if(Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(MapReference.Instance.playerPosition.x-1, MapReference.Instance.playerPosition.y);
        }
        
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < MapReference.Instance.mapHeight && j > -1 && j < MapReference.Instance.mapWidth) return true;
        return false;
    }


}
