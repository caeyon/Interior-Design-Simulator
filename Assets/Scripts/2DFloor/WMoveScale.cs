using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WMoveScale : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private MeshCollider planeCollider; 
    private Vector3 planeBounds;

    private float resizeSpeed = 0.4f;
    private float minSize = 1f;
    private float maxSize = 10f;
    private int rotateValue = 0;

    private Transform parentObject; // 자식 오브젝트

    private GameObject selectedObject;
    private Vector3 screenPoint;
    private Vector3 offset2;

    private List<Collider> floorColliders = new List<Collider>();

    void Start()
    {
        // 'Plane' 태그가 붙은 오브젝트의 MeshCollider 가져오기
        UpdateFloorColliders();

        parentObject = transform.parent;

    }

    private void Update()
    {
        GameObject floorObject = GameObject.FindGameObjectWithTag("FLOOR");

        if (rotateValue == 0 || rotateValue == 2)
        {
            maxSize = floorObject.transform.localScale.x * 20f;
        }
        else if (rotateValue == 1 || rotateValue == 3)
        {
            maxSize = floorObject.transform.localScale.z * 20f;
        }
    }

    public void UpdateFloorColliders()
    {
        floorColliders.Clear(); // 기존 리스트 초기화
        GameObject[] floorObjects = GameObject.FindGameObjectsWithTag("FLOOR");
        foreach (GameObject floorObject in floorObjects)
        {
            Collider floorCollider = floorObject.GetComponent<Collider>();
            if (floorCollider != null)
            {
                floorColliders.Add(floorCollider);
            }
        }
    }

    // 드래그 시작 시 호출
    public void StartDragging(Vector3 mousePosition)
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        lastMousePosition = mouseWorldPosition;
        lastMousePosition.y = parentObject.position.y;  //마우스 Y값 고정
        isDragging = true;
    }

    // 드래그 종료 시 호출
    public void StopDragging()
    {
        isDragging = false;
    }

    // 이동
    public void MoveObject(Vector3 mousePosition)
    {
        if (isDragging && floorColliders.Count > 0)
        {
            Vector3 curScreenPoint = new Vector3(mousePosition.x, mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset2;
            curPosition.y = parentObject.transform.position.y;

            foreach (Collider floorCollider in floorColliders)
            {

                if (curPosition.x >= floorCollider.bounds.min.x - 0.1 && curPosition.x <= floorCollider.bounds.max.x && curPosition.z >= floorCollider.bounds.min.z && curPosition.z <= floorCollider.bounds.max.z)
                {
                    float objectHalfLength = transform.localScale.z / 2;

                    if (rotateValue == 0)
                    {
                        curPosition.x = Mathf.Clamp(curPosition.x, floorCollider.bounds.min.x, floorCollider.bounds.max.x - parentObject.localScale.x / 2);
                        curPosition.z = Mathf.Clamp(curPosition.z, floorCollider.bounds.min.z + objectHalfLength, floorCollider.bounds.max.z - objectHalfLength);
                    }
                    else if (rotateValue == 1)
                    {
                        curPosition.x = Mathf.Clamp(curPosition.x, floorCollider.bounds.min.x + objectHalfLength, floorCollider.bounds.max.x - objectHalfLength);
                        curPosition.z = Mathf.Clamp(curPosition.z, floorCollider.bounds.min.z + parentObject.localScale.x / 2, floorCollider.bounds.max.z);
                    }
                    else if (rotateValue == 2)
                    {
                        curPosition.x = Mathf.Clamp(curPosition.x, floorCollider.bounds.min.x + parentObject.localScale.x / 2, floorCollider.bounds.max.x);
                        curPosition.z = Mathf.Clamp(curPosition.z, floorCollider.bounds.min.z + objectHalfLength, floorCollider.bounds.max.z - objectHalfLength);
                    }
                    else if (rotateValue == 3)
                    {
                        curPosition.x = Mathf.Clamp(curPosition.x, floorCollider.bounds.min.x + objectHalfLength, floorCollider.bounds.max.x - objectHalfLength);
                        curPosition.z = Mathf.Clamp(curPosition.z, floorCollider.bounds.min.z, floorCollider.bounds.max.z - parentObject.localScale.x / 2);
                    }

                    if (!IsColliding(curPosition))
                    {
                        parentObject.position = curPosition;
                    }
                }
            }
        }
    }


    public void ResizeObject(Vector3 mousePosition)
    {
        Vector3 newScale = parentObject.localScale;
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //마우스 이동 감지
        float deltaX = currentMousePosition.x - lastMousePosition.x;
        float deltaZ = currentMousePosition.z - lastMousePosition.z;

        // 0도 90도 180도 270도 감지
        if (rotateValue == 0)
        {
            newScale = parentObject.localScale + new Vector3(deltaX, 0, 0) * resizeSpeed;
        }
        else if (rotateValue == 1)
        {
            newScale = parentObject.localScale - new Vector3(deltaZ, 0, 0) * resizeSpeed;
        }
        else if (rotateValue == 2)
        {
            newScale = parentObject.localScale - new Vector3(deltaX, 0, 0) * resizeSpeed;
        }
        else if (rotateValue == 3)
        {
             newScale = parentObject.localScale + new Vector3(deltaZ, 0, 0) * resizeSpeed;
        }

        // 최소 크기 1, 최대 크기 10으로 제한
        newScale.x = Mathf.Clamp(newScale.x, minSize, maxSize);

        // 새로운 크기 적용
        parentObject.localScale = newScale;

        lastMousePosition = currentMousePosition;
    }

    public void Rotate()
    {
        parentObject.Rotate(0, 90, 0);
        rotateValue += 1;
        if (rotateValue == 4) rotateValue = 0;
    }

    public void Del()
    {
        Destroy(parentObject.gameObject);
    }

    public void Select()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) //ray가 3D 오브젝트와 충돌
        {
            GameObject clickedObject = hit.transform.gameObject; //클릭된 오브젝트

            selectedObject = clickedObject;

            //마우스 위치와 물체 위치 사이 계산
            screenPoint = Camera.main.WorldToScreenPoint(selectedObject.transform.parent.position);
            offset2 = selectedObject.transform.parent.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }

    private bool IsColliding(Vector3 targetPosition)
    {

        Collider selectedCollider = selectedObject.GetComponent<Collider>();
        Bounds selectedBounds = selectedCollider.bounds;

        Vector3 newExtents = selectedBounds.extents;

        if (rotateValue == 0)
        {
            targetPosition.x = targetPosition.x + parentObject.localScale.x / 4;
        }
        else if (rotateValue == 1)
        {
            targetPosition.z = targetPosition.z - parentObject.localScale.x / 4;
        }
        else if (rotateValue == 2)
        {
            targetPosition.x = targetPosition.x - parentObject.localScale.x / 4;
        }
        else if (rotateValue == 3)
        {
            targetPosition.z = targetPosition.z + parentObject.localScale.x / 4;
        }

        if (rotateValue == 0 || rotateValue == 2)
        {
            newExtents.x *= 0.93f;
            newExtents.z *= 0.85f;
        }
        else if (rotateValue == 1 || rotateValue == 3)
        {
            newExtents.z *= 0.93f;
            newExtents.x *= 0.85f;
        }

        // OverlapBox로 충돌 체크
        Collider[] colliders = Physics.OverlapBox(targetPosition, newExtents, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            // 다른 벽과의 충돌 확인
            if (collider.CompareTag("WALL") && collider.gameObject != selectedObject)
            {
                Debug.Log("Collision detected with another Wall.");
                return true; // 충돌 있음
            }
        }
        //충돌 없음
        return false;
    }
    public int rotateReturn()
    {
        return rotateValue;
    }
}
