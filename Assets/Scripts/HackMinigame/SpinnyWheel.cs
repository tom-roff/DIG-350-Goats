using UnityEngine;

public class SpinnyWheel : MonoBehaviour
{
    public int rotationSpeed = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate (0, rotationSpeed*Time.deltaTime, 0);
    }
}
