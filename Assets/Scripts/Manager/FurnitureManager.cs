using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureManager : MonoBehaviour
{
    private GameObject selectedObject; //더블 클릭으로 선택된 오브젝트
    private Vector3 screenPoint;
    private Vector3 offset;

    private float lastClickTime = 0f; //마지막 클릭 시간
    private float doubleClickTime = 0.3f; //더블 클릭 간의 최대 시간 (초)
    private bool isDragging = false; //물체를 드래그할지 여부

    private bool isRotating = false; //회전 중인지 여부
    private float rotationSpeed = 10f; //회전 속도
    private float keyHoldTime = 0f;

    void Update()
    {
        //마우스 왼쪽 버튼 클릭 시
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) //ray가 3D 오브젝트와 충돌
            {
                GameObject clickedObject = hit.transform.gameObject; //클릭된 오브젝트

                if (clickedObject.CompareTag("WALL") == false)
                {
                    //더블 클릭 감지
                    if (Time.time - lastClickTime <= doubleClickTime)
                    {
                        //더블 클릭이 감지되면 물체를 선택하고 이동 가능 상태로 설정
                        selectedObject = clickedObject;

                        isDragging = true; //드래그 시작

                        //마우스 위치와 물체 위치 사이 계산
                        screenPoint = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                    }
                    else
                    {
                        //selectedObject = clickedObject;
                    }

                    //마지막 클릭 시간 갱신
                    lastClickTime = Time.time;
                }
            }
        }

        //물체 이동(x, z로만 이동)
        if (isDragging && selectedObject != null && Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z); //마우스의 현재 위치
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset; //월드 좌표로 변환

            curPosition.y = selectedObject.transform.position.y; // 현재 y값을 그대로 유지

            if (!IsColliding(curPosition)) //다른 물체와 충돌하지 않았을 때만 이동
                selectedObject.transform.position = curPosition;
        }

        //물체 회전
        if (selectedObject != null)
        {
            //r 키를 눌러서 90도 회전
            if (Input.GetKeyDown(KeyCode.R))
            {
                //현재 Y축 각도 가져오기
                float currentAngle = selectedObject.transform.rotation.eulerAngles.y;

                //90의 배수로 스냅
                float snappedAngle = Mathf.Round(currentAngle / 90f) * 90f;

                //다음 각도로 이동 (90도씩 증가, 360도 넘으면 0도로 초기화)
                float nextAngle = (snappedAngle + 90f) % 360f;

                //회전 적용
                selectedObject.transform.rotation = Quaternion.Euler(
                    selectedObject.transform.rotation.eulerAngles.x,
                    nextAngle,
                    selectedObject.transform.rotation.eulerAngles.z
                );
            }

            //r 키를 1초 이상 꾹 누르면 회전
            if (Input.GetKey(KeyCode.R))
            {
                keyHoldTime += Time.deltaTime; //누르고 있는 시간 증가

                if (keyHoldTime >= 1f) //1초 이상 눌렀을 때
                {
                    isDragging = false;
                    isRotating = true;
                    selectedObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * 20f);
                }
            }
            else
            {
                //키를 떼면 초기화
                keyHoldTime = 0f;
                isRotating = false;
            }
        }

        if (selectedObject != null && Input.GetKeyDown(KeyCode.Delete)) //객체 삭제
        {
            var selectedFurniture = FCategoryManager.furnitureDataList
                .Find(furniture => furniture.furnitureName == selectedObject.name);

            if (selectedFurniture != null)
            {
                bool isRemoved = FCategoryManager.furnitureDataList.Remove(selectedFurniture);
                if (isRemoved)
                {
                    Debug.Log("리스트에서 해당 가구 데이터가 제거되었습니다: " + selectedFurniture.furnitureName);
                }
            }

            Destroy(selectedObject);
            selectedObject = null;
            isDragging = false;
        }

        //마우스를 놓았을 때 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            //RemoveOutline();
            selectedObject = null;
        }
    }

    private bool IsColliding(Vector3 targetPosition)
    {
        //선택된 물체의 콜라이더와 기본 충돌 범위
        Collider selectedCollider = selectedObject.GetComponent<Collider>(); //Collide로 변경
        Bounds selectedBounds = selectedCollider.bounds;
        Vector3 normalExtents = selectedBounds.extents; //기본 충돌 범위

        //움직이는 가구의 확장 범위를 결정
        float expandFactor = 0f; // 기본 확장 범위는 0
        var selectedFurniture = FCategoryManager.furnitureDataList
            .Find(furniture => furniture.furnitureName == selectedObject.name);

        if (selectedFurniture != null && selectedFurniture.collideCm != 0)
        {
            expandFactor = selectedFurniture.collideCm; // 자신의 확장 범위를 가져옴
        }

        //벽과의 충돌 확인 (기본 범위만 사용)
        Collider[] wallColliders = Physics.OverlapBox(targetPosition, normalExtents, Quaternion.identity);
        foreach (Collider wallCollider in wallColliders)
        {
            if (wallCollider.gameObject.tag == "WALL" && wallCollider.gameObject != selectedObject)
            {
                return true;
            }
        }

        // 다른 가구와의 충돌 확인 (상대 가구의 확장 범위 적용)
        Collider[] colliders = Physics.OverlapBox(targetPosition, normalExtents + new Vector3(expandFactor, expandFactor, expandFactor), Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != selectedObject && collider.gameObject.tag == "FURNITURE")
            {
                //충돌한 가구의 `collideCm` 값을 가져와서 충돌 범위를 확장
                var otherFurniture = FCategoryManager.furnitureDataList
                    .Find(furniture => furniture.furnitureName == collider.gameObject.name);

                float otherExpandFactor = 0f; //기본 확장 범위
                if (otherFurniture != null && otherFurniture.collideCm != 0)
                {
                    otherExpandFactor = otherFurniture.collideCm;
                }

                //상대 가구의 확장된 범위와 충돌 여부 확인
                Bounds otherBounds = collider.bounds;
                Vector3 otherExtents = otherBounds.extents + new Vector3(otherExpandFactor, otherExpandFactor, otherExpandFactor);
                if (Physics.CheckBox(collider.bounds.center, otherExtents, Quaternion.identity))
                {
                    return true;
                }
            }
        }

        //충돌 없음
        return false;
    }
}