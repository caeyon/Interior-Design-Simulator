using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // 橇府普 曼炼
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject doorPrefab;

    // 积己 困摹
    public Transform spawnPoint;

    // 寒 积己
    public void SpawnWall()
    {
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            wallPrefab.transform.position.y,
            spawnPoint.position.z
        );
        Instantiate(wallPrefab, spawnPosition, spawnPoint.rotation);
    }

    // 芒巩 积己
    public void SpawnWindow()
    {
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            windowPrefab.transform.position.y,
            spawnPoint.position.z
        );
        Instantiate(windowPrefab, spawnPosition, spawnPoint.rotation);
    }

    // 巩 积己
    public void SpawnDoor()
    {
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            doorPrefab.transform.position.y,
            spawnPoint.position.z
        );
        Instantiate(doorPrefab, spawnPosition, spawnPoint.rotation);
    }
}
