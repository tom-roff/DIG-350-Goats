using UnityEngine;
using Unity.Netcode;

public class NumPlayersManager : NetworkBehaviour
{
    private OurNetwork network;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        network = GameManager.Instance.OurNetwork;
        int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;


        for(int i = 1; i <= 6; i++)
        {
            GameObject curLever = GameObject.Find("Lever"+i);
            curLever.SetActive(false);
    
            if(i <= numPlayers){
                curLever.SetActive(true);
            }
        }

    }

}
