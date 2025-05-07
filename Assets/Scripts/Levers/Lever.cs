using UnityEngine;
using Unity.Netcode;

public class Lever : NetworkBehaviour
{
    private Animator animator;
    private CameraController cameraController;
    private bool hasBeenPulled = false;

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

        Input.gyro.enabled = true;
    }

    /*void Update() REAL TRACK ROTATION FUNCTION FOR PHONE
    {
        // Only run this on the client that owns the lever
        if (!IsOwner || hasBeenPulled) return;
        Quaternion rotation = Input.gyro.attitude;
        rotation = Quaternion.Euler(90f, 0f, 0f) * (new Quaternion(-rotation.x, -rotation.y, rotation.z, rotation.w)); // Convert to Unity coordinates

        float zTilt = rotation.eulerAngles.z;

        // Normalize Z-axis to handle wrap-around
        if (zTilt > 180f) zTilt -= 360f;

        if (zTilt < -30f) // Pull-back detected (tweak threshold as needed)
        {
            Debug.Log($"Phone pulled for lever {leverIndex} (zTilt = {zTilt})");
            hasBeenPulled = true;
            PullLeverServerRpc(leverIndex);
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
        if (hasBeenPulled)
        {
            return;
        }
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            Debug.Log($"Lever {leverIndex} clicked or tapped!");
            hasBeenPulled = true;
            PullLeverServerRpc(leverIndex);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void PullLeverServerRpc(int _leverIndex, ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[ServerRpc] Lever {_leverIndex} was pulled.");

        // Let server check lever logic
        CameraController serverCameraController = FindFirstObjectByType<CameraController>();
        if (serverCameraController != null)
        {
            serverCameraController.OnLeverPulled(_leverIndex);
        }

        // Broadcast to all clients that this lever has been pulled
        PullLeverClientRpc(_leverIndex);
    }

    [ClientRpc]
    void PullLeverClientRpc(int _leverIndex, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"[ClientRpc] Lever {_leverIndex} animation triggered.");
        
        // Play animation and disable collider
        animator.SetTrigger("LeverPulled");
    }


}
