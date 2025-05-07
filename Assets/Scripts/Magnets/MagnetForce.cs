using UnityEngine;
using Unity.Netcode;

public class MagnetForce : MonoBehaviour
{
    public float forceStrength = 30f;

    private double score;
    private void Start()
    {
        score = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 direction = (transform.position - other.transform.position).normalized;
                rb.AddForce(direction * forceStrength * -1);
            }
        }
        
    }

}

