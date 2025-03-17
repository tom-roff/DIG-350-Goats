using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior_Map : MonoBehaviour
{
    [Header("References")]
    int mapWidth;
    int mapHeight;
    public int playerX;
    public int playerY;
    public bool playing;
    [SerializeField] public GameObject startButton;
    public void OnStart()
    {
        mapWidth = MapReference.Instance.map.GetLength(1);
        mapHeight = MapReference.Instance.map.GetLength(0);
        FindStart();
        startButton.SetActive(false);
        playing = true;
    }

    void FindStart()
    {
        if(MapReference.Instance.playerPosition.x == Vector2.positiveInfinity.x)
        {
            
            for(int j = 0; j < mapWidth; j++)
            {
                if(MapReference.Instance.map[0,j] == 4)
                {
                    MovePlayer(0,j);
                }
                
            }
        }
        else
        {
            MovePlayer((int)MapReference.Instance.playerPosition.x, (int)MapReference.Instance.playerPosition.y);
        }
        
    }

    void MovePlayer(int i, int j)
    {
        if(InBounds(i,j) && MapReference.Instance.map[i, j] != -1)
        {
            this.transform.SetParent(MapReference.Instance.tiles[i,j].transform);
            playerX = i;
            playerY = j;
            MapReference.Instance.SetPlayerPosition(i,j);
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            MapReference.Instance.CheckPosition(playerX, playerY);
        }
    }

    void Update()
    {
        if(playing)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(playerX, playerY+1);
            if(Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(playerX, playerY-1);
            if(Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(playerX+1, playerY);
            if(Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(playerX-1, playerY);
        }
        
    }

    bool InBounds(int i, int j)
    {
        if(i > -1 && i < mapHeight && j > -1 && j < mapWidth) return true;
        return false;
    }


}
