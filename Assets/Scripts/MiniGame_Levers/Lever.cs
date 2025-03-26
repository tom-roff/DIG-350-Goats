using UnityEngine;
using Unity.Netcode;

public class LeverController : NetworkBehaviour
{
    private Animator animator;
    private CameraController cameraController;
    private OurNetwork network;

    private int leverIndex;

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

        network = FindFirstObjectByType<OurNetwork>();
        if (network == null)
        {
            Debug.LogError("OurNetwork instance not found!");
        }
    }

    void Update()
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
    }

    [ServerRpc(RequireOwnership = false)]
    void PullLeverServerRpc(int leverIndex, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[ServerRpc] Lever {leverIndex} was pulled.");

        // Broadcast to all clients that this lever has been pulled
        PullLeverClientRpc(leverIndex);
    }

    [ClientRpc]
    void PullLeverClientRpc(int leverIndex, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"[ClientRpc] Lever {leverIndex} animation triggered.");
        
        // Play animation and disable collider
        animator.SetTrigger("IsPulled");
        GetComponent<Collider>().enabled = false;

        // Only the owner should call OnLeverPulled to update game logic
        if (IsOwner)
        {
            cameraController.OnLeverPulled(leverIndex);
        }
    }
}
