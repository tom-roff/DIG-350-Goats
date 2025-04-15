using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPlayer
{
    public ulong playerID { get; set; }
    public Vector2 position { get; set; }
    public GameObject body { get; set; }
    public Color32 color { get; set; }


    public MapPlayer(ulong playerID)
    {
        this.playerID = playerID;
        this.position = Vector2.positiveInfinity;
    }

    public MapPlayer(ulong playerID, Color32 color)
    {
        this.playerID = playerID;
        this.position = Vector2.positiveInfinity;
        this.color = color;
    }

    public MapPlayer(ulong playerID, Vector2 position)
    {
        this.playerID = playerID;
        this.position = position;
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public void SetBody(GameObject body)
    {
        this.body = body;
    }
}