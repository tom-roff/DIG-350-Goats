using UnityEngine;
using System;
using Unity.Netcode;



public class MultiplayerMapControls : NetworkBehaviour
{
    public OurNetwork network;
    public PlayerBehavior_Map player;

    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>();
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
        }
        player = FindFirstObjectByType<PlayerBehavior_Map>();
    }

    void Update()
    {
        if (IsClient)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) TryMoveRpc("right");
            if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMoveRpc("left");
            if (Input.GetKeyDown(KeyCode.UpArrow)) TryMoveRpc("up");
            if (Input.GetKeyDown(KeyCode.DownArrow)) TryMoveRpc("down");
        }
        

    }



    [Rpc(SendTo.Server)]
    void TryMoveRpc(string direction)
    {
        Debug.Log("tried move");


        MoveRpc(direction);
    }
    
    [Rpc(SendTo.NotServer)]
    void MoveRpc(string direction)
    {
        player.TryMove(direction);
    }
}
