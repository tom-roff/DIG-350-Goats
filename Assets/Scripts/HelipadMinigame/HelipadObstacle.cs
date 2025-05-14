using UnityEngine;

public class HelipadObstacle : MonoBehaviour
{
    public float speed = 5f;
    public bool moveLeft = true;
    public float destroyX = 15f;

    void Update()
    {
        Vector3 direction = moveLeft ? Vector3.left : Vector3.right;
        transform.position += direction * speed * Time.deltaTime;

        // Destroy when off screen
        if (Mathf.Abs(transform.position.x) > destroyX)
            Destroy(gameObject);
    }
}
