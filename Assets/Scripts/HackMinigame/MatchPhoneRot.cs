using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GyroRotation : NetworkBehaviour
{
    private Gyroscope gyro;
    private Quaternion initialRotation;
    private bool gyroAvailable = false;
    private List<Quaternion> gyro_inputs = new List<Quaternion>();

    private Quaternion avg;

    public GameObject ballDownCollision;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroAvailable = true;

            
            // Capture the initial rotation as an inverse, so we can apply it as an offset
            initialRotation = Quaternion.Inverse(ConvertGyroRotation(gyro.attitude));
        }
        else
        {
            Debug.LogError("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        // if(IsServer == true)
            // Apply the offset so the object starts with zeroed rotation
            // avg = gyro_inputs.Average<Quaternion>();
            transform.rotation = initialRotation * ConvertGyroRotation(gyro.attitude);
    }

    // Convert the gyroscope rotation to Unity's coordinate system
    private Quaternion ConvertGyroRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.z, q.y, -q.w);
    }
}
