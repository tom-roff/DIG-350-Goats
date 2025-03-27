using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPlayer
{
    public string playerID { get; set; }
    public Vector2 position { get; set; }
    public GameObject body { get; set; }


    public MapPlayer(string playerID)
    {
        this.playerID = playerID;
        this.position = Vector2.positiveInfinity;
    }

    public MapPlayer(string playerID, Vector2 position)
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