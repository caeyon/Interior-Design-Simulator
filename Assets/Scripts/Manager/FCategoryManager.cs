using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FCategoryManager : MonoBehaviour
{
    public static FCategoryManager Instance { get; private set; } //싱글톤 인스턴스

    public Image heightBackground;
    public TMP_InputField heightInput; //키 입력창
    public Button categoryButton; //카테고리 여는 버튼
    public ScrollRect scrollView; 
    private RectTransform content; //스크롤뷰 콘텐츠

    CameraController mainCamera;
    public GameObject player;

    private float lastClickTime = 0f; //마지막 클릭 시간
    private float doubleClickTime = 0.3f; //더블 클릭 간의 최대 시간 (초)

    string siteName;
    string size;

    public static List<FurnitureData> furnitureDataList = new List<FurnitureData>();

    void Awake()
    {
        //이미 인스턴스가 존재하는지 확인
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //기존 인스턴스가 있으면 새로운 오브젝트는 삭제
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //씬 전환 시 오브젝트 유지
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
                //버튼 클릭 이벤트에 리스너 추가 가능
                button.onClick.AddListener(() => loadFurniture(button));
            }
        }
    }

    private void loadFurniture(Button button)
    {
        if (Time.time - lastClickTime <= doubleClickTime) //더블 클릭
        {
            //버튼 이름을 기반으로 리소스를 로드
            var resource = Resources.Load<GameObject>($"Furniture/{button.name}");
            if (resource != null)
            {
                //리소스를 인스턴스화하고, 원하는 위치에 배치
                Vector3 spawnPosition = player.transform.position + player.transform.forward * 2.0f; //플레이어 앞쪽 위치 계산
                spawnPosition.y = 0; //바닥에 배치
                GameObject spawnedObject = Instantiate(resource, spawnPosition, Quaternion.identity); //프리팹 생성

                spawnedObject.name = GenerateUniqueName(button.name); //이름 (Clone)부분 삭제+가구 숫자화

                FurnitureDatabase.getFurniture(button.name, (f) =>
                {
                    if (f != null)
                    {
                        siteName = f.name;
                        size = f.size;
                        furnitureDataList.Add(new FurnitureData(spawnedObject.name, button.name, siteName, size)); //로드된 가구 리스트에 넣기
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

        //같은 이름으로 시작하는 모든 오브젝트를 찾아 개수 확인
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith(baseName))
            {
                //"_" 뒤에 있는 숫자를 확인하여 현재 개수에 포함
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

        //최종적으로 고유 이름을 설정
        uniqueName = baseName + (count > 0 ? "_" + count : "");
        return uniqueName;
    }

    void openCategory() //가구 카테고리 열기
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
        if (Input.GetKeyDown(KeyCode.Tab)) //Tab키 누를 시-> 카테고리 열림
        {
            openCategory();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //esc 누를 시-> 카테고리 닫힘
        {
            categoryButton.gameObject.SetActive(true);
            scrollView.gameObject.SetActive(false);
            heightInput.gameObject.SetActive(false);
            heightBackground.gameObject.SetActive(false);
            mainCamera.enabled = true;
        }
    }
}
