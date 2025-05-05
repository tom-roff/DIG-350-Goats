using UnityEngine;

public class MovingPart : MonoBehaviour
{
    public Transform startMarker;
    public Transform endMarker;
    public float speed = 6.0F;

    private float startTime;
    private float journeyLength;
    private bool movingToEnd = true;

    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        // Determine the direction of movement
        if (movingToEnd)
        {
            transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
        }
        else
        {
            transform.position = Vector3.Lerp(endMarker.position, startMarker.position, fractionOfJourney);
        }

        // If the object has reached (or slightly passed) the end of the journey
        if (fractionOfJourney >= 1.0f)
        {
            movingToEnd = !movingToEnd;
            startTime = Time.time; // reset the start time
        }
    }
}
