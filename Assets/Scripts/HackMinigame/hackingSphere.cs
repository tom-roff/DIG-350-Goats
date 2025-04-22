using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;


public class hackingSphere : NetworkBehaviour
{
   public float forceMultiplier = 10f;
    private Rigidbody rb;
    private Gyroscope gyro;
    private bool gyroEnabled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Only enable gyro if this is the local player
        if (IsOwner)
        {
            gyroEnabled = EnableGyro();
        }
    }

    void FixedUpdate()
    {
        // Only the owning client applies physics
        if (!IsOwner || !gyroEnabled) return;

        Vector3 gravity = gyro.gravity;
        Vector3 forceDirection = new Vector3(gravity.x, 0, gravity.y);

        rb.AddForce(forceDirection * forceMultiplier);
    }

    bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            return true;
        }
        return false;
    }
}

// using UnityEngine;

// public class hackingSphere : MonoBehaviour
// {
//     public float forceMultiplier = 10f;
//     private Rigidbody rb;
//     private bool gyroEnabled;
//     private Gyroscope gyro;

//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();

//         // Enable gyroscope
//         gyroEnabled = EnableGyro();
//     }

//     void FixedUpdate()
//     {
//         if (!gyroEnabled) return;

//         // Get the gravity vector from the gyroscope
//         Vector3 gravity = gyro.gravity;

//         // Apply force opposite to the gravity vector's X and Y (you may adjust based on how the phone is held)
//         Vector3 forceDirection = new Vector3(gravity.x, 0, gravity.y);

//         rb.AddForce(forceDirection * forceMultiplier);
//     }

//     bool EnableGyro()
//     {
//         if (SystemInfo.supportsGyroscope)
//         {
//             gyro = Input.gyro;
//             gyro.enabled = true;
//             return true;
//         }
//         return false;
//     }
// }
