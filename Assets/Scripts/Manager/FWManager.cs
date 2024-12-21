using HSVPicker;
using UnityEngine;
using UnityEngine.UI;

public class FWManager : MonoBehaviour
{
    public ScrollRect floorCategoty; //��ũ�Ѻ��� ScrollRect ������Ʈ
    public ScrollRect wallCategoty;
    private RectTransform floorContent; //������ ����
    private RectTransform wallContent;
    public GameObject furnitureCategory; //���� ī�װ�
    public Button categoryButton; //���� ī�װ� ��ư

    public GameObject player;
    private GameObject selectedObject;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    private Camera mainCamera;
    CameraController CameraMove;

    public ColorPicker colorPicker;

    void Start()
    {
        CameraMove = Camera.main.GetComponent<CameraController>();

        mainCamera = Camera.main;

        floorContent = floorCategoty.content;
        wallContent = wallCategoty.content;

        foreach (Transform child in floorContent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //��ư Ŭ�� �̺�Ʈ�� ������ �߰� ����
                button.onClick.AddListener(() => ChangeFloor(button));
            }
        }
        foreach (Transform child in wallContent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //��ư Ŭ�� �̺�Ʈ�� ������ �߰� ����
                button.onClick.AddListener(() => ChangeWall(button));
            }
        }

    }

    void Update()
    {
        //���콺 Ŭ�� ����
        if (Input.GetMouseButtonDown(0) && furnitureCategory.activeSelf == false) // ���� Ŭ��
        {
            //Ŭ�� ��ġ���� ����ĳ��Ʈ�� �߻��Ͽ� Ŭ���� ��ü ����
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //���� Ŭ���� �����Ϸ��� ���� �ð��� ������ Ŭ�� �ð� ��
                if (Time.time - lastClickTime < doubleClickThreshold)
                {
                    if (hit.collider.gameObject.CompareTag("FLOOR")) // �ٴ� ������Ʈ Ȯ��
                    {
                        if (wallCategoty.gameObject.activeSelf) //wall�� Ȱ��ȭ�� ����
                        {
                            wallCategoty.gameObject.SetActive(false);
                            colorPicker.gameObject.SetActive(false);
                        }
                        floorCategoty.gameObject.SetActive(true); // UI Ȱ��ȭ
                        selectedObject = hit.collider.gameObject;
                        CameraMove.enabled = false;
                    }
                    else if (hit.collider.gameObject.CompareTag("WALL"))
                    {
                        if (floorCategoty.gameObject.activeSelf) //floor�� Ȱ��ȭ�� ����
                        {
                            floorCategoty.gameObject.SetActive(false);
                        }
                        wallCategoty.gameObject.SetActive(true); // UI Ȱ��ȭ
                        selectedObject = hit.collider.gameObject;
                        CameraMove.enabled = false;
                    }
                }
                else
                {
                    //ù ��° Ŭ���� ���� ������ Ŭ�� �ð� ������Ʈ
                    lastClickTime = Time.time;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedObject = null;
            floorCategoty.gameObject.SetActive(false);
            wallCategoty.gameObject.SetActive(false);
            colorPicker.gameObject.SetActive(false);
            categoryButton.gameObject.SetActive(true);
            CameraMove.enabled = true;
        }
    }

    private void ChangeFloor(Button button)
    {
        Material floorMaterial = Resources.Load<Material>($"Floor/{button.name}");

        if (floorMaterial != null && selectedObject != null)
        {
            //�ٴ� ������Ʈ�� Renderer�� ã�� �ؽ�ó ����
            selectedObject.GetComponent<Renderer>().material = floorMaterial;
        }
    }

    private void ActivateColorPicker()
    {
        if (selectedObject != null) // ���õ� ������Ʈ�� ���� ��
        {
            //�÷���Ŀ �̺�Ʈ �ʱ�ȭ
            colorPicker.onValueChanged.RemoveAllListeners();

            //���õ� ���� ������ �÷���Ŀ �ʱⰪ���� ����
            Renderer selectedRenderer = selectedObject.GetComponent<Renderer>();
            if (selectedRenderer != null)
            {
                UnityEngine.Color initialColor = selectedRenderer.material.color;
                colorPicker.CurrentColor = initialColor; // �÷���Ŀ �ʱⰪ ����
            }

            //��� �� ��ü�� ���� ������ ���� ���� ���� ����
            colorPicker.onValueChanged.AddListener(color =>
            {
                foreach (var wall in GameObject.FindGameObjectsWithTag("WALL"))
                {
                    Renderer wallRenderer = wall.GetComponent<Renderer>();
                    if (wallRenderer != null)
                    {
                        wallRenderer.material.color = color; //��� ���� �� ����
                    }
                }
            });

            //�÷���Ŀ UI Ȱ��ȭ
            colorPicker.gameObject.SetActive(true);
        }
    }

    private void ChangeWall(Button button)
    {
        //��ư �̸��� colorpicker�� ���, �÷���Ŀ �Լ� ȣ��
        if (button.name.Equals("colorpicker", System.StringComparison.OrdinalIgnoreCase))
        {
            categoryButton.gameObject.SetActive(false);
            ActivateColorPicker();
            return; //�÷���Ŀ�� ����� ��� �Լ� ����
        }

        colorPicker.gameObject.SetActive(false);

        //��ư �̸��� ��ġ�ϴ� ���� �ε�
        Material wallMaterial = Resources.Load<Material>($"Wall/{button.name}");

        if (wallMaterial != null)
        {
            //"WALL" �±װ� ������ ��� ��ü�� ã�� ���� ����
            GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("WALL");
            foreach (GameObject wall in wallObjects)
            {
                Renderer wallRenderer = wall.GetComponent<Renderer>();
                if (wallRenderer != null)
                {
                    wallRenderer.material = wallMaterial;
                }
            }
        }
    }
}
