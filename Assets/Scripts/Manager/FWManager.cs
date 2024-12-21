using HSVPicker;
using UnityEngine;
using UnityEngine.UI;

public class FWManager : MonoBehaviour
{
    public ScrollRect floorCategoty; //스크롤뷰의 ScrollRect 컴포넌트
    public ScrollRect wallCategoty;
    private RectTransform floorContent; //콘텐츠 영역
    private RectTransform wallContent;
    public GameObject furnitureCategory; //가구 카테고리
    public Button categoryButton; //가구 카테고리 버튼

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
                //버튼 클릭 이벤트에 리스너 추가 가능
                button.onClick.AddListener(() => ChangeFloor(button));
            }
        }
        foreach (Transform child in wallContent)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                //버튼 클릭 이벤트에 리스너 추가 가능
                button.onClick.AddListener(() => ChangeWall(button));
            }
        }

    }

    void Update()
    {
        //마우스 클릭 감지
        if (Input.GetMouseButtonDown(0) && furnitureCategory.activeSelf == false) // 왼쪽 클릭
        {
            //클릭 위치에서 레이캐스트를 발사하여 클릭한 물체 감지
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //더블 클릭을 감지하려면 현재 시간과 마지막 클릭 시간 비교
                if (Time.time - lastClickTime < doubleClickThreshold)
                {
                    if (hit.collider.gameObject.CompareTag("FLOOR")) // 바닥 오브젝트 확인
                    {
                        if (wallCategoty.gameObject.activeSelf) //wall이 활성화된 상태
                        {
                            wallCategoty.gameObject.SetActive(false);
                            colorPicker.gameObject.SetActive(false);
                        }
                        floorCategoty.gameObject.SetActive(true); // UI 활성화
                        selectedObject = hit.collider.gameObject;
                        CameraMove.enabled = false;
                    }
                    else if (hit.collider.gameObject.CompareTag("WALL"))
                    {
                        if (floorCategoty.gameObject.activeSelf) //floor이 활성화된 상태
                        {
                            floorCategoty.gameObject.SetActive(false);
                        }
                        wallCategoty.gameObject.SetActive(true); // UI 활성화
                        selectedObject = hit.collider.gameObject;
                        CameraMove.enabled = false;
                    }
                }
                else
                {
                    //첫 번째 클릭일 때는 마지막 클릭 시간 업데이트
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
            //바닥 오브젝트의 Renderer를 찾아 텍스처 변경
            selectedObject.GetComponent<Renderer>().material = floorMaterial;
        }
    }

    private void ActivateColorPicker()
    {
        if (selectedObject != null) // 선택된 오브젝트가 있을 때
        {
            //컬러피커 이벤트 초기화
            colorPicker.onValueChanged.RemoveAllListeners();

            //선택된 벽의 색상을 컬러피커 초기값으로 설정
            Renderer selectedRenderer = selectedObject.GetComponent<Renderer>();
            if (selectedRenderer != null)
            {
                UnityEngine.Color initialColor = selectedRenderer.material.color;
                colorPicker.CurrentColor = initialColor; // 컬러피커 초기값 설정
            }

            //모든 벽 객체에 대해 동일한 색상 변경 로직 설정
            colorPicker.onValueChanged.AddListener(color =>
            {
                foreach (var wall in GameObject.FindGameObjectsWithTag("WALL"))
                {
                    Renderer wallRenderer = wall.GetComponent<Renderer>();
                    if (wallRenderer != null)
                    {
                        wallRenderer.material.color = color; //모든 벽의 색 변경
                    }
                }
            });

            //컬러피커 UI 활성화
            colorPicker.gameObject.SetActive(true);
        }
    }

    private void ChangeWall(Button button)
    {
        //버튼 이름이 colorpicker일 경우, 컬러피커 함수 호출
        if (button.name.Equals("colorpicker", System.StringComparison.OrdinalIgnoreCase))
        {
            categoryButton.gameObject.SetActive(false);
            ActivateColorPicker();
            return; //컬러피커를 사용한 경우 함수 종료
        }

        colorPicker.gameObject.SetActive(false);

        //버튼 이름과 일치하는 재질 로드
        Material wallMaterial = Resources.Load<Material>($"Wall/{button.name}");

        if (wallMaterial != null)
        {
            //"WALL" 태그가 지정된 모든 객체를 찾아 재질 변경
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
