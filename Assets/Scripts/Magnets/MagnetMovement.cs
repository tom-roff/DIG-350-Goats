using UnityEngine;

public class MagnetMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal,0,vertical) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
