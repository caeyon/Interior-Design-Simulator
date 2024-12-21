using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureManager : MonoBehaviour
{
    private GameObject selectedObject; //���� Ŭ������ ���õ� ������Ʈ
    private Vector3 screenPoint;
    private Vector3 offset;

    private float lastClickTime = 0f; //������ Ŭ�� �ð�
    private float doubleClickTime = 0.3f; //���� Ŭ�� ���� �ִ� �ð� (��)
    private bool isDragging = false; //��ü�� �巡������ ����

    private bool isRotating = false; //ȸ�� ������ ����
    private float rotationSpeed = 10f; //ȸ�� �ӵ�
    private float keyHoldTime = 0f;

    void Update()
    {
        //���콺 ���� ��ư Ŭ�� ��
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) //ray�� 3D ������Ʈ�� �浹
            {
                GameObject clickedObject = hit.transform.gameObject; //Ŭ���� ������Ʈ

                if (clickedObject.CompareTag("WALL") == false)
                {
                    //���� Ŭ�� ����
                    if (Time.time - lastClickTime <= doubleClickTime)
                    {
                        //���� Ŭ���� �����Ǹ� ��ü�� �����ϰ� �̵� ���� ���·� ����
                        selectedObject = clickedObject;

                        isDragging = true; //�巡�� ����

                        //���콺 ��ġ�� ��ü ��ġ ���� ���
                        screenPoint = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                    }
                    else
                    {
                        //selectedObject = clickedObject;
                    }

                    //������ Ŭ�� �ð� ����
                    lastClickTime = Time.time;
                }
            }
        }

        //��ü �̵�(x, z�θ� �̵�)
        if (isDragging && selectedObject != null && Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z); //���콺�� ���� ��ġ
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset; //���� ��ǥ�� ��ȯ

            curPosition.y = selectedObject.transform.position.y; // ���� y���� �״�� ����

            if (!IsColliding(curPosition)) //�ٸ� ��ü�� �浹���� �ʾ��� ���� �̵�
                selectedObject.transform.position = curPosition;
        }

        //��ü ȸ��
        if (selectedObject != null)
        {
            //r Ű�� ������ 90�� ȸ��
            if (Input.GetKeyDown(KeyCode.R))
            {
                //���� Y�� ���� ��������
                float currentAngle = selectedObject.transform.rotation.eulerAngles.y;

                //90�� ����� ����
                float snappedAngle = Mathf.Round(currentAngle / 90f) * 90f;

                //���� ������ �̵� (90���� ����, 360�� ������ 0���� �ʱ�ȭ)
                float nextAngle = (snappedAngle + 90f) % 360f;

                //ȸ�� ����
                selectedObject.transform.rotation = Quaternion.Euler(
                    selectedObject.transform.rotation.eulerAngles.x,
                    nextAngle,
                    selectedObject.transform.rotation.eulerAngles.z
                );
            }

            //r Ű�� 1�� �̻� �� ������ ȸ��
            if (Input.GetKey(KeyCode.R))
            {
                keyHoldTime += Time.deltaTime; //������ �ִ� �ð� ����

                if (keyHoldTime >= 1f) //1�� �̻� ������ ��
                {
                    isDragging = false;
                    isRotating = true;
                    selectedObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * 20f);
                }
            }
            else
            {
                //Ű�� ���� �ʱ�ȭ
                keyHoldTime = 0f;
                isRotating = false;
            }
        }

        if (selectedObject != null && Input.GetKeyDown(KeyCode.Delete)) //��ü ����
        {
            var selectedFurniture = FCategoryManager.furnitureDataList
                .Find(furniture => furniture.furnitureName == selectedObject.name);

            if (selectedFurniture != null)
            {
                bool isRemoved = FCategoryManager.furnitureDataList.Remove(selectedFurniture);
                if (isRemoved)
                {
                    Debug.Log("����Ʈ���� �ش� ���� �����Ͱ� ���ŵǾ����ϴ�: " + selectedFurniture.furnitureName);
                }
            }

            Destroy(selectedObject);
            selectedObject = null;
            isDragging = false;
        }

        //���콺�� ������ �� �巡�� ����
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            //RemoveOutline();
            selectedObject = null;
        }
    }

    private bool IsColliding(Vector3 targetPosition)
    {
        //���õ� ��ü�� �ݶ��̴��� �⺻ �浹 ����
        Collider selectedCollider = selectedObject.GetComponent<Collider>(); //Collide�� ����
        Bounds selectedBounds = selectedCollider.bounds;
        Vector3 normalExtents = selectedBounds.extents; //�⺻ �浹 ����

        //�����̴� ������ Ȯ�� ������ ����
        float expandFactor = 0f; // �⺻ Ȯ�� ������ 0
        var selectedFurniture = FCategoryManager.furnitureDataList
            .Find(furniture => furniture.furnitureName == selectedObject.name);

        if (selectedFurniture != null && selectedFurniture.collideCm != 0)
        {
            expandFactor = selectedFurniture.collideCm; // �ڽ��� Ȯ�� ������ ������
        }

        //������ �浹 Ȯ�� (�⺻ ������ ���)
        Collider[] wallColliders = Physics.OverlapBox(targetPosition, normalExtents, Quaternion.identity);
        foreach (Collider wallCollider in wallColliders)
        {
            if (wallCollider.gameObject.tag == "WALL" && wallCollider.gameObject != selectedObject)
            {
                return true;
            }
        }

        // �ٸ� �������� �浹 Ȯ�� (��� ������ Ȯ�� ���� ����)
        Collider[] colliders = Physics.OverlapBox(targetPosition, normalExtents + new Vector3(expandFactor, expandFactor, expandFactor), Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != selectedObject && collider.gameObject.tag == "FURNITURE")
            {
                //�浹�� ������ `collideCm` ���� �����ͼ� �浹 ������ Ȯ��
                var otherFurniture = FCategoryManager.furnitureDataList
                    .Find(furniture => furniture.furnitureName == collider.gameObject.name);

                float otherExpandFactor = 0f; //�⺻ Ȯ�� ����
                if (otherFurniture != null && otherFurniture.collideCm != 0)
                {
                    otherExpandFactor = otherFurniture.collideCm;
                }

                //��� ������ Ȯ��� ������ �浹 ���� Ȯ��
                Bounds otherBounds = collider.bounds;
                Vector3 otherExtents = otherBounds.extents + new Vector3(otherExpandFactor, otherExpandFactor, otherExpandFactor);
                if (Physics.CheckBox(collider.bounds.center, otherExtents, Quaternion.identity))
                {
                    return true;
                }
            }
        }

        //�浹 ����
        return false;
    }
}