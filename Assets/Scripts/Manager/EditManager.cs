using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditManager : MonoBehaviour
{
    public Button  showList; //����Ʈ���� ��ư 
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

            //Raycast�� ������Ʈ�� �浹�ߴ��� Ȯ��
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
                return; //UI�� Ŭ���� ���, �Ʒ� �ڵ� ���� �� ��
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

        if (float.TryParse(input, out collide)) //�Է��� ���ڷ� ��ȿ���� Ȯ��
        {
            collide = collide / 100f;

            if (selectedObject != null)
            {
                //���õ� ������Ʈ �̸��� ��ġ�ϴ� ���� �����͸� ã��
                var selectedFurniture = FCategoryManager.furnitureDataList
                    .Find(furniture => furniture.furnitureName == selectedObject.name);

                if (selectedFurniture != null)
                {
                    selectedFurniture.setCollideCm(collide); //�浹 ���� ����
                    Debug.Log("�浹 ������ �����Ǿ����ϴ�: " + selectedFurniture.collideCm);
                }
            }

        }
    }
}
