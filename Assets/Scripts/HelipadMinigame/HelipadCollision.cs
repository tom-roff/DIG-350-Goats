using UnityEngine;

public class HelipadCollision : MonoBehaviour
{
    private bool isCrashed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeliObstacle") && !isCrashed)
        {
            isCrashed = true;
            Debug.Log("Helicopter crashed!");

            //gameover trigger to manager
            HelipadManager.Instance.TriggerGameOver();

            // Disable movement
            GetComponent<HelicopterControl>().enabled = false;
        }
    }

    // void HandleCrash()
    // {
    //     // Disable movement
    //     GetComponent<YourMovementScript>().enabled = false;

    //     //TODO: UI components, what happens upon collision
    //     // needs integration with network etc.

    //     Debug.Log("Game Over");
    // }
}
