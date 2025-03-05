using UnityEngine;

public class LeverController : MonoBehaviour
{
    private Animator animator;
    private bool isPulled = false;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        isPulled = true;
        animator.SetTrigger("isPulled");
    }
}
