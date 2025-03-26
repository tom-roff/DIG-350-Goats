using UnityEngine;

public class HackGoalScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            Debug.Log("Player has won!");
        }
    }
}
