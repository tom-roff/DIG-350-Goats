using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;


public struct PlayerColor : INetworkSerializable{
    public colorEnumerator colorEnum;
    public Color32 colorRGB;

    public PlayerColor(colorEnumerator enumer, Color rgb, Material colorMat){
        colorEnum = enumer;
        colorRGB = rgb;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref colorEnum);
        serializer.SerializeValue(ref colorRGB);
    }
}


public enum colorEnumerator
{
    DarkBlue, DarkGreen, Fuchsia, Gold, LightBlue, LightPink, Lime, Red
};



public struct PlayerInfo : INetworkSerializable, IEquatable<PlayerInfo>{
    //Remember when you add a variable here to also add it to the NetworkSerialize function! This is necessary for online syncing.
    public FixedString32Bytes playerName;
    public PlayerColor playerColor;
    public int treasuresCollected;

    public PlayerInfo(String name, PlayerColor color, int treasures){

        playerName = name;
        playerColor = color;
        treasuresCollected = treasures;

    }

    public bool Equals(PlayerInfo other)
    {
        return playerName.Equals(other.playerName) && treasuresCollected == other.treasuresCollected;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerColor);
        serializer.SerializeValue(ref treasuresCollected);
    }
}

public static class Structs
{
    
}
