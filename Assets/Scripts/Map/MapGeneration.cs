using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class MapGeneration : MonoBehaviour
{
    float tileWidth;
    float tileHeight;
    int mapWidth;
    int mapHeight;

    [Header("References")]
    [SerializeField] public GameObject tileParent;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject player;

    [Header("Margins")]
    [SerializeField] public float xMargin = .1f;
    [SerializeField] public float yMargin = .1f;


    void OnEnable()
    {
        mapWidth = MapReference.Instance.map.GetLength(1);
        mapHeight = MapReference.Instance.map.GetLength(0);
        tileWidth = (1-(xMargin*2)) / mapWidth;
        tileHeight = (1-(yMargin*2)) / mapHeight;

        MapReference.Instance.tiles = new GameObject[mapHeight, mapWidth];

        GenerateMap();
    }

    void GenerateMap()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (MapReference.Instance.map[i, j] != MapReference.Tiles.Wall)
                {
                    float xStart = xMargin + (j * tileWidth);
                    float yStart = yMargin + (i * tileHeight);

                    GameObject tileInstance = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    tileInstance.transform.SetParent(tileParent.transform);
                    tileInstance.GetComponent<RectTransform>().anchorMin = new Vector2(xStart, yStart);
                    tileInstance.GetComponent<RectTransform>().anchorMax = new Vector2(xStart + tileWidth, yStart + tileHeight);
                    tileInstance.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                    tileInstance.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
                    tileInstance.name = i + ", " + j;
                    MapReference.Instance.tiles[i, j] = tileInstance;

                    MapReference.Instance.ChangeColor(new Vector2(i, j));
                }
            }
        }
        player.GetComponent<PlayerBehavior_Map>().StartMap();
    }

    
    

}
