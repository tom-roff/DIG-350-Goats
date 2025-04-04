using UnityEngine;

public class ClimingPlayerMovement : MonoBehaviour
{
    private bool right = true;
    private int climbAmount = 2;

    private void Update()
    {
        if (right)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                // Start checking for W key while D is held
                StartCoroutine(CheckForWKeyWhileHoldingD());
            }
        } 
        else 
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Start checking for W key while A is held
                StartCoroutine(CheckForWKeyWhileHoldingA());
            }
        }
    }

    private System.Collections.IEnumerator CheckForWKeyWhileHoldingD()
    {
        // Continue checking as long as D is held down
        while (Input.GetKey(KeyCode.D))
        {
            // If W is pressed while D is still held down
            if (Input.GetKeyDown(KeyCode.W))
            {
                Climb();
                yield break; // Exit the coroutine after climbing
            }
            yield return null; // Wait for next frame
        }
    }

    private System.Collections.IEnumerator CheckForWKeyWhileHoldingA()
    {
        // Continue checking as long as A is held down
        while (Input.GetKey(KeyCode.A))
        {
            // If W is pressed while A is still held down
            if (Input.GetKeyDown(KeyCode.W))
            {
                Climb();
                yield break; // Exit the coroutine after climbing
            }
            yield return null; // Wait for next frame
        }
    }

    private void Climb()
    {
        // Move the player up
        transform.position += Vector3.up * climbAmount;

        ToggleArm();
        
        // Room for animation code to be added later
        // Example:
        // Animator animator = GetComponent<Animator>();
        // if (right)
        //     animator.SetTrigger("ClimbRightHand");
        // else
        //     animator.SetTrigger("ClimbLeftHand");
    }

    // Public method to toggle which arm is active
    public void ToggleArm()
    {
        right = !right;
    }
}
