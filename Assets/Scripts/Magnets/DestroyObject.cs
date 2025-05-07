using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public double score;

    void Start()
    {
        score = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Magnet"))
        {
            Destroy(gameObject);
            LocalScoreManager.AddScore(0.5f);
            //Debug.Log("score = " + score); // Add 0.5 points to the player's score
        }
    }
}

// using UnityEngine;
// using Unity.Netcode;

// public class DestroyObject : NetworkBehaviour
// {
//     private void OnCollisionEnter(Collision collision)
//     {
//         if(!IsOwner) return; 
//         if (collision.gameObject.CompareTag("Magnet"))
//         {
//             if (IsServer) // Ensure only the server handles scoring and destruction
//             {
//                 ulong playerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId; // Get the player who owns the magnet
//                 ScoreManager.Instance.AddScoreServerRpc(playerId, 0.5f); // Add 0.5 points to the player's score
//                 Destroy(gameObject); // Destroy the object on the server
            
//             }
//         }
//     }
// }

