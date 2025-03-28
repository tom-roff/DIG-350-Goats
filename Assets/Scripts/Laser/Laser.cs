using UnityEngine;
using Unity.Netcode;

public class Laser : NetworkBehaviour
{
    public float speed = 4f;
    LaserManager laserManager;

    public override void OnNetworkSpawn()
    {
        laserManager = FindObjectOfType<LaserManager>();
    }
    
    void Update()
    {
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<NetworkObject>(out var hitObject))
        {
            if (hitObject.CompareTag("Player"))
            {
                Debug.Log("hit player");
                ulong playerId = hitObject.OwnerClientId;
                
                hitObject.Despawn(true);
                
                laserManager.KillPlayer(playerId);
            }
        }
    }
}
