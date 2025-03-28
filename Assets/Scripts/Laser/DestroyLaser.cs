using UnityEngine;
using Unity.Netcode;

public class DestroyLaser : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out var laser))
        {
            laser.Despawn(true);
        }
    }
}
