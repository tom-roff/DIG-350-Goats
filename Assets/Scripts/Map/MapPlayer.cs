using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPlayer
{
    public string playerID { get; set; }
    public Vector2 position { get; set; }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }
}