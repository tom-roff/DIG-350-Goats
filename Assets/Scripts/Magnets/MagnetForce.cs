using UnityEngine;

public class MagnetForce : MonoBehaviour
{
    public float forceStrength = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pearl") || other.CompareTag("Cube") || other.CompareTag("Trash"))
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

