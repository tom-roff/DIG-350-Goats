//  using UnityEngine;
// using System.Collections.Generic;
// using Unity.Netcode;

// public class HostMapControls : NetworkBehaviour
// {
//     public void Start()
//     {
//         if (GameManager.Instance.MapManager.players == null)
//         {
//             CreatePlayerQueue();
//         }
//     }
//     void CreatePlayerQueue()
//     {
//         GameManager.Instance.MapManager.players = new MapPlayer[NetworkManager.Singleton.ConnectedClientsIds.Count]; // replace with # of players in lobby
//         for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
//         {
//             if (NetworkManager.Singleton.ConnectedClientsIds[i] != NetworkManager.Singleton.LocalClientId)
//             {
//                 GameManager.Instance.MapManager.players[i] = new MapPlayer(NetworkManager.Singleton.ConnectedClientsIds[i]);
//                 Debug.Log(NetworkManager.Singleton.ConnectedClientsIds[i]);
//             }
//         }
//     }
// }