using UnityEngine;

public class GyroRotation : MonoBehaviour
{
    private Gyroscope gyro;
    private Quaternion initialRotation;
    private bool gyroAvailable = false;

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
        if (gyroAvailable)
        {
            // Apply the offset so the object starts with zeroed rotation
            transform.rotation = initialRotation * ConvertGyroRotation(gyro.attitude);
        }
    }

    // Convert the gyroscope rotation to Unity's coordinate system
    private Quaternion ConvertGyroRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.z, q.y, -q.w);
    }
}
