using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;



public class MultiplayerMapControls : NetworkBehaviour
{
    private ulong hostId;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        hostId = NetworkManager.Singleton.LocalClientId;
    }


    void Update()
    {
        if (GameManager.Instance.MapManager.playing
        && GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer].playerID == hostId)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRpc("right");
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveRpc("left");
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveRpc("up");
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoveRpc("down");
        }
        

    }



    [Rpc(SendTo.Server)]
    private void MoveRpc(string direction)
    {
        MapPlayer currentPlayer = GameManager.Instance.MapManager.players[GameManager.Instance.MapManager.currentPlayer];
        switch (direction)
        {
            case "right":
                MapPlayerBehavior.MovePlayer(currentPlayer.position.x, currentPlayer.position.y + 1);
                break;
            case "left":
                MapPlayerBehavior.MovePlayer(currentPlayer.position.x, currentPlayer.position.y - 1);
                break;
            case "up":
                MapPlayerBehavior.MovePlayer(currentPlayer.position.x + 1, currentPlayer.position.y);
                break;
            case "down":
                MapPlayerBehavior.MovePlayer(currentPlayer.position.x - 1, currentPlayer.position.y);
                break;
            }
        
        if (GameManager.Instance.MapManager.moves < 1)
        {
            GameManager.Instance.MapManager.NextPlayer();
        }
    }
}
