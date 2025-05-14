using UnityEngine;
using System.Collections;

public class HelicopterControl : MonoBehaviour
{
    public float minAccReq = 1.2f; //min acc required to trigger movement
    public float moveBy = 2f;
    public float cooldown = 0.5f;

    private float cooldownTimer = 0f;

    void Start()
    {
        // for phoen orientation
        Input.gyro.enabled = true;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // // acceleration in physical space
        // Vector3 deviceAcceleration = Input.acceleration;

        //SIMULATING INPUT for testing; TO REMOVE
        Vector3 inputAcceleration;
#if UNITY_EDITOR
        // simulate using arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
            inputAcceleration = new Vector3(0, 1.5f, 0);
        else if (Input.GetKey(KeyCode.DownArrow))
            inputAcceleration = new Vector3(0, -1.5f, 0); 
        else
            inputAcceleration = Vector3.zero;
#else
        // Real input on actual device
        inputAcceleration = Input.acceleration;
#endif


        // access orientation
        Quaternion deviceRotation = Input.gyro.attitude;

        // Convert to Unity's coordinate system (right-handed)
        Quaternion unityRotation = new Quaternion(deviceRotation.x, deviceRotation.y, -deviceRotation.z, -deviceRotation.w);

        // translating acc to world(game) space
        // Vector3 worldAcceleration = unityRotation * deviceAcceleration;
        Vector3 worldAcceleration = unityRotation * inputAcceleration; //simulating purposes, REMOVE


        // DEBUGGING
        // Debug.Log("World Accel Y: " + worldAcceleration.y);

        if (cooldownTimer <= 0f)
        {
            if (worldAcceleration.y > minAccReq)
            {
                transform.position += Vector3.up * moveBy;
                cooldownTimer = cooldown;
            }
            else if (worldAcceleration.y < -minAccReq)
            {
                transform.position += Vector3.down * moveBy;
                cooldownTimer = cooldown;
            }
        }
    }
}
