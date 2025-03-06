using UnityEngine;

public class MatchPhoneRot : MonoBehaviour
{
    private Gyroscope gyro;
    private Quaternion rotation;

    void Start()
    {
        // Check if the device supports the gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;  // Enable the gyroscope
        }
        else
        {
            Debug.LogError("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (gyro != null)
        {
            // Get the rotation data from the gyroscope and apply it to the object's rotation
            rotation = gyro.attitude; // Get the gyroscope's attitude (rotation in space)
            transform.rotation = new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w); // Adjust for Unity's coordinate system
        }
    }
}