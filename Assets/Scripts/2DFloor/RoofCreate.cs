using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoofCreate : MonoBehaviour
{
    public string wallTag = "WALL"; // �� ������Ʈ�� �±�
    public Material roofMaterial; // ���ؿ� ����� ����
    public float roofHeightOffset = 0.5f; // ������ ���� ���� (�� ��� ����)
    private float wallHeight = 2.31f;

    public void GenerateRoof()
    {
        // �±׸� ������� �� ������Ʈ �˻�
        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);

        if (walls == null || walls.Length == 0)
        {
            Debug.LogError("�±� '" + wallTag + "'�� ���� �� ������Ʈ�� ã�� �� �����ϴ�!");
            return;
        }

        List<Vector3> wallVertices = new List<Vector3>();

        // ������ �������� ����
        foreach (GameObject wall in walls)
        {
            if (wall == null) continue;

            MeshFilter meshFilter = wall.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) continue;

            // ���� ��ǥ�� ��ȯ�� �� ������ �߰�
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 worldPoint = wall.transform.TransformPoint(vertex);
                wallVertices.Add(worldPoint);
            }
        }

        if (wallVertices.Count == 0)
        {
            Debug.LogError("�� �������� ã�� �� �����ϴ�!");
            return;
        }

        // �ߺ��� ������ ����
        wallVertices = FilterUniqueVertices(wallVertices, 0.01f);

        // �� ������ ��� ��� ���
        Vector3 minPoint = Vector3.positiveInfinity;
        Vector3 maxPoint = Vector3.negativeInfinity;

        foreach (Vector3 vertex in wallVertices)
        {
            minPoint = Vector3.Min(minPoint, vertex);
            maxPoint = Vector3.Max(maxPoint, vertex);
        }

        Vector3 center = (minPoint + maxPoint) / 2; // �߽���
        float maxHeight = wallHeight; // ���� ���� ����

        // ���� ����
        GameObject roof = new GameObject("Roof", typeof(MeshFilter), typeof(MeshRenderer));

        // �޽� ����
        Mesh roofMesh = new Mesh();

        // Convex Hull ���
        List<Vector2> points2D = new List<Vector2>();
        foreach (Vector3 vertex in wallVertices)
        {
            points2D.Add(new Vector2(vertex.x, vertex.z)); // XZ ��� ��ȯ
        }

        List<int> hullIndices = CalculateConvexHull(points2D);

        Vector3[] roofVertices = new Vector3[hullIndices.Count];
        int[] roofTriangles = new int[(hullIndices.Count - 2) * 6];

        for (int i = 0; i < hullIndices.Count; i++)
        {
            Vector2 point2D = points2D[hullIndices[i]];
            roofVertices[i] = new Vector3(point2D.x, maxHeight, point2D.y);
        }

        // ���� �ﰢ�� ������ ���� �ڵ�
        for (int i = 0; i < hullIndices.Count - 2; i++)
        {
            // �ո�
            roofTriangles[i * 6] = 0;
            roofTriangles[i * 6 + 1] = i + 1;
            roofTriangles[i * 6 + 2] = i + 2;

            // �޸�
            roofTriangles[i * 6 + 3] = 0;
            roofTriangles[i * 6 + 4] = i + 2;
            roofTriangles[i * 6 + 5] = i + 1;
        }


        // �ﰢ�� ���� ������ (�븻 ���� ����)
        for (int i = 0; i < roofTriangles.Length; i += 3)
        {
            int temp = roofTriangles[i];
            roofTriangles[i] = roofTriangles[i + 2];
            roofTriangles[i + 2] = temp;
        }

        roofMesh.vertices = roofVertices;
        roofMesh.triangles = roofTriangles;
        roofMesh.RecalculateNormals(); // �븻 ����
        roofMesh.RecalculateBounds(); // ��� ����

        roof.GetComponent<MeshFilter>().mesh = roofMesh;

        // ���� ����
        Renderer roofRenderer = roof.GetComponent<Renderer>();
        if (roofMaterial != null)
        {
            roofRenderer.material = roofMaterial;
        }
        else
        {
            roofRenderer.material = new Material(Shader.Find("Standard"));
        }

        // BuildLoad ��ũ��Ʈ �߰�
        BuildLoad buildLoad = roof.AddComponent<BuildLoad>();

        Debug.Log("������ ���������� �����Ǿ����ϴ�!");

        // ����׿� �ð�ȭ
        foreach (Vector3 vertex in roofVertices)
        {
            Debug.DrawLine(vertex, vertex + Vector3.up * 0.5f, Color.red, 5f);
        }
    }

    // �ߺ��� ������ ���͸�
    List<Vector3> FilterUniqueVertices(List<Vector3> vertices, float tolerance)
    {
        List<Vector3> uniqueVertices = new List<Vector3>();

        foreach (Vector3 vertex in vertices)
        {
            if (!uniqueVertices.Exists(v => Vector3.Distance(v, vertex) < tolerance))
            {
                uniqueVertices.Add(vertex);
            }
        }

        return uniqueVertices;
    }

    // Convex Hull �˰��� (XZ ��� ����)
    List<int> CalculateConvexHull(List<Vector2> points)
    {
        points.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

        List<int> hull = new List<int>();

        // �ϴ� ���
        for (int i = 0; i < points.Count; i++)
        {
            while (hull.Count >= 2 && Cross(points[hull[hull.Count - 2]], points[hull[hull.Count - 1]], points[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(i);
        }

        // ��� ���
        int t = hull.Count + 1;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            while (hull.Count >= t && Cross(points[hull[hull.Count - 2]], points[hull[hull.Count - 1]], points[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(i);
        }

        hull.RemoveAt(hull.Count - 1); // �ߺ� ����
        return hull;
    }

    // ���� ���
    float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }
}