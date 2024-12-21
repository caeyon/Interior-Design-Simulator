using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoofCreate : MonoBehaviour
{
    public string wallTag = "WALL"; // 벽 오브젝트의 태그
    public Material roofMaterial; // 지붕에 사용할 재질
    public float roofHeightOffset = 0.5f; // 지붕의 높이 보정 (벽 상단 위로)
    private float wallHeight = 2.31f;

    public void GenerateRoof()
    {
        // 태그를 기반으로 벽 오브젝트 검색
        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);

        if (walls == null || walls.Length == 0)
        {
            Debug.LogError("태그 '" + wallTag + "'를 가진 벽 오브젝트를 찾을 수 없습니다!");
            return;
        }

        List<Vector3> wallVertices = new List<Vector3>();

        // 벽들의 꼭짓점을 수집
        foreach (GameObject wall in walls)
        {
            if (wall == null) continue;

            MeshFilter meshFilter = wall.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) continue;

            // 월드 좌표로 변환된 벽 꼭짓점 추가
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 worldPoint = wall.transform.TransformPoint(vertex);
                wallVertices.Add(worldPoint);
            }
        }

        if (wallVertices.Count == 0)
        {
            Debug.LogError("벽 꼭짓점을 찾을 수 없습니다!");
            return;
        }

        // 중복된 꼭짓점 제거
        wallVertices = FilterUniqueVertices(wallVertices, 0.01f);

        // 벽 꼭짓점 기반 경계 계산
        Vector3 minPoint = Vector3.positiveInfinity;
        Vector3 maxPoint = Vector3.negativeInfinity;

        foreach (Vector3 vertex in wallVertices)
        {
            minPoint = Vector3.Min(minPoint, vertex);
            maxPoint = Vector3.Max(maxPoint, vertex);
        }

        Vector3 center = (minPoint + maxPoint) / 2; // 중심점
        float maxHeight = wallHeight; // 지붕 높이 설정

        // 지붕 생성
        GameObject roof = new GameObject("Roof", typeof(MeshFilter), typeof(MeshRenderer));

        // 메쉬 생성
        Mesh roofMesh = new Mesh();

        // Convex Hull 계산
        List<Vector2> points2D = new List<Vector2>();
        foreach (Vector3 vertex in wallVertices)
        {
            points2D.Add(new Vector2(vertex.x, vertex.z)); // XZ 평면 변환
        }

        List<int> hullIndices = CalculateConvexHull(points2D);

        Vector3[] roofVertices = new Vector3[hullIndices.Count];
        int[] roofTriangles = new int[(hullIndices.Count - 2) * 6];

        for (int i = 0; i < hullIndices.Count; i++)
        {
            Vector2 point2D = points2D[hullIndices[i]];
            roofVertices[i] = new Vector3(point2D.x, maxHeight, point2D.y);
        }

        // 기존 삼각형 데이터 생성 코드
        for (int i = 0; i < hullIndices.Count - 2; i++)
        {
            // 앞면
            roofTriangles[i * 6] = 0;
            roofTriangles[i * 6 + 1] = i + 1;
            roofTriangles[i * 6 + 2] = i + 2;

            // 뒷면
            roofTriangles[i * 6 + 3] = 0;
            roofTriangles[i * 6 + 4] = i + 2;
            roofTriangles[i * 6 + 5] = i + 1;
        }


        // 삼각형 순서 뒤집기 (노말 방향 수정)
        for (int i = 0; i < roofTriangles.Length; i += 3)
        {
            int temp = roofTriangles[i];
            roofTriangles[i] = roofTriangles[i + 2];
            roofTriangles[i + 2] = temp;
        }

        roofMesh.vertices = roofVertices;
        roofMesh.triangles = roofTriangles;
        roofMesh.RecalculateNormals(); // 노말 재계산
        roofMesh.RecalculateBounds(); // 경계 재계산

        roof.GetComponent<MeshFilter>().mesh = roofMesh;

        // 재질 설정
        Renderer roofRenderer = roof.GetComponent<Renderer>();
        if (roofMaterial != null)
        {
            roofRenderer.material = roofMaterial;
        }
        else
        {
            roofRenderer.material = new Material(Shader.Find("Standard"));
        }

        // BuildLoad 스크립트 추가
        BuildLoad buildLoad = roof.AddComponent<BuildLoad>();

        Debug.Log("지붕이 성공적으로 생성되었습니다!");

        // 디버그용 시각화
        foreach (Vector3 vertex in roofVertices)
        {
            Debug.DrawLine(vertex, vertex + Vector3.up * 0.5f, Color.red, 5f);
        }
    }

    // 중복된 꼭짓점 필터링
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

    // Convex Hull 알고리즘 (XZ 평면 기준)
    List<int> CalculateConvexHull(List<Vector2> points)
    {
        points.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

        List<int> hull = new List<int>();

        // 하단 경계
        for (int i = 0; i < points.Count; i++)
        {
            while (hull.Count >= 2 && Cross(points[hull[hull.Count - 2]], points[hull[hull.Count - 1]], points[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(i);
        }

        // 상단 경계
        int t = hull.Count + 1;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            while (hull.Count >= t && Cross(points[hull[hull.Count - 2]], points[hull[hull.Count - 1]], points[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(i);
        }

        hull.RemoveAt(hull.Count - 1); // 중복 제거
        return hull;
    }

    // 외적 계산
    float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }
}