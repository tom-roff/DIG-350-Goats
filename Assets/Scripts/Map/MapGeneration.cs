using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class MapGeneration : MonoBehaviour
{
    float tileWidth;
    float tileHeight;

    [Header("References")]
    [SerializeField] public GameObject tileParent;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public MapPlayerBehavior mapPlayerBehavior;

    [Header("Margins")]
    [SerializeField] public float xMargin = .1f;
    [SerializeField] public float yMargin = .1f;


    void OnEnable()
    {
        tileWidth = (1-(xMargin*2)) / MapManager.Instance.mapWidth;
        tileHeight = (1-(yMargin*2)) / MapManager.Instance.mapHeight;

        MapManager.Instance.tiles = new GameObject[MapManager.Instance.mapHeight, MapManager.Instance.mapWidth];

        GenerateMap();
    }

    void GenerateMap()
    {
        for (int i = 0; i < MapManager.Instance.mapHeight; i++)
        {
            for (int j = 0; j < MapManager.Instance.mapWidth; j++)
            {
                if (MapManager.Instance.map[i, j] != MapManager.Tiles.Wall)
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
                    MapManager.Instance.tiles[i, j] = tileInstance;

                    MapHelpers.ChangeColor(MapManager.Instance.map, MapManager.Instance.tiles, new Vector2(i, j));
                }
            }
        }
        mapPlayerBehavior.StartMap();
    }

    
    

}
