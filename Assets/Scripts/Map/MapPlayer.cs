using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPlayer
{
    public ulong playerID { get; set; }
    public string name { get; set; }
    public PlayerInfo playerInfo { get; set; }
    public Vector2 position { get; set; }
    public GameObject body { get; set; }
    public Color32 color { get; set; }
    public int rerolls { get; set; }



    public MapPlayer(ulong playerID, PlayerInfo playerInfo)
    {
        this.playerID = playerID;
        this.playerInfo = playerInfo;

        name = playerInfo.playerName.ToString();
        this.position = Vector2.positiveInfinity;
        this.color = playerInfo.playerColor.colorRGB;
        rerolls = 0;
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public void SetBody(GameObject body)
    {
        this.body = body;
    }

    public void AddRerolls(int inc)
    {
        if (rerolls + inc < 0)
        {
            rerolls = 0;
            return;
        }
            
        rerolls += inc;
    }

    public void SetRerolls(int newRerolls)
    {
        rerolls = newRerolls;
    }
}