using UnityEngine;

public class HelipadObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float spawnInterval = 4f;
    public float verticalRange = 4f;
    public float horizontalSpawnOffset = 12f;

    public float obstacleSpeedMin = 2f;
    public float obstacleSpeedMax = 6f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        // Random Y position
        float y = Random.Range(-verticalRange, verticalRange);

        // Random left or right side
        bool fromLeft = Random.value > 0.5f;
        float x = fromLeft ? -horizontalSpawnOffset : horizontalSpawnOffset;
        Vector3 spawnPos = new Vector3(x, y, 0f);

        // Instantiate obstacle
        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        // Set direction and speed
        HelipadObstacle obstacle = newObstacle.GetComponent<HelipadObstacle>();
        obstacle.moveLeft = !fromLeft; // move toward the center
        obstacle.speed = Random.Range(obstacleSpeedMin, obstacleSpeedMax);
    }
}
