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
    MapManager mapManager;

    [Header("References")]
    [SerializeField] public GameObject tileParent;
    [SerializeField] public GameObject tilePrefab;

    [Header("Margins")]
    [SerializeField] public float xMargin = .1f;
    [SerializeField] public float yMargin = .1f;


    void OnEnable()
    {
        mapManager = GameManager.Instance.MapManager;
        mapWidth = mapManager.MapWidth();
        mapHeight = mapManager.MapHeight();
        tileWidth = (1 - (xMargin * 2)) / mapWidth;
        tileHeight = (1 - (yMargin * 2)) / mapHeight;

        mapManager.tiles = new GameObject[mapHeight, mapWidth];
        EventManager.StartListening("Building", Building);
    }

    void OnDisable()
    {
        EventManager.StopListening("Building", Building);
    }

    public void Building()
    {
        Debug.Log("Building");
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (mapManager.map[i, j] != MapManager.Tiles.Wall)
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
                    mapManager.tiles[i, j] = tileInstance;

                    MapHelpers.ChangeColor(mapManager.map, mapManager.tiles, new Vector2(i, j));
                }
            }
        }
        EventManager.TriggerEvent("NextState");
    }

    
    

}
