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

        int numPlayers = network.playerIndexMap.Count;
        ulong playerId = NetworkManager.Singleton.LocalClientId;

        for(int i = 0; i < 6; i++)
        {
            GameObject curLever = GameObject.Find("Lever"+i);
            Debug.Log("Lever"+i);
            if(playerId == 0)
                if(i >= numPlayers){
                    Debug.Log("i = " + i);
                    Debug.Log("NumPlayers = " + numPlayers);
                    Debug.Log(i +" > " + numPlayers);
                    curLever.SetActive(false);
                }
            else{
                if(i > numPlayers){
                    Debug.Log("i = " + i);
                    Debug.Log("NumPlayers = " + numPlayers);
                    Debug.Log(i +" > " + numPlayers);
                    curLever.SetActive(false);
                }
            }
        }

    }

}
