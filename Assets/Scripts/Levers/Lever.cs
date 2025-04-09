using UnityEngine;
using Unity.Netcode;

public class Lever : NetworkBehaviour
{
    private Animator animator;
    private CameraController cameraController;

    public int leverIndex;

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

    /*void Update() REAL TRACK TOUCH FUNCTION FOR PHONE
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch Detected");
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                PullLeverServerRpc(leverIndex);
            }
        }
    }*/

    void Update()
    {
        // Touch input (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            CheckLeverHit(ray);
        }

        // Mouse input (desktop testing)
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CheckLeverHit(ray);
        }
    }

    void CheckLeverHit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            Debug.Log($"Lever {leverIndex} clicked or tapped!");
            PullLeverServerRpc(leverIndex);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void PullLeverServerRpc(int _leverIndex, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[ServerRpc] Lever {_leverIndex} was pulled.");

        // Broadcast to all clients that this lever has been pulled
        PullLeverClientRpc(_leverIndex);
    }

    [ClientRpc]
    void PullLeverClientRpc(int _leverIndex, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"[ClientRpc] Lever {_leverIndex} animation triggered.");
        
        // Play animation and disable collider
        animator.SetTrigger("IsPulled");
        //GetComponent<Collider>().enabled = false;

        // Only the owner should call OnLeverPulled to update game logic
        if (IsOwner)
        {
            cameraController.OnLeverPulled(_leverIndex);
        }
    }


}
