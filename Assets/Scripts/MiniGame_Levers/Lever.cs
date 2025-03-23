using UnityEngine;

public class LeverController : MonoBehaviour
{
    private Animator animator;
    private bool isPulled = false;

    public int leverIndex;
    public int PlayerID;
    private CameraController cameraController;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from lever!");
        }

        cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch Detected");
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform && !isPulled)
            {
                PullLever();
            }
        }
    }

    void PullLever()
    {
        if (!isPulled)
        {
            isPulled = true;
            animator.SetTrigger("IsPulled");
            GetComponent<Collider>().enabled = false; // Disable collider so it can't be touched again
            Debug.Log($"Lever {leverIndex} pulled!");
            cameraController.OnLeverPulled(leverIndex);
        }
    }
}
