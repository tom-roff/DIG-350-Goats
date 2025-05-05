using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Laser : NetworkBehaviour
{
    private float speed = 30f;
    LaserManager laserManager;
    private bool canMove = false;

    public override void OnNetworkSpawn()
    {
        laserManager = FindObjectOfType<LaserManager>();
        if (IsServer)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    private IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    void FixedUpdate()
    {
        if (IsServer && canMove)
        {
            transform.Translate(-Vector3.forward * speed * Time.fixedDeltaTime);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<NetworkObject>(out var hitObject))
        {
            if (hitObject.CompareTag("Player"))
            {
                ulong playerId = hitObject.OwnerClientId;
                
                hitObject.Despawn(true);
                
                laserManager.KillPlayer(playerId);
            }
        }
    }
}
