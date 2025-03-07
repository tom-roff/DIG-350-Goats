using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 4f;
    
    void Update()
    {
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);
    }
}
