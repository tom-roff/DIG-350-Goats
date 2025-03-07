using Unity.Netcode;
using UnityEngine;

public class LaserUI : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsHost)
        {
            gameObject.SetActive(false);
        }
    }
}
