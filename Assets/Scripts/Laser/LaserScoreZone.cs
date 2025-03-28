using UnityEngine;

public class LaserScoreZone : MonoBehaviour
{
    public LaserManager LaserManager;
    private void OnTriggerEnter(Collider other)
    {
        LaserManager.UpdateScores();
    }
}
