using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int side;
    public Rigidbody rb;
    public BoxCollider boxCollider;
    public bool isGrounded;

    List<Vector3> sides = new List<Vector3>()
    {
        new Vector3(0, 1, 0), // 1(+) or 6(-)
        new Vector3(0, 0, 1), // 2(+) or 5(-)
        new Vector3(1, 0, 0), // 3(+) or 4(-)
    };

    void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (boxCollider == null)
            boxCollider = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        side = WhichIsUp();
        isGrounded = IsGrounded();
    }

    // void OnCollisionStay(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //         isGrounded = true;
    // }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f);
    }

    // https://discussions.unity.com/t/dice-which-face-is-up/381512/6
    int WhichIsUp()
    {
        var maxY = float.NegativeInfinity;
        var result = -1;

        for (var i = 0; i < 3; i++)
        {
            // Transform the vector to world-space:
            Vector3 worldSpace = transform.TransformDirection(sides[i]);
            if (worldSpace.y > maxY)
            {
                result = i + 1; // index 0 is 1
                maxY = worldSpace.y;
            }
            // also check opposite side
            if (-worldSpace.y > maxY)
            {
                result = 6 - i; // sum of opposite sides = 7
                maxY = -worldSpace.y;
            }
        }
        return result;
    }
}
