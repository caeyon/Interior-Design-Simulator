using UnityEngine;
using UnityEngine.SceneManagement;

public class moveScene : MonoBehaviour
{
    public string[] tagsToAdjust = { "WALL", "WINDOW", "DOOR" };
    public float yOffset = 0.136f;

    private string targetSceneName = "FurnitureEdit";

    public void ChangeScene()
    {
        transform.GetComponent<RoofCreate>().GenerateRoof();
        AdjustYPositionForTaggedObjects();
        SceneManager.LoadScene(targetSceneName);
    }

    void AdjustYPositionForTaggedObjects()
    {
        foreach (string tag in tagsToAdjust)
        {
            // 현재 태그를 가진 모든 오브젝트를 검색
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    if (obj.CompareTag("WALL"))
                    {
                        Transform parentObject = obj.transform.parent;
                        Vector3 nP = parentObject.position;
                        nP.y -= yOffset; // Y축 이동
                        parentObject.position = nP;

                        // 디버그 메시지 출력
                        Debug.Log($"Object '{obj.name}' with tag '{tag}' moved to Y: {nP.y}");
                    }
                    else
                    {
                        // 오브젝트의 Y 좌표를 변경
                        Vector3 newPosition = obj.transform.position;
                        newPosition.y -= yOffset; // Y축 이동
                        obj.transform.position = newPosition;

                        // 디버그 메시지 출력
                        Debug.Log($"Object '{obj.name}' with tag '{tag}' moved to Y: {newPosition.y}");
                    }
                }
            }
        }

        GameObject floor = GameObject.FindGameObjectWithTag("FLOOR");

        if (floor != null)
        {
            // 오브젝트의 Y 좌표를 변경
            Vector3 newPosition = floor.transform.position;
            newPosition.y += 0.1f; // Y축 이동
            floor.transform.position = newPosition;

            // 디버그 메시지 출력
            Debug.Log($"Object '{floor.name}' with tag '{tag}' moved to Y: {newPosition.y}");
        }
    }
}
