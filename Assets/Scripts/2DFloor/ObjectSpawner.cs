using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // ������ ����
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject doorPrefab;

    // ���� ��ġ
    public Transform spawnPoint;

    // �� ����
    public void SpawnWall()
    {
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            wallPrefab.transform.position.y,
            spawnPoint.position.z
        );
        Instantiate(wallPrefab, spawnPosition, spawnPoint.rotation);
    }

    // â�� ����
    public void SpawnWindow()
    {
        Vector3 spawnPosition = new Vector3(
            spawnPoint.position.x,
            windowPrefab.transform.position.y,
            spawnPoint.position.z
        );
        Instantiate(windowPrefab, spawnPosition, spawnPoint.rotation);
    }

    // �� ����
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
