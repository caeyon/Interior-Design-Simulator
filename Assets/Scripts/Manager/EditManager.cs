using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditManager : MonoBehaviour
{
    public Button  showList; //리스트보기 버튼 
    public Image collideBackground;
    public TMP_InputField collideInput;

    public GameObject player;
    CameraController mainCamera;

    public GameObject selectedObject;

    private int startEdit = 0;

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();

        collideInput.onEndEdit.AddListener(editCollide);
    }

    void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;

            //Raycast가 오브젝트와 충돌했는지 확인
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("FURNITURE") && selectedObject == null) // && selectedObject == null
            {
                selectedObject = hit.transform.gameObject;
                //menuBackground.gameObject.SetActive(true);
                showList.gameObject.SetActive(false);
                collideBackground.gameObject.SetActive(true);
                collideInput.text = "";
                mainCamera.enabled = false;

                startEdit = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && startEdit == 1)
        {
            collideBackground.gameObject.SetActive(false);
            showList.gameObject.SetActive(true);
        }
        if (Input.GetMouseButtonDown(0) && startEdit == 1)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; //UI를 클릭한 경우, 아래 코드 실행 안 함
            }

            startEdit = 0;
            selectedObject = null;

            showList.gameObject.SetActive(true);
            collideBackground.gameObject.SetActive(false);
            mainCamera.enabled = true;
        }
    }

    void editCollide(string input)
    {
        float collide;

        if (float.TryParse(input, out collide)) //입력이 숫자로 유효한지 확인
        {
            collide = collide / 100f;

            if (selectedObject != null)
            {
                //선택된 오브젝트 이름과 일치하는 가구 데이터를 찾음
                var selectedFurniture = FCategoryManager.furnitureDataList
                    .Find(furniture => furniture.furnitureName == selectedObject.name);

                if (selectedFurniture != null)
                {
                    selectedFurniture.setCollideCm(collide); //충돌 범위 설정
                    Debug.Log("충돌 범위가 설정되었습니다: " + selectedFurniture.collideCm);
                }
            }

        }
    }
}
