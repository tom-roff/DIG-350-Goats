using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior_Map : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public MapReference mapReference;
    int mapWidth;
    int mapHeight;
    public int playerX;
    public int playerY;
    public bool playing;
    [SerializeField] public GameObject startButton;
    public void OnStart()
    {
        mapWidth = mapReference.map.GetLength(1);
        mapHeight = mapReference.map.GetLength(0);
        FindStart();
        startButton.SetActive(false);
        playing = true;
    }

    void FindStart()
    {
        for(int j = 0; j < mapWidth; j++)
        {
            if(mapReference.map[0,j] == 4)
            {
                MovePlayer(0,j);
            }
            
        }
    }

    void MovePlayer(int i, int j)
    {
        if(InBounds(i,j) && mapReference.map[i, j] != -1)
        {
            this.transform.SetParent(mapReference.tiles[i,j].transform);
            playerX = i;
            playerY = j;
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            if(mapReference.map[i, j] == 0){
                mapReference.map[i, j] = 1;
                mapReference.tiles[i, j].GetComponent<Image>().color = Color.white;
            }
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
