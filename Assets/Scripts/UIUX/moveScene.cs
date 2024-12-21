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
            // ���� �±׸� ���� ��� ������Ʈ�� �˻�
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    if (obj.CompareTag("WALL"))
                    {
                        Transform parentObject = obj.transform.parent;
                        Vector3 nP = parentObject.position;
                        nP.y -= yOffset; // Y�� �̵�
                        parentObject.position = nP;

                        // ����� �޽��� ���
                        Debug.Log($"Object '{obj.name}' with tag '{tag}' moved to Y: {nP.y}");
                    }
                    else
                    {
                        // ������Ʈ�� Y ��ǥ�� ����
                        Vector3 newPosition = obj.transform.position;
                        newPosition.y -= yOffset; // Y�� �̵�
                        obj.transform.position = newPosition;

                        // ����� �޽��� ���
                        Debug.Log($"Object '{obj.name}' with tag '{tag}' moved to Y: {newPosition.y}");
                    }
                }
            }
        }

        GameObject floor = GameObject.FindGameObjectWithTag("FLOOR");

        if (floor != null)
        {
            // ������Ʈ�� Y ��ǥ�� ����
            Vector3 newPosition = floor.transform.position;
            newPosition.y += 0.1f; // Y�� �̵�
            floor.transform.position = newPosition;

            // ����� �޽��� ���
            Debug.Log($"Object '{floor.name}' with tag '{tag}' moved to Y: {newPosition.y}");
        }
    }
}
