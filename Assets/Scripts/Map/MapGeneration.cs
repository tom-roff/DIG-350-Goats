using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour
{
    float tileWidth;
    float tileHeight;
    [SerializeField] public float xMargin = .1f;
    [SerializeField] public float yMargin = .1f;
    int mapWidth;
    int mapHeight;
    [SerializeField] public Canvas mapCanvas;
    [SerializeField] public GameObject tilePrefab;
    GameObject[,] tiles;


   /*
   Map Syntax:
   -1 = wall
   0 = unexplored path
   1 = explored path
   2 = minigame
   3 = item
   4 = start
   5 = end
   This may be changed later
   */

    // int[,] map = {{ -1, -1, 5},
    //               { -1, 0, 0},    
    //               { 0, 0, -1},    
    //               { -1, 0, 0},    
    //               { -1, -1, 4}};

    int[,] map = {{ -1, -1, 4,-1,-1},
                  { -1, 0, 0,-1,0},    
                  { -1, 0, -1,-1,0},    
                  { -1, 0, 0,0,0},    
                  { -1, -1, 5,-1,-1}};
    
    void OnValidate()
    {
        mapWidth = map.GetLength(1);
        mapHeight = map.GetLength(0);
        // Debug.Log("Width: " + mapWidth + " Height: " + mapHeight);
        tileWidth = (1-(xMargin*2)) / mapWidth;
        tileHeight = (1-(yMargin*2)) / mapHeight;
        //Debug.Log("Tile width: " + tileWidth + " Tile height: " + tileHeight);
    }
    void Start()
    {
        tiles = new GameObject[mapHeight, mapWidth];
        GenerateMap();
    }

    void GenerateMap() 
    {
        for(int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++) 
            {
                if(map[i,j] >= 0)
                {
                    float xStart = xMargin + (j*tileWidth);
                    float yStart = yMargin + (i*tileHeight);

                    GameObject tileInstance = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    tileInstance.transform.SetParent(mapCanvas.transform); 
                    tileInstance.GetComponent<RectTransform>().anchorMin = new Vector2(xStart, yStart);
                    tileInstance.GetComponent<RectTransform>().anchorMax = new Vector2(xStart + tileWidth, yStart + tileHeight);
                    tileInstance.GetComponent<RectTransform>().offsetMin = new Vector2(0,0);
                    tileInstance.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
                    tileInstance.name = i + ", " + j;
                    tiles[i,j] = tileInstance;

                    switch(map[i,j]) 
                    {
                    case 0:
                        tileInstance.GetComponent<Image>().color = Color.gray;
                        break;
                    case 4:
                        tileInstance.GetComponent<Image>().color = Color.green;
                        break;
                    default:
                        tileInstance.GetComponent<Image>().color = Color.red;
                        break;
                    }
                }
            }
        }
    }

}
