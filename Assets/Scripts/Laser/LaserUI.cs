using Unity.Netcode;
using UnityEngine;

public class LaserUI : NetworkBehaviour
{
    void Awake()
    {
        if (GameManager.Instance.OurNetwork.isHost)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
