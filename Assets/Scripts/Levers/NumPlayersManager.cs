using UnityEngine;
using Unity.Netcode;

public class NumPlayersManager : NetworkBehaviour
{
    private OurNetwork network;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        network = FindFirstObjectByType<OurNetwork>();
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
            return;
        }

        int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;


        for(int i = 0; i <= 5; i++)
        {
            GameObject curLever = GameObject.Find("Lever"+i);
            curLever.SetActive(false);
    
            if(i < numPlayers){
                curLever.SetActive(true);
            }
        }

    }

}
