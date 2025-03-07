using UnityEngine;

public class LeverController : MonoBehaviour
{
    private Animator animator;
    private bool isPulled = false;

    //public GameObject TVCamera;

    //public GameObject PhoneCamera;

    private NetworkManager network;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from lever!");
        }

        /*if (IsMainScreen())
        {
            TVCamera.SetActive(true);
            PhoneCamera.SetActive(false);
        }
        else
        {
            TVCamera.SetActive(false);
            PhoneCamera.SetActive(true);
        }*/
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
            Debug.Log("Lever pulled, animation triggered!");
        }
    }

    /*bool IsMainScreen()
    {
        if (network.isHost){
            return true;
        }
        else{
            return false;
        }
        
    }*/
}
