using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;


public enum PlayerColor
{
    DarkBlue, DarkGreen, Fuchsia, Gold, LightBlue, LightPink, Lime, Red
};

public struct PlayerInfo{
    public int playerIndex;
    public String playerName;
    public PlayerColor playerColor;
    public int treasuresCollected;


    public PlayerInfo(int index, String name, PlayerColor color, int treasures){
        playerIndex = index;
        playerName = name;
        playerColor = color;
        treasuresCollected = treasures;
    }

}

public class Structs
{
    
}
