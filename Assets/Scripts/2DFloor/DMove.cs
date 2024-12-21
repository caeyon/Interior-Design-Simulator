using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMove : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private GameObject selectedObject;
    private Vector3 screenPoint;
    private Vector3 offset2;
    private GameObject[] wallObjects;
    private int rotateValue;

    private List<Collider> wallColliders = new List<Collider>();

    void Start()
    {
        UpdateWallColliders();
    }

    public void UpdateWallColliders()
    {
        wallColliders.Clear(); // ���� ����Ʈ �ʱ�ȭ
        wallObjects = GameObject.FindGameObjectsWithTag("WALL");
        foreach (GameObject wallObject in wallObjects)
        {
            Collider wallCollider = wallObject.GetComponent<Collider>();
            if (wallCollider != null)
            {
                wallColliders.Add(wallCollider);
            }
        }
    }

    public void MoveObject(Vector3 mousePosition)
    {
        if (isDragging && wallColliders.Count > 0)
        {
            Vector3 curScreenPoint = new Vector3(mousePosition.x, mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset2;
            curPosition.y = transform.position.y;

            float objectHalfLength = transform.localScale.z / 2;
            foreach (Collider wallCollider in wallColliders)
            {
                if (curPosition.x >= wallCollider.bounds.min.x && curPosition.x <= wallCollider.bounds.max.x && curPosition.z >= wallCollider.bounds.min.z && curPosition.z <= wallCollider.bounds.max.z)
                {
                    rotateValue = wallCollider.GetComponent<WMoveScale>().rotateReturn();
                    if (rotateValue == 0 || rotateValue == 2)
                    {
                        curPosition.x = Mathf.Clamp(curPosition.x, wallCollider.bounds.min.x + transform.localScale.x, wallCollider.bounds.max.x - transform.localScale.x);
                        curPosition.z = wallCollider.transform.parent.position.z;
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    }
                    else if (rotateValue == 1 || rotateValue == 3)
                    {
                        curPosition.z = Mathf.Clamp(curPosition.z, wallCollider.bounds.min.z + transform.localScale.x, wallCollider.bounds.max.z - transform.localScale.x);
                        curPosition.x = wallCollider.transform.parent.position.x;
                        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    }
                    transform.position = curPosition;
                }
            }
        }
    }

    public void StartDragging(Vector3 mousePosition)
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        lastMousePosition = mouseWorldPosition;
        lastMousePosition.y = transform.position.y; // ���콺 Y�� ����
        isDragging = true;
    }

    public void StopDragging()
    {
        isDragging = false;
    }

    public void Del()
    {
        Destroy(transform.gameObject);
    }

    public void Select()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) // Ray�� 3D ������Ʈ�� �浹
        {
            GameObject clickedObject = hit.transform.gameObject; // Ŭ���� ������Ʈ

            selectedObject = clickedObject;

            // ���콺 ��ġ�� ��ü ��ġ ���� ���
            screenPoint = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
            offset2 = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }
}
