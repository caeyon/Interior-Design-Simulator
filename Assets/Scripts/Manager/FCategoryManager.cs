using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FCategoryManager : MonoBehaviour
{
    public static FCategoryManager Instance { get; private set; } //�̱��� �ν��Ͻ�

    public Image heightBackground;
    public TMP_InputField heightInput; //Ű �Է�â
    public Button categoryButton; //ī�װ� ���� ��ư
    public ScrollRect scrollView; 
    private RectTransform content; //��ũ�Ѻ� ������

    CameraController mainCamera;
    public GameObject player;

    private float lastClickTime = 0f; //������ Ŭ�� �ð�
    private float doubleClickTime = 0.3f; //���� Ŭ�� ���� �ִ� �ð� (��)

    string siteName;
    string size;

    public static List<FurnitureData> furnitureDataList = new List<FurnitureData>();

    void Awake()
    {
        //�̹� �ν��Ͻ��� �����ϴ��� Ȯ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //���� �ν��Ͻ��� ������ ���ο� ������Ʈ�� ����
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //�� ��ȯ �� ������Ʈ ����
        }
    }

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();

        categoryButton.onClick.AddListener(openCategory);

        content = scrollView.content;

        foreach (Transform child in content)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //��ư Ŭ�� �̺�Ʈ�� ������ �߰� ����
                button.onClick.AddListener(() => loadFurniture(button));
            }
        }
    }

    private void loadFurniture(Button button)
    {
        if (Time.time - lastClickTime <= doubleClickTime) //���� Ŭ��
        {
            //��ư �̸��� ������� ���ҽ��� �ε�
            var resource = Resources.Load<GameObject>($"Furniture/{button.name}");
            if (resource != null)
            {
                //���ҽ��� �ν��Ͻ�ȭ�ϰ�, ���ϴ� ��ġ�� ��ġ
                Vector3 spawnPosition = player.transform.position + player.transform.forward * 2.0f; //�÷��̾� ���� ��ġ ���
                spawnPosition.y = 0; //�ٴڿ� ��ġ
                GameObject spawnedObject = Instantiate(resource, spawnPosition, Quaternion.identity); //������ ����

                spawnedObject.name = GenerateUniqueName(button.name); //�̸� (Clone)�κ� ����+���� ����ȭ

                FurnitureDatabase.getFurniture(button.name, (f) =>
                {
                    if (f != null)
                    {
                        siteName = f.name;
                        size = f.size;
                        furnitureDataList.Add(new FurnitureData(spawnedObject.name, button.name, siteName, size)); //�ε�� ���� ����Ʈ�� �ֱ�
                    }
                });
            }
        }
        lastClickTime = Time.time;
    }

    string GenerateUniqueName(string baseName)
    {
        int count = 0;
        string uniqueName = baseName;

        //���� �̸����� �����ϴ� ��� ������Ʈ�� ã�� ���� Ȯ��
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith(baseName))
            {
                //"_" �ڿ� �ִ� ���ڸ� Ȯ���Ͽ� ���� ������ ����
                string suffix = obj.name.Substring(baseName.Length);
                if (suffix.StartsWith("_") && int.TryParse(suffix.Substring(1), out int num))
                {
                    count = Mathf.Max(count, num + 1);
                }
                else
                {
                    count = Mathf.Max(count, 1);
                }
            }
        }

        //���������� ���� �̸��� ����
        uniqueName = baseName + (count > 0 ? "_" + count : "");
        return uniqueName;
    }

    void openCategory() //���� ī�װ� ����
    {
        if (categoryButton != null)
        {
            categoryButton.gameObject.SetActive(false);
            scrollView.gameObject.SetActive(true);
            heightInput.gameObject.SetActive(true);
            heightBackground.gameObject.SetActive(true);
            mainCamera.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) //TabŰ ���� ��-> ī�װ� ����
        {
            openCategory();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //esc ���� ��-> ī�װ� ����
        {
            categoryButton.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(false);
            heightInput.gameObject.SetActive(false);
            heightBackground.gameObject.SetActive(false);
            mainCamera.enabled = true;
        }
    }
}
